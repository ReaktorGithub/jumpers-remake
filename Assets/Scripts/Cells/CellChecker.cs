using System.Collections.Generic;
using UnityEngine;

public class CellChecker : MonoBehaviour
{
    public static CellChecker Instance { get; private set; }
    private TopPanel _topPanel;
    private CameraControl _camera;
    private PopupAttack _popupAttack;
    [SerializeField] private List<ECellTypes> _skipRivalCheckTypes = new();
    private ModalReplaceEffect _modalReplace;
    private ModalMoneybox _modalMoneybox;

    private void Awake() {
        Instance = this;
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _popupAttack = GameObject.Find("GameScripts").GetComponent<PopupAttack>();
        _modalReplace = GameObject.Find("ModalScripts").GetComponent<ModalReplaceEffect>();
        _modalMoneybox = GameObject.Find("ModalScripts").GetComponent<ModalMoneybox>();
    }

    // Проверка клетки перед совершением хода

    public bool CheckCellBeforeStep(PlayerControl player) {
        return CheckBranch(player);
    }

    public bool CheckBranch(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        if (cell.TryGetComponent(out BranchCell branchCell)) {
            BranchControl branch = branchCell.BranchControl;
            
            // Если игрок пришел сюда из ветки с тупиком, то сбросить параметры
            // !!! Выбор бранча не скипать !!!
            if (player.IsDeadEndMode && player.IsReverseMove && player.StepsLeft >= 0) {
                player.IsDeadEndMode = false;
                player.IsReverseMove = false;
            }

            // Если направление фишки не соответствует направлению бранча, то выбор бранча не вызывать
            if (player.IsReverseMove != branch.IsReverse) {
                return true;
            }
            
            // Направление фишки соответствует направлению бранча
            ActivateBranch(player, branch, player.StepsLeft);
            return false;
        }

        // Клетка не является бранчем, продолжаем движение
        return true;
    }

    public void ActivateBranch(PlayerControl player, BranchControl branch, int rest) {
        string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " выбирает направление";
        Messages.Instance.AddMessage(message);

        if (player.IsMe()) {
            branch.ShowAllBranches();
            _topPanel.SetText("Выберите направление");
            _topPanel.SetCancelButtonActive(false);
            _topPanel.OpenWindow();
            if (!branch.IsHedgehog()) {
                string cubicStatus = Utils.Wrap("Остаток: " + rest, UIColors.Green);
                CubicControl.Instance.WriteStatus(cubicStatus);
            }
        }

        if (player.IsAi()) {
            AiControl.Instance.AiSelectBranch(player, branch, rest);
        }
    }

    // Проверка во время хода при достижении новой клетки

    public bool CheckCellAfterStep(CellControl currentCell, PlayerControl player) {
        ECellTypes cellType = currentCell.CellType;

        if (cellType == ECellTypes.Checkpoint) {
            string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " достигает " + Utils.Wrap("чекпойнта", UIColors.Blue);
            Messages.Instance.AddMessage(message);
        }

        // Могут вызвать прерывание

        if (currentCell.IsWallEffect()) {
            MoveControl.Instance.BreakMovingAndConfirmNewPosition();
            return false;
        }

        if (cellType == ECellTypes.Finish) {
            currentCell.TryGetComponent(out FinishCell finish);
            if (finish != null) {
                bool isHedgehog = finish.CheckHedgehog();
                if (isHedgehog) {
                   return false; 
                }
            }
            player.Effects.ExecuteFinish();
            return false;
        }

        if (cellType == ECellTypes.Start && player.IsReverseMove) {
            player.IsReverseMove = false;
            MoveControl.Instance.BreakMovingAndConfirmNewPosition();
            return false;
        }

        if (cellType == ECellTypes.Wall) {
            player.Effects.ExecuteWall(false);
            MoveControl.Instance.BreakMovingAndConfirmNewPosition();
            return false;
        }

        return true;
    }

    /*
        Начало серии проверок клетки по окончании хода
        1. Проверка на ежа.
        2. Подбор бонуса.
        3. Проверка на капкан.
        4. Проверка, может ли игрок избежать вредного эффекта.
        5. Исполнение эффекта.
        6. Исполнение эффекта «стрелка».
        7. Атака на соперников.
    */

    public void CheckCellAfterMove(PlayerControl player) {
        CheckCellHedgehog(player);
    }

    private void CheckCellHedgehog(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        if (cell.CellType == ECellTypes.HedgehogBranch) {
            if (cell.TryGetComponent(out BranchCell branchCell)) {
                BranchControl branch = branchCell.BranchControl;
                ActivateBranch(player, branch, 0);
                return;
            }
        }

        CheckPickableBonus(player, cell);
    }

    private void CheckPickableBonus(PlayerControl player, CellControl cell) {
        if (cell.PickableType != EPickables.None) {
            if (player.Boosters.IsBlot()) {
                player.Boosters.ExecuteBlotAsVictim("получить бонус");
            } else {
                player.ExecutePickableBonus(cell);
            }
        }

        CheckTrap(player, cell);
    }

    private void CheckTrap(PlayerControl player, CellControl cell) {
        if (cell.WhosTrap != null) {
            player.ExecuteTrap(cell);
        }

        CheckCellCharacter(player, cell);
    }

    // Проверка, может ли игрок избежать вредного эффекта

    public void CheckCellCharacter(PlayerControl player, CellControl cell) {
        if (cell.IsNegativeEffect() && player.Effects.IsEnoughEffects(cell.Effect)) {
            if (player.IsAi()) {
                // todo научить принимать решения
                CheckCellEffects(player);
                return;
            }
            EffectsControl.Instance.SelectedEffect = cell.Effect;
            _modalReplace.BuildContent(player);
            _modalReplace.OpenModal();
            return;
        }

        CheckCellEffects(player);
    }

    public void CheckCellEffects(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();
        bool isBlot = player.Boosters.IsBlot();

        if (cell.Effect == EControllableEffects.Green) {
            if (isBlot) {
                player.Boosters.ExecuteBlotAsVictim("получить зелёный эффект");
            } else {
                player.Effects.ExecuteGreen();
            }
        }

        if (cell.Effect == EControllableEffects.Yellow) {
            player.Effects.ExecuteYellow();
        }

        if (cell.Effect == EControllableEffects.Star) {
            if (isBlot) {
                player.Boosters.ExecuteBlotAsVictim("получить звезду");
            } else {
                player.Effects.ExecuteStar();
            }
        }

        if (cell.CellType == ECellTypes.Moneybox) {
            CheckMoneyboxAfterMove(player);
        }

        if (cell.CellType == ECellTypes.Lightning) {
            if (isBlot) {
                player.Boosters.ExecuteBlotAsVictim("получить молнию");
            } else {
                player.Effects.ExecuteLightning(false);
            }
        }

        if (cell.CoinBonusValue != 0) {
            if (isBlot && cell.CoinBonusValue > 0) {
                player.Boosters.ExecuteBlotAsVictim("получить монеты");
            } else {
                player.Effects.ExecuteCoinBonus(cell.CoinBonusValue);
            }
        }

        if (cell.CellType == ECellTypes.Wall) {
            player.Effects.ExecuteWall(true);
        }

        // Отменяют дальнейшую серию проверок

        if (cell.Effect == EControllableEffects.Black) {
            player.Effects.ExecuteBlack();
            return;
        }

        if (cell.Effect == EControllableEffects.Red) {
            player.Effects.ExecuteRed();
            return;
        }

        if (cell.CellType == ECellTypes.Teleport) {
            player.Effects.ExecuteTeleport();
            return;
        }

        CheckCellArrows(player);
    }

    public void CheckCellArrows(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();
        TokenControl token = player.GetTokenControl();

        if (cell.transform.TryGetComponent(out ArrowCell arrowCell)) {
            token.ExecuteArrowMove(arrowCell.ArrowSpline);
            string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " перемещается по стрелке";
            Messages.Instance.AddMessage(message);
            return;
        }

        CheckCellRivals(player);
    }

    public void CheckCellRivals(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        // на некоторых клетках атаки не проводятся
        if (_skipRivalCheckTypes.Contains(cell.CellType)) {
            StartCoroutine(MoveControl.Instance.EndMoveDefer());
            return;
        }
        
        List<GameObject> tokens = cell.CurrentTokens;

        // Для проверки на атаку должно быть больше 1 фишки на клетке
        // Сортировать соперников на тех, что со щитами и без щитов
        // Применить эффекты щитов
        // Если еще остались соперники без щитов, то открыть диалоговое окно с атакой

        if (tokens.Count > 1) {
            List<PlayerControl> rivalsUnprotected = new();
            List<PlayerControl> rivalsWithShields = new();
            List<PlayerControl> rivals = new();

            foreach(PlayerControl playerForCheck in PlayersControl.Instance.Players) {
                if (tokens.Contains(playerForCheck.TokenObject) && player.TokenObject != playerForCheck.TokenObject) {
                    rivals.Add(playerForCheck);
                    if (playerForCheck.Boosters.Armor > 0) {
                        rivalsWithShields.Add(playerForCheck);
                    } else {
                        rivalsUnprotected.Add(playerForCheck);
                    }
                }
            }

            if (rivalsWithShields.Count > 0) {
                player.Boosters.HarvestShieldBonus(rivalsWithShields);
            }

            if (rivalsUnprotected.Count > 0) {
                if (player.IsAi()) {
                    AiControl.Instance.AiAttackPlayer(player, rivalsUnprotected);
                } else {
                    _popupAttack.BuildContent(player, rivals);
                    _popupAttack.OnOpenWindow();
                }
                return;
            }
        }

        // закончить ход, если нет соперников

        StartCoroutine(MoveControl.Instance.EndMoveDefer());
    }

    // Проверка копилки

    public bool CheckMoneyboxBeforeMove(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        if (cell.CellType != ECellTypes.Moneybox) {
            return true;
        }
        
        MoneyboxVault vault = cell.GetComponent<MoneyboxCell>().MoneyboxVault;

        if (vault.IsOver) {
            return true;
        }

        if (vault.OccupiedPlayer == player) {
            ActivateMoneyboxDialogue(player, vault);
            return false;
        }

        return true;
    }

    private void ActivateMoneyboxDialogue(PlayerControl player, MoneyboxVault vault) {
        if (player.IsMe()) {
            _modalMoneybox.BuildContent(player);
            _modalMoneybox.OpenModal();
        }

        if (player.IsAi()) {
            AiControl.Instance.AiMoneybox(player, vault);
        }
    }

    private void CheckMoneyboxAfterMove(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        if (cell.CellType != ECellTypes.Moneybox) {
            return;
        }

        MoneyboxVault vault = cell.GetComponent<MoneyboxCell>().MoneyboxVault;

        if (vault.IsOver) {
            return;
        }

        if (vault.OccupiedPlayer == null) {
            if (player.Boosters.IsBlot()) {
                player.Boosters.ExecuteBlotAsVictim("попасть в копилку");
            } else {
                vault.PutPlayerToVault(player);
                if (MoveControl.Instance.IsLassoMode) {
                    ActivateMoneyboxDialogue(player, vault);
                }
            }
        }
    }
}

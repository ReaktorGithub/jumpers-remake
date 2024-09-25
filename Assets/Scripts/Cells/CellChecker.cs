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

    public bool CheckBranch(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        if (cell.TryGetComponent(out BranchCell branchCell)) {
            BranchControl branch = branchCell.BranchControl;

            // Если фишка движется назад, а бранч не реверс, то у фишки надо сменить направление, затем активируем бранч
            if (player.IsReverseMove && !branch.IsReverse) {
                player.IsReverseMove = false;
                ActivateBranch(player, branch, player.StepsLeft);
                return false;
            }

            // Если направление фишки не соответствует направлению бранча, то скипаем
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

    // Проверка по время хода при достижении новой клетки

    public bool CheckCellAfterStep(CellControl currentCell, PlayerControl player) {
        ECellTypes cellType = currentCell.CellType;

        if (cellType == ECellTypes.Checkpoint) {
            string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " достигает " + Utils.Wrap("чекпойнта", UIColors.Blue);
            Messages.Instance.AddMessage(message);
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

        if (cellType == ECellTypes.Start && player.IsReverseMove || cellType == ECellTypes.Wall && !player.IsReverseMove) {
            player.IsReverseMove = !player.IsReverseMove;
            MoveControl.Instance.BreakMovingAndConfirmNewPosition();
            if (cellType == ECellTypes.Wall) {
                string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " уткнулся носом в " + Utils.Wrap("стену", UIColors.Brick);
                Messages.Instance.AddMessage(message);
            }
            return false;
        }

        return true;
    }

    // Начало серии проверок клетки по окончании хода

    public void CheckCellAfterMove(PlayerControl player) {
        CheckCellHedgehog(player);
    }

    public void CheckCellHedgehog(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        if (cell.CellType == ECellTypes.HedgehogBranch) {
            if (cell.TryGetComponent(out BranchCell branchCell)) {
                BranchControl branch = branchCell.BranchControl;
                ActivateBranch(player, branch, 0);
                return;
            }
        }

        CheckCellCharacter(player);
    }

    // Проверка, может ли игрок избежать вредного эффекта

    public void CheckCellCharacter(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();
        
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

    // Необходимые действия перед завершением хода:
    // 1.	Подбор бонуса.
    // 2.	Срабатывание капкана.
    // 3.	Исполнение эффекта.
    // 4.	Исполнение эффекта «стрелка», либо «синяя стрелка».
    // 5.	Атака на соперников.

    public void CheckCellEffects(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        if (cell.Effect == EControllableEffects.Green) {
            player.AddMovesToDo(1);
            string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("зелёный", UIColors.Green) + " эффект и походит ещё раз";
            Messages.Instance.AddMessage(message);
        }

        if (cell.Effect == EControllableEffects.Yellow) {
            player.SkipMoveIncrease();
            string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("жёлтый", UIColors.Yellow) + " эффект и пропустит ход";
            Messages.Instance.AddMessage(message);
        }

        if (cell.Effect == EControllableEffects.Star) {
            player.Effects.ExecuteStar();
        }

        if (cell.CellType == ECellTypes.Moneybox) {
            CheckMoneyboxAfterMove(player);
        }

        if (cell.CellType == ECellTypes.Lightning) {
            player.Effects.ExecuteLightning();
        }

        if (cell.CoinBonusValue != 0) {
            player.Effects.ExecuteCoinBonus(cell.CoinBonusValue);
        }

        if (cell.Effect == EControllableEffects.Black) {
            player.Effects.ExecuteBlack();
            return;
        }

        if (cell.Effect == EControllableEffects.Red) {
            player.Effects.ExecuteRed();
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
            if (player.IsMe()) {
                _modalMoneybox.BuildContent(player);
                _modalMoneybox.OpenModal();
            }

            if (player.IsAi()) {
                AiControl.Instance.AiMoneybox(player, vault);
            }
            
            return false;
        }

        return true;
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
            vault.PutPlayerToVault(player);
        }
    }
}

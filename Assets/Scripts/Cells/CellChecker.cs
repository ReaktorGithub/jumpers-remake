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

    private void Awake() {
        Instance = this;
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _popupAttack = GameObject.Find("GameScripts").GetComponent<PopupAttack>();
        _modalReplace = GameObject.Find("GameScripts").GetComponent<ModalReplaceEffect>();
    }

    public bool CheckBranch(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();

        if (cell.TryGetComponent(out BranchCell branchCell)) {
            BranchControl branch = branchCell.BranchControl;
            if (branch.IsReverse != player.IsReverseMove) {
                // Если направление движения фишки не соответствует направлению бранча, то скипаем
                return true;
            }
            
            ActivateBranch(player, branch, player.StepsLeft);
            return false;
        }

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
            string cubicStatus = Utils.Wrap("Остаток: " + rest, UIColors.Green);
            CubicControl.Instance.WriteStatus(cubicStatus);
        }

        if (player.IsAi()) {
            AiControl.Instance.AiSelectBranch(player, branch, rest);
        }
    }

    public bool CheckCellAfterStep(ECellTypes cellType, PlayerControl player) {
        if (cellType == ECellTypes.Checkpoint) {
            string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " достигает " + Utils.Wrap("чекпойнта", UIColors.Blue);
            Messages.Instance.AddMessage(message);
        }

        if (cellType == ECellTypes.Finish) {
            player.ExecuteFinish();
            _camera.ClearFollow();
            return false;
        }

        if (cellType == ECellTypes.Start && player.IsReverseMove) {
            _camera.ClearFollow();
            return false;
        }

        return true;
    }

    // Проверка, может ли игрок избежать вредного эффекта

    public void CheckCellCharacter(PlayerControl player) {
        CellControl cell = player.GetCurrentCell();
        
        if (cell.IsNegativeEffect() && player.IsEnoughEffects(cell.Effect)) {
            if (player.IsAi()) {
                // todo научить принимать решения
                CheckCellEffects(player);
                return;
            }
            EffectsControl.Instance.SelectedEffect = cell.Effect;
            _modalReplace.BuildContent(player);
            _modalReplace.OpenWindow();
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
            player.ExecuteStarEffect();
        }

        if (cell.Effect == EControllableEffects.Black) {
            player.ExecuteBlackEffect();
            return;
        }

        if (cell.Effect == EControllableEffects.Red) {
            player.MovesToDo = 0;
            player.StepsLeft = 0;
            player.ExecuteRedEffect();
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
            List<PlayerControl> rivals = new();
            List<PlayerControl> rivalsWithShields = new();

            foreach(PlayerControl playerForCheck in PlayersControl.Instance.Players) {
                if (tokens.Contains(playerForCheck.TokenObject) && player.TokenObject != playerForCheck.TokenObject) {
                    if (playerForCheck.Armor > 0) {
                        rivalsWithShields.Add(playerForCheck);
                    } else {
                        rivals.Add(playerForCheck);
                    }
                }
            }

            if (rivalsWithShields.Count > 0) {
                player.HarvestShieldBonus(rivalsWithShields);
            }

            if (rivals.Count > 0) {
                if (player.IsAi()) {
                    AiControl.Instance.AiAttackPlayer(player, rivals);
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
}

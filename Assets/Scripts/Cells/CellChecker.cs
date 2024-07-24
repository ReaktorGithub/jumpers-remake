using System.Collections.Generic;
using UnityEngine;

public class CellChecker : MonoBehaviour
{
    public static CellChecker Instance { get; private set; }
    private TopPanel _topPanel;
    private CubicControl _cubicControl;
    private CameraControl _camera;
    private PopupAttack _popupAttack;
    [SerializeField] private List<ECellTypes> _skipRivalCheckTypes = new();

    private void Awake() {
        Instance = this;
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _cubicControl = GameObject.Find("Cubic").GetComponent<CubicControl>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _popupAttack = GameObject.Find("GameScripts").GetComponent<PopupAttack>();
    }

    public bool CheckBranch(CellControl currentCellControl, int stepsLeft) {
        if (currentCellControl.TryGetComponent(out BranchCell branchCell)) {
            BranchControl branch = branchCell.BranchObject.GetComponent<BranchControl>();
            branch.ShowAllBranches();
            _topPanel.OpenWindow();
            string message = Utils.Wrap("Остаток: " + stepsLeft, UIColors.Green);
            _cubicControl.WriteStatus(message);
            return false;
        }

        return true;
    }

    public bool CheckCellAfterStep(ECellTypes cellType, PlayerControl currentPlayer) {
        if (cellType == ECellTypes.Checkpoint) {
            string message = Utils.Wrap(currentPlayer.PlayerName, UIColors.Yellow) + " достигает " + Utils.Wrap("чекпойнта", UIColors.Blue);
            Messages.Instance.AddMessage(message);
        }

        if (cellType == ECellTypes.Finish) {
            currentPlayer.ExecuteFinish();
            _camera.ClearFollow();
            return false;
        }

        return true;
    }

    // Необходимые действия перед завершением хода:
    // 1.	Подбор бонуса.
    // 2.	Срабатывание капкана.
    // 3.	Исполнение эффекта.
    // 4.	Исполнение эффекта «стрелка», либо «синяя стрелка».
    // 5.	Атака на соперников.

    public void CheckCellEffects(CellControl cellControl, PlayerControl currentPlayer, TokenControl tokenControl, int currentPlayerIndex) {
        if (cellControl.Effect == EControllableEffects.Green) {
            MoveControl.Instance.AddMovesLeft(1);
            string message = Utils.Wrap(currentPlayer.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("зелёный", UIColors.Green) + " эффект и ходит ещё раз";
            Messages.Instance.AddMessage(message);
        }

        if (cellControl.Effect == EControllableEffects.Yellow) {
            currentPlayer.SkipMoveIncrease(tokenControl);
            string message = Utils.Wrap(currentPlayer.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("жёлтый", UIColors.Yellow) + " эффект и пропустит ход";
            Messages.Instance.AddMessage(message);
        }

        if (cellControl.Effect == EControllableEffects.Black) {
            currentPlayer.ExecuteBlackEffect(cellControl, tokenControl, currentPlayerIndex);
            return;
        }

        if (cellControl.transform.TryGetComponent(out RedCell _)) {
            MoveControl.Instance.MovesLeft = 0;
            MoveControl.Instance.StepsLeft = 0;
            currentPlayer.ExecuteRedEffect(currentPlayerIndex);
            return;
        }

        CheckCellArrows(cellControl, currentPlayer, tokenControl);
    }

    public void CheckCellArrows(CellControl cellControl, PlayerControl currentPlayer, TokenControl tokenControl) {
        if (cellControl.transform.TryGetComponent(out ArrowCell arrowCell)) {
            tokenControl.PutTokenToArrowSpline(arrowCell.ArrowSpline);
            return;
        }

        CheckCellRivals(cellControl, currentPlayer);
    }

    public void CheckCellRivals(CellControl cellControl, PlayerControl currentPlayer) {
        if (_skipRivalCheckTypes.Contains(cellControl.CellType)) {
            StartCoroutine(MoveControl.Instance.EndMoveDefer());
            return;
        }
        
        List<string> tokens = cellControl.CurrentTokens;

        if (tokens.Count > 1) {
            List<PlayerControl> rivals = new();
            foreach(PlayerControl player in PlayersControl.Instance.Players) {
                if (tokens.Contains(player.TokenName) && currentPlayer.TokenName != player.TokenName) {
                    rivals.Add(player);
                }
            }
            
            _popupAttack.BuildContent(currentPlayer, rivals);
            _popupAttack.OpenWindow();

            return;
        }

        // закончить ход, если нет соперников

        StartCoroutine(MoveControl.Instance.EndMoveDefer());
    }
}

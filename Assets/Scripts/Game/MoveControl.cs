using System.Collections;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    public static MoveControl Instance { get; private set; }
    private int _currentPlayerIndex = 1;
    private int _stepsLeft;
    private int _movesLeft = 1;
    [SerializeField] private float stepDelay = 0.3f;
    [SerializeField] private float endMoveDelay = 0.5f;
    [SerializeField] private float skipMoveDelay = 1.5f;
    [SerializeField] private float alignTime = 5f;
    private TokenControl _currentToken;
    private PlayerControl _currentPlayer;
    private CellControl _currentCell;
    private CubicControl _cubicControl;
    private ModalResults _modalResults;
    private ModalWin _modalWin;
    private ModalLose _modalLose;
    private CameraControl _camera;
    private TopPanel _topPanel;

    private void Awake() {
        Instance = this;
        _cubicControl = GameObject.Find("Cubic").GetComponent<CubicControl>();
        _modalResults = GameObject.Find("ModalResults").GetComponent<ModalResults>();
        _modalWin = GameObject.Find("GameScripts").GetComponent<ModalWin>();
        _modalLose = GameObject.Find("GameScripts").GetComponent<ModalLose>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
    }

    public PlayerControl CurrentPlayer {
        get { return _currentPlayer; }
        private set {}
    }

    public int MovesLeft {
        get { return _movesLeft; }
        set {}
    }

    public void AddMovesLeft(int count) {
        _movesLeft += count;
    }

    public int StepsLeft {
        get { return _stepsLeft; }
        set {}
    }

    public void SetNextPlayerIndex() {
        if (_currentPlayerIndex < 4) {
            _currentPlayerIndex += 1;
        } else {
            _currentPlayerIndex = 1;
        }
        SwitchPlayer();
    }

    public int CurrentPlayerIndex {
        get {
            return _currentPlayerIndex;
        }
        set {
            if (value >= 1 && value <= 4) {
                _currentPlayerIndex = value;
            } else {
                Debug.Log("Error while set current player " + value);
            }
        }
    }

    public bool IsRaceOver() {
        int count = 0;
        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            if (!player.IsFinished) {
                count++;
            }
        }
        return count < 2;
    }

    // сохраняем нового игрока как текущего
    // если игрок финишировал, то меняем игрока и прерываем цикл
    // ОСТОРОЖНО! рекурсия
    
    public void SwitchPlayer() {
        PlayerControl[] playerControls = PlayersControl.Instance.Players;

        for (int i = 0; i < playerControls.Length; i++) {
            if (playerControls[i].MoveOrder == _currentPlayerIndex) {
                if (playerControls[i].IsFinished) {
                    SetNextPlayerIndex();
                    break;
                } else {
                    _currentPlayer = playerControls[i];
                    _currentToken = playerControls[i].GetTokenControl();
                    PlayersControl.Instance.UpdateTokenLayerOrder(_currentPlayerIndex);
                    PlayersControl.Instance.UpdateSqueezeAnimation(_currentPlayerIndex);
                    PlayersControl.Instance.UpdatePlayersInfo(_currentPlayerIndex);
                    EffectsControl.Instance.UpdateQuantityText(_currentPlayer);
                    EffectsControl.Instance.UpdateEffectEmptiness(_currentPlayer);
                    if (_currentPlayer.MovesSkip == 0) {
                        _movesLeft = 1;
                        string message = Utils.Wrap(_currentPlayer.PlayerName, UIColors.Yellow) + " ходит";
                        string cubicMessage = Utils.Wrap("ваш ход!", UIColors.Green);
                        PreparePlayerForMove(cubicMessage, message);
                    } else {
                        StartCoroutine(SkipMoveDefer());
                    }
                }
            }
        }
    }

    private void PreparePlayerForMove(string cubicMessage, string messengerMessage = null) {
        _cubicControl.SetCubicInteractable(true);
        _cubicControl.WriteStatus(cubicMessage);
        if (messengerMessage != null) {
            Messages.Instance.AddMessage(messengerMessage);
        }
        _camera.FollowObject(_currentToken.gameObject.transform);
        EffectsControl.Instance.SetDisabledEffectButtons(false);
    }

    private void StartCellCheckBeforeStep() {
        bool check = CellChecker.Instance.CheckBranch(_currentCell, _stepsLeft, _currentPlayer.IsReverseMove);
        if (check) {
            StartCoroutine(MakeStepDefer());
        }
    }

    public IEnumerator MakeStepDefer() {
        yield return new WaitForSeconds(stepDelay);
        MakeStep();
    }

    private void MakeStep() {
        _stepsLeft--;
        _currentToken.SetToNextCell(() => {
            // проверяем тип клетки, на которой сейчас находимся
            // некоторые типы прерывают движение, либо вызывают код во время движения

            _currentCell = _currentToken.GetCurrentCellControl();

            bool check = CellChecker.Instance.CheckCellAfterStep(_currentCell.CellType, _currentPlayer);
            if (!check) {
                return;
            }

            // если тип клетки не прерывает движение, то проверяем условие выхода из цикла шагов
            if (_stepsLeft > 0) {
                StartCellCheckBeforeStep();
            } else {
                StartCoroutine(ConfirmNewPositionDefer());
            }
        });
    }

    public void MakeMove(int score) {
        _stepsLeft = score;
        _currentCell = _currentToken.GetCurrentCellControl();
        _currentCell.RemoveToken(_currentPlayer.TokenName);
        _currentCell.AlignTokens(alignTime);
        StartCellCheckBeforeStep();
    }

    private IEnumerator ConfirmNewPositionDefer() {
        yield return new WaitForSeconds(stepDelay);
        ConfirmNewPosition();
    }

    // подтверждение новой позиции по окончании движения

    public void ConfirmNewPosition() {
        _currentCell = _currentToken.GetCurrentCellControl();
        _currentCell.AddToken(_currentPlayer.TokenName);
        _currentCell.AlignTokens(alignTime, () => {
            CellChecker.Instance.CheckCellEffects(_currentCell, _currentPlayer, _currentToken, _currentPlayerIndex);
        });
    }

    // смена направления

    public void SwitchBranch(GameObject nextCell) {
        if (!_currentCell.TryGetComponent(out BranchCell branchCell)) {
            Debug.Log("Error while switching branch");
            return;
        }
        
        BranchControl branch = branchCell.BranchObject.GetComponent<BranchControl>();
        branch.HideAllBranches();
        _topPanel.CloseWindow();
        if (CurrentPlayer.IsReverseMove) {
            _currentCell.PreviousCell = nextCell;
        } else {
            _currentCell.NextCell = nextCell;
        }
        _cubicControl.WriteStatus("");
        StartCoroutine(MakeStepDefer());
    }

    public IEnumerator EndMoveDefer() {
        yield return new WaitForSeconds(endMoveDelay);
        EndMove();
    }

    public IEnumerator SkipMoveDefer() {
        string message = Utils.Wrap(_currentPlayer.PlayerName, UIColors.Yellow) + " пропускает ход";
        Messages.Instance.AddMessage(message);
        message = Utils.Wrap("пропуск", UIColors.Yellow);
        _cubicControl.WriteStatus(message);
        yield return new WaitForSeconds(skipMoveDelay);
        _currentPlayer.SkipMoveDecrease(_currentToken);
        EndMove();
    }

    public void EndMove() {
        // проверка на окончание гонки

        bool isRaceOver = IsRaceOver();

        if (isRaceOver) {
            PlayersControl.Instance.UpdatePlayersInfo(_currentPlayerIndex);
            PlayersControl.Instance.MoveAllTokensToPedestal(_currentPlayerIndex, endMoveDelay);
            StartCoroutine(RaceOverDefer());
            return;
        }

        // текущий игрок мог финишировать во время хода - проверить

        if (_currentPlayer.IsFinished) {
            SetNextPlayerIndex();
            return;
        }

        // проверка на бонусные ходы

        _movesLeft--;

        if (_movesLeft <= 0) {
            SetNextPlayerIndex();
            return;
        }

        // проверка на пропуск хода

        if (_currentPlayer.MovesSkip > 0) {
            StartCoroutine(SkipMoveDefer());
            return;
        }

        // текущий игрок продолжает ходить

        string cubicMessage = Utils.Wrap("бонусный ход!", UIColors.Green);
        PreparePlayerForMove(cubicMessage);

        // _cellControl.ShowTokensAtCells();
    }

    public IEnumerator RaceOverDefer() {
        yield return new WaitForSeconds(endMoveDelay);
        RaceOver();
    }

    public void RaceOver() {
        PlayersControl.Instance.GiveResourcesAfterRace();
        CloseAllOptionalModals();
        _modalResults.OpenWindow(PlayersControl.Instance.Players);
    }

    public void CloseAllOptionalModals() {
        if (_modalLose.gameObject.activeInHierarchy) {
            _modalLose.CloseWindow();
        }

        if (_modalWin.gameObject.activeInHierarchy) {
            _modalWin.CloseWindow();
        }
    }

    // debug

    public void DebugStatus() {
        Debug.Log("_currentPlayerIndex " + _currentPlayerIndex);
        Debug.Log("_stepsLeft " + _stepsLeft);
        Debug.Log("_movesLeft " + _movesLeft);
        Debug.Log("CurrentCell " + _currentToken.CurrentCell);
        Debug.Log("_currentPlayer " + _currentPlayer.PlayerName);
        Debug.Log("Cerrent cell control name " + _currentCell.transform.name);
    }
}

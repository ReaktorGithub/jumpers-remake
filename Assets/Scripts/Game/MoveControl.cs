using System.Collections;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    public static MoveControl Instance { get; private set; }
    private int _currentPlayerIndex = 1;
    [SerializeField] private float _moveTime = 12f;
    [SerializeField] private float _specifiedMoveTime = 6f;
    [SerializeField] private float _stepDelay = 0.3f;
    [SerializeField] private float _endMoveDelay = 0.5f;
    [SerializeField] private float _skipMoveDelay = 1.5f;
    [SerializeField] private float _alignTime = 5f;
    private TokenControl _currentToken;
    private PlayerControl _currentPlayer;
    private CellControl _currentCell;
    private ModalResults _modalResults;
    private ModalWin _modalWin;
    private ModalLose _modalLose;
    private CameraControl _camera;
    private TopPanel _topPanel;
    private ModifiersControl _modifiersControl;
    private bool _isLassoMode = false;

    private void Awake() {
        Instance = this;
        _modalResults = GameObject.Find("ModalResults").GetComponent<ModalResults>();
        _modalWin = GameObject.Find("GameScripts").GetComponent<ModalWin>();
        _modalLose = GameObject.Find("GameScripts").GetComponent<ModalLose>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _modifiersControl = GameObject.Find("Modifiers").GetComponent<ModifiersControl>();
    }

    public PlayerControl CurrentPlayer {
        get { return _currentPlayer; }
        private set {}
    }

    public CellControl CurrentCell {
        get { return _currentCell; }
        private set {}
    }

    public float MoveTime {
        get { return _moveTime; }
        private set {}
    }

    public float SpecifiedMoveTime {
        get { return _specifiedMoveTime; }
        private set {}
    }

    public bool IsLassoMode {
        get { return _isLassoMode; }
        private set {}
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
                    SetNextPlayerIndex(); // ОСТОРОЖНО! рекурсия
                    break;
                } else {
                    // инициация текущего игрока
                    _currentPlayer = playerControls[i];
                    _currentToken = playerControls[i].GetTokenControl();
                    _currentCell = _currentToken.GetCurrentCellControl();

                    // апдейт состояния фишек
                    PlayersControl.Instance.UpdateTokenLayerOrder(_currentPlayerIndex);
                    PlayersControl.Instance.UpdateSqueezeAnimation(_currentPlayerIndex);

                    // применение сайд-эффектов и продолжение переключения игрока
                    bool check = ExecuteSideEffects();
                    if (check) {
                        ContinueSwitchPlayer();
                    }
                }
            }
        }
    }

    // Активация разных эффектов после смены игрока (трата щитов, взрыв бумки и.т.д)
    // Если требует прерывания, то возвращает false

    private bool ExecuteSideEffects() {
        _currentPlayer.SpendArmor();
        return true;
    }

    // Вторая часть метода по переключению игроков

    private void ContinueSwitchPlayer() {
        // апдейт камеры
        _camera.FollowObject(_currentToken.gameObject.transform);

        // апдейт панели управления
        PlayersControl.Instance.UpdatePlayersInfo(_currentPlayerIndex);
        EffectsControl.Instance.UpdateQuantityText(_currentPlayer);
        EffectsControl.Instance.UpdateEffectEmptiness(_currentPlayer);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_currentPlayer);

        // проверка текущего игрока на пропуск хода
        if (_currentPlayer.MovesSkip == 0) {
            _currentPlayer.MovesToDo = 1;
            string cubicMessage = Utils.Wrap("ваш ход!", UIColors.Green);
            PreparePlayerForMove(cubicMessage);
        } else {
            StartCoroutine(SkipMoveDefer());
        }
    }

    private void PreparePlayerForMove(string cubicMessage = null) {
        CubicControl.Instance.SetCubicInteractable(true);
        if (cubicMessage != null) {
            CubicControl.Instance.WriteStatus(cubicMessage);
        }
        _currentPlayer.IsEffectPlaced = false;
        EffectsControl.Instance.DisableAllButtons(false);
        BoostersControl.Instance.EnableAllButtons();
    }

    private void StartCellCheckBeforeStep() {
        bool check = CellChecker.Instance.CheckBranch(_currentCell, _currentPlayer);
        if (check) {
            StartCoroutine(MakeStepDefer());
        }
    }

    public IEnumerator MakeStepDefer() {
        yield return new WaitForSeconds(_stepDelay);
        MakeStep();
    }

    private void MakeStep() {
        _currentPlayer.AddStepsLeft(-1);
        _currentToken.SetToNextCell(_moveTime, AfterStep);
    }

    private void AfterStep() {
        // проверяем тип клетки, на которой сейчас находимся
        // некоторые типы прерывают движение, либо вызывают код во время движения

        _currentCell = _currentToken.GetCurrentCellControl();

        bool check = CellChecker.Instance.CheckCellAfterStep(_currentCell.CellType, _currentPlayer);
        if (!check) {
            return;
        }

        // если тип клетки не прерывает движение, то проверяем условие выхода из цикла шагов
        if (_currentPlayer.StepsLeft > 0) {
            StartCellCheckBeforeStep();
        } else {
            StartCoroutine(ConfirmNewPositionDefer());
        }
    }

    public void MakeMove(int score) {
        _isLassoMode = false;
        _currentPlayer.StepsLeft = score;
        _currentCell = _currentToken.GetCurrentCellControl();
        _currentCell.RemoveToken(_currentPlayer.TokenObject);
        _currentCell.AlignTokens(_alignTime);
        StartCellCheckBeforeStep();
    }

    public void MakeLassoMove(CellControl targetCell) {
        _isLassoMode = true;
        _currentPlayer.StepsLeft = 0;
        _currentPlayer.AddMovesToDo(1);
        _currentCell = _currentToken.GetCurrentCellControl();
        _currentCell.RemoveToken(_currentPlayer.TokenObject);
        _currentCell.AlignTokens(_alignTime);
        _currentToken.SetToSpecifiedCell(targetCell.gameObject, _specifiedMoveTime, AfterStep);
    }

    private IEnumerator ConfirmNewPositionDefer() {
        yield return new WaitForSeconds(_stepDelay);
        ConfirmNewPosition();
    }

    // подтверждение новой позиции по окончании движения

    public void ConfirmNewPosition() {
        _currentCell = _currentToken.GetCurrentCellControl();
        _currentCell.AddToken(_currentPlayer.TokenObject);
        _currentCell.AlignTokens(_alignTime, () => {
            CellChecker.Instance.CheckCellCharacter(_currentCell, _currentPlayer, _currentToken, _currentPlayerIndex);
        });
    }

    public void CheckCellEffects() {
        CellChecker.Instance.CheckCellEffects(_currentCell, _currentPlayer, _currentToken, _currentPlayerIndex);
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
        CubicControl.Instance.WriteStatus("");
        StartCoroutine(MakeStepDefer());
    }

    public IEnumerator EndMoveDefer() {
        yield return new WaitForSeconds(_endMoveDelay);
        EndMove();
    }

    public IEnumerator SkipMoveDefer() {
        string message = Utils.Wrap(_currentPlayer.PlayerName, UIColors.Yellow) + " пропускает ход";
        Messages.Instance.AddMessage(message);
        message = Utils.Wrap("пропуск", UIColors.Yellow);
        CubicControl.Instance.WriteStatus(message);
        yield return new WaitForSeconds(_skipMoveDelay);
        _currentPlayer.SkipMoveDecrease(_currentToken);
        EndMove();
    }

    public void EndMove() {
        // CellsControl.Instance.ShowTokensAtCells();

        // сброс параметров
        _modifiersControl.HideModifierMagnet();
        CellsControl.Instance.ResetCellMagnetHint();

        // проверка на окончание гонки

        bool isRaceOver = IsRaceOver();

        if (isRaceOver) {
            PlayersControl.Instance.UpdatePlayersInfo(_currentPlayerIndex);
            PlayersControl.Instance.MoveAllTokensToPedestal(_currentPlayerIndex, _endMoveDelay);
            StartCoroutine(RaceOverDefer());
            return;
        }

        // текущий игрок мог финишировать во время хода - проверить

        if (_currentPlayer.IsFinished) {
            SetNextPlayerIndex();
            return;
        }

        // проверка на бонусные ходы

        _currentPlayer.AddMovesToDo(-1);

        if (_currentPlayer.MovesToDo <= 0) {
            SetNextPlayerIndex();
            return;
        }

        // проверка на пропуск хода

        if (_currentPlayer.MovesSkip > 0) {
            StartCoroutine(SkipMoveDefer());
            return;
        }

        // текущий игрок продолжает ходить

        string cubicMessage = Utils.Wrap(_isLassoMode ? "ваш ход!" : "бонусный ход!", UIColors.Green);
        PreparePlayerForMove(cubicMessage);
    }

    public IEnumerator RaceOverDefer() {
        yield return new WaitForSeconds(_endMoveDelay);
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
        Debug.Log("_stepsLeft " + _currentPlayer.StepsLeft);
        Debug.Log("_movesToDo " + _currentPlayer.MovesToDo);
        Debug.Log("CurrentCell " + _currentToken.CurrentCell);
        Debug.Log("_currentPlayer " + _currentPlayer.PlayerName);
        Debug.Log("Cerrent cell control name " + _currentCell.transform.name);
    }
}

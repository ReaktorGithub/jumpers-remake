using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class MoveControl : MonoBehaviour
{
    public static MoveControl Instance { get; private set; }
    [SerializeField] private int _currentPlayerIndex = 1;
    [SerializeField] private float _moveTime = 12f;
    [SerializeField] private float _specifiedMoveTime = 6f;
    [SerializeField] private float _stepDelay = 0.3f;
    [SerializeField] private float _endMoveDelay = 0.5f;
    [SerializeField] private float _skipMoveDelay = 1.5f;
    [SerializeField] private float _alignTime = 5f;
    private PlayerControl _currentPlayer;
    private ModalResults _modalResults;
    private ModalWin _modalWin;
    private ModalLose _modalLose;
    private CameraControl _camera;
    private TopPanel _topPanel;
    private HedgehogsControl _hedgehogsControl;
    private bool _isLassoMode = false;
    private bool _isViolateMode = false; // в этом режиме текущий игрок является жертвой волшебного пинка или пылесоса
    private int _restSteps = 0; // сохранение оставшихся шагов в режиме жертвы

    private void Awake() {
        Instance = this;
        _modalResults = GameObject.Find("ModalResults").GetComponent<ModalResults>();
        _modalWin = GameObject.Find("GameScripts").GetComponent<ModalWin>();
        _modalLose = GameObject.Find("GameScripts").GetComponent<ModalLose>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _hedgehogsControl = GameObject.Find("LevelScripts").GetComponent<HedgehogsControl>();
    }

    public PlayerControl CurrentPlayer {
        get { return _currentPlayer; }
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

    public bool IsViolateMode {
        get { return _isViolateMode; }
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

    private bool IsRaceOver() {
        int count = 0;
        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            if (!player.IsFinished) {
                count++;
            }
        }
        return count < 2;
    }

    private void FollowCameraToCurrentPlayer() {
        _camera.FollowObject(_currentPlayer.GetTokenControl().gameObject.transform);
    }

    // сохраняем нового игрока как текущего
    // если игрок финишировал, то меняем игрока и прерываем цикл
    // ОСТОРОЖНО! рекурсия
    
    public void SwitchPlayer() {
        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            if (player.MoveOrder != _currentPlayerIndex) {
                continue;
            }

            if (player.IsFinished) {
                SetNextPlayerIndex(); // ОСТОРОЖНО! рекурсия
                break;
            } else {
                // сохранение ссылки на текущего игрока
                _currentPlayer = player;

                // апдейт состояния фишек
                PlayersControl.Instance.UpdateTokenLayerOrder(_currentPlayerIndex);
                PlayersControl.Instance.UpdateSqueezeAnimation();

                // применение сайд-эффектов и продолжение переключения игрока
                bool check = ExecuteSideEffects();
                if (check) {
                    ContinueSwitchPlayer();
                }
            }
        }
    }

    // Активация разных эффектов после смены игрока (трата щитов, взрыв бумки и.т.д)
    // Если требует прерывания, то возвращает false

    private bool ExecuteSideEffects() {
        PlayersControl.Instance.SpendPlayersArmor();
        BranchHedgehog branch = _hedgehogsControl.FindCompletedHedgehogBranch();
        if (branch != null) {
            _hedgehogsControl.MoveHedgehog(branch);
        }
        return true;
    }

    // Вторая часть метода по переключению игроков

    private void ContinueSwitchPlayer() {
        // апдейт камеры
        FollowCameraToCurrentPlayer();

        // апдейт панели управления
        PlayersControl.Instance.UpdatePlayersInfo();
        if (_currentPlayer.IsMe()) {
            EffectsControl.Instance.UpdateQuantityText(_currentPlayer);
            EffectsControl.Instance.UpdateEffectEmptiness(_currentPlayer);
            BoostersControl.Instance.UpdateBoostersFromPlayer(_currentPlayer);
        }

        // проверка на копилку
        bool check = CellChecker.Instance.CheckMoneyboxBeforeMove(_currentPlayer);
        if (!check) {
            return;
        }

        CheckMoveSkipAndPreparePlayer();
    }

    public void CheckMoveSkipAndPreparePlayer() {
        if (_currentPlayer.MovesSkip == 0) {
            _currentPlayer.AddMovesToDo(1);
            PreparePlayerForMove();
        } else {
            StartCoroutine(SkipMoveDefer());
        }
    }

    public void PreparePlayerForMove() {
        string cubicMessage;
        switch (_currentPlayer.Type) {
            case EPlayerTypes.Me: {
                cubicMessage = Utils.Wrap(_isLassoMode ? "ваш ход!" : "бонусный ход!", UIColors.Green);
                break;
            }
            case EPlayerTypes.Ai: {
                cubicMessage = Utils.Wrap("ход компьютера", UIColors.Grey);
                break;
            }
            case EPlayerTypes.Web: {
                cubicMessage = Utils.Wrap("ход соперника", UIColors.Grey);
                break;
            }
            default: {
                cubicMessage = Utils.Wrap("ход черепа", UIColors.Red);
                break;
            }
        }
        CubicControl.Instance.WriteStatus(cubicMessage);

        // возможность применять эффекты должна восстанавливаться только если это новый ход, а не возвращение хода после лассо / пылесоса и т.д
        bool isNewMove = !_isLassoMode;
        if (isNewMove) {
           _currentPlayer.Effects.IsEffectPlaced = false;
        }

        // включение индикатора молнии на кубике
        _currentPlayer.Effects.CheckLightningStartMove();
        
        bool isMe = _currentPlayer.IsMe();
        if (isMe) {
            EffectsControl.Instance.TryToEnableAllEffectButtons();
            CubicControl.Instance.SetCubicInteractable(true);
        } else if (_currentPlayer.Type == EPlayerTypes.Ai) {
            StartCoroutine(AiControl.Instance.AiThrowCubic());
        }
        BoostersControl.Instance.EnableAllButtons(!isMe);

        // CellsControl.Instance.ShowTokensAtCells();
    }

    private void StartCellCheckBeforeStep() {
        bool check = CellChecker.Instance.CheckBranch(_currentPlayer);
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
        _currentPlayer.GetTokenControl().SetToNextCell(_moveTime, AfterStep);
    }

    private void AfterStep() {
        // проверяем тип клетки, на которой сейчас находимся
        // некоторые типы прерывают движение, либо вызывают код во время движения

        bool check = CellChecker.Instance.CheckCellAfterStep(_currentPlayer.GetCurrentCell(), _currentPlayer);
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
        _currentPlayer.Effects.SpendLightning();
        CellControl cell = _currentPlayer.GetCurrentCell();
        cell.RemoveToken(_currentPlayer.TokenObject);
        cell.AlignTokens(_alignTime);
        StartCellCheckBeforeStep();
    }

    public void MakeLassoMove(CellControl targetCell) {
        _isLassoMode = true;
        _currentPlayer.StepsLeft = 0;
        _currentPlayer.AddMovesToDo(1);
        CellControl cell = _currentPlayer.GetCurrentCell();
        cell.RemoveToken(_currentPlayer.TokenObject);
        cell.AlignTokens(_alignTime);
        _currentPlayer.GetTokenControl().SetToSpecifiedCell(targetCell.gameObject, _specifiedMoveTime, AfterStep);
    }

    /*
        1. Переместить соперника на новую клетку
        2. После анимации совершить анимацию выравнивания фишек
        3. После анимации выравнивания назначить жертву текущим игроком и повысить слой фишки
        4. Включить режим насилия и выполнить условия на клетке
        5. Назначить текущего игрока в соответствии с currentPlayerIndex, повысить слой его фишки и передать ему ход

        Если в процессе движения попался бранч, то алгоритм будет другой

        2. Совершить анимацию выравнивания только у текущего игрока
        3. После анимации выравнивания назначить жертву текущим игроком и повысить слой фишки
        4. Дать игроку выбрать направление
        5. Повторять шаги 1-4, пока смещение не будет завершено
        6. Продолжаем с п.4 предыдущего сценария
    */

    public void MakeMagicKickMove(PlayerControl victim, CellControl currentCell, int steps) {
        (GameObject, int) cellResult = CellsControl.Instance.FindCellBySteps(currentCell.gameObject, false, steps);
        Debug.Log("cellResult: " + cellResult.Item1 + ", rest steps: " + cellResult.Item2);

        if (cellResult.Item1 == null) {
            Debug.Log("Error while magic kick attack");
            return;
        }

        CellControl targetCell = cellResult.Item1.GetComponent<CellControl>();
        _restSteps = cellResult.Item2;
        TokenControl victimToken = victim.GetTokenControl();
        
        victimToken.SetToSpecifiedCell(targetCell.gameObject, _specifiedMoveTime, () => {
            currentCell.RemoveToken(victim.TokenObject);
            currentCell.AlignTokens(_alignTime);

            // жертва назначается текущим игроком
            _currentPlayer = victim;
            PlayersControl.Instance.UpdateTokenLayerOrder(_currentPlayer.MoveOrder);
            FollowCameraToCurrentPlayer();
            _isViolateMode = true;

            bool isBranch = targetCell.TryGetComponent(out BranchCell branchCell);
            if (isBranch && _restSteps > 0) {
                CellChecker.Instance.ActivateBranch(_currentPlayer, branchCell.BranchControl, _restSteps);
            } else {
                ConfirmNewPosition();
            }
        });
    }

    public void BreakMovingAndConfirmNewPosition() {
        _currentPlayer.StepsLeft = 0;
        StartCoroutine(ConfirmNewPositionDefer());
    }

    private IEnumerator ConfirmNewPositionDefer() {
        yield return new WaitForSeconds(_stepDelay);
        ConfirmNewPosition();
    }

    // подтверждение новой позиции по окончании движения

    public void ConfirmNewPosition() {
        CellControl cell = _currentPlayer.GetCurrentCell();
        cell.AddToken(_currentPlayer.TokenObject);
        cell.AlignTokens(_alignTime, () => {
            CellChecker.Instance.CheckCellAfterMove(_currentPlayer);
        });
    }

    // Совершить действия после удаления проигравшей фишки на педестал

    public void ConfirmLosePlayer(PlayerControl player) {
        CellControl cellControl = player.GetCurrentCell();
        cellControl.RemoveToken(player.TokenObject);
        cellControl.AlignTokens(_alignTime);
        StartCoroutine(EndMoveDefer());
    }

    public void CheckCellEffects() {
        CellChecker.Instance.CheckCellEffects(_currentPlayer);
    }

    // смена направления

    public void SwitchBranch(GameObject nextCell) {
        CellControl cell = _currentPlayer.GetCurrentCell();

        if (!cell.TryGetComponent(out BranchCell branchCell)) {
            Debug.Log("Error while switching branch");
            return;
        }
        
        branchCell.BranchControl.HideAllBranches();
        _topPanel.CloseWindow();
        if (CurrentPlayer.IsReverseMove) {
            cell.PreviousCell = nextCell;
        } else {
            cell.NextCell = nextCell;
        }
        CubicControl.Instance.WriteStatus("");
        StartCoroutine(MakeStepDefer());
    }

    public void SwitchBranchViolate(GameObject nextCell) {
        CellControl cell = _currentPlayer.GetCurrentCell();

        if (!cell.TryGetComponent(out BranchCell branchCell)) {
            Debug.Log("Error while switching branch");
            return;
        }
        
        branchCell.BranchControl.HideAllBranches();
        _topPanel.CloseWindow();
        CubicControl.Instance.WriteStatus("");

        CellControl nextCellControl = nextCell.GetComponent<CellControl>();
        MakeMagicKickMove(_currentPlayer, nextCellControl, _restSteps - 1);
    }

    public void SwitchBranchHedgehog(GameObject nextCell, SplineContainer nextArrowSpline) {
        CellControl cell = _currentPlayer.GetCurrentCell();
        

        if (!cell.TryGetComponent(out BranchCell branchCell)) {
            Debug.Log("Error while switching branch");
            return;
        }
        
        branchCell.BranchControl.HideAllBranches();
        _topPanel.CloseWindow();
        
        TokenControl token = _currentPlayer.GetTokenControl();
        cell.RemoveToken(token.gameObject);
        token.ExecuteArrowMove(nextArrowSpline, nextCell);
    }

    // Конец хода

    public IEnumerator EndMoveDefer() {
        yield return new WaitForSeconds(_endMoveDelay);
        if (_isViolateMode) {
            _currentPlayer = PlayersControl.Instance.GetPlayer(_currentPlayerIndex);
            PlayersControl.Instance.UpdateTokenLayerOrder(_currentPlayerIndex);
            _isViolateMode = false;
        }
        EndMove();
    }

    public IEnumerator SkipMoveDefer() {
        string message = Utils.Wrap(_currentPlayer.PlayerName, UIColors.Yellow) + " пропускает ход";
        Messages.Instance.AddMessage(message);
        message = Utils.Wrap("пропуск", UIColors.Yellow);
        CubicControl.Instance.WriteStatus(message);
        yield return new WaitForSeconds(_skipMoveDelay);
        _currentPlayer.SkipMoveDecrease();
        _currentPlayer.Effects.SpendLightning();
        EndMove();
    }

    public void EndMove() {
        // CellsControl.Instance.ShowTokensAtCells();

        // Сброс параметров

        ResetParamsAfterMove();

        // Проверка молнии

        _currentPlayer.Effects.CheckLightningEndMove();

        // Проверка на окончание гонки

        bool isRaceOver = IsRaceOver();

        if (isRaceOver) {
            PlayersControl.Instance.UpdatePlayersInfo();
            PlayersControl.Instance.MoveAllTokensToPedestal(_endMoveDelay);
            StartCoroutine(RaceOverDefer());
            return;
        }

        // Текущий игрок мог финишировать во время хода - проверить

        if (_currentPlayer.IsFinished) {
            SetNextPlayerIndex();
            return;
        }

        // Проверка на бонусные ходы

        _currentPlayer.AddMovesToDo(-1);

        if (_currentPlayer.MovesToDo <= 0) {
            _currentPlayer.MovesToDo = 0;
            SetNextPlayerIndex();
            return;
        }

        // Проверка на пропуск хода

        if (_currentPlayer.MovesSkip > 0) {
            StartCoroutine(SkipMoveDefer());
            return;
        }

        // Текущий игрок продолжает ходить
        PreparePlayerForMove();
    }

    private void ResetParamsAfterMove() {
        CubicControl.Instance.ModifiersControl.HideModifierMagnet();
        CellsControl.Instance.ResetCellMagnetHint();
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
        Debug.Log("CurrentCell " + _currentPlayer.GetTokenControl().CurrentCell);
        Debug.Log("_currentPlayer " + _currentPlayer.PlayerName);
        Debug.Log("Cerrent cell control name " + _currentPlayer.GetCurrentCell().transform.name);
        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            if (player.Boosters.Armor > 0) {
                Debug.Log(player.PlayerName + " " + player.Boosters.Armor);
            }
        }
    }
}

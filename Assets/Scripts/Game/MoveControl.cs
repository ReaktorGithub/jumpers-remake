using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    [SerializeField] private int currentPlayerIndex = 1;
    private int _stepsLeft;
    private int _movesLeft = 1;
    [SerializeField] private float stepDelay = 0.3f;
    [SerializeField] private float endMoveDelay = 0.5f;
    [SerializeField] private float skipMoveDelay = 1.5f;
    [SerializeField] private float alignTime = 5f;
    private TokenControl _currentTokenControl;
    private PlayerControl _currentPlayer;
    private CellControl _currentCellControl;
    private Pedestal _pedestal;
    private PlayerControl[] _playerControls = new PlayerControl[4];

    private CubicControl _cubicControl;
    private CellControl _startCellControl;
    private Messages _messages;
    private PopupAttack _popupAttack;
    private LevelData _levelData;
    private ModalResults _modalResults;
    private ModalWin _modalWin;
    private ModalLose _modalLose;
    private CameraControl _camera;
    private TopPanel _topPanel;
    private ECellTypes[] _skipRivalCheckTypes = new ECellTypes[2];

    private void Awake() {
        _cubicControl = GameObject.Find("Cubic").GetComponent<CubicControl>();
        _startCellControl = GameObject.Find("start").GetComponent<CellControl>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
        _messages = GameObject.Find("Messages").GetComponent<Messages>();
        _popupAttack = GameObject.Find("GameScripts").GetComponent<PopupAttack>();
        _levelData = GameObject.Find("GameScripts").GetComponent<LevelData>();
        _modalResults = GameObject.Find("ModalResults").GetComponent<ModalResults>();
        _modalWin = GameObject.Find("GameScripts").GetComponent<ModalWin>();
        _modalLose = GameObject.Find("GameScripts").GetComponent<ModalLose>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _skipRivalCheckTypes[0] = ECellTypes.Start;
        _skipRivalCheckTypes[1] = ECellTypes.Checkpoint;
    }

    private void Start() {
        _playerControls = GameObject.Find("GameScripts").GetComponent<PrepareLevel>().PlayerControls;
    }

    public PlayerControl CurrentPlayer {
        get { return _currentPlayer; }
        private set {}
    }

    public TokenControl CurrentTokenControl {
        get { return _currentTokenControl; }
        private set {}
    }

    public void MoveTokensToStart() {
        _startCellControl.AddToken("token_1");
        _startCellControl.AddToken("token_2");
        _startCellControl.AddToken("token_3");
        _startCellControl.AddToken("token_4");
        _startCellControl.AlignTokens(alignTime / 1.5f);
    }

    public string GetTokenNameByMoveOrder(int order) {
        foreach(PlayerControl player in _playerControls) {
            if (player.MoveOrder == order) {
                return player.TokenName;
            }
        }
        return null;
    }

    /*
        Порядок слоев фишек по умолчанию:
        order 1 - 3 - текущий
        order 2 - 0
        order 3 - 1
        order 4 - 2
        При смене игрока текущий порядок устанавливается на 3, все остальные -1
    */

    public void UpdateTokenLayerOrder() {
        foreach(PlayerControl player in _playerControls) {
            string tokenName = GetTokenNameByMoveOrder(player.MoveOrder);
            GameObject token = GameObject.Find(tokenName);
            if (token == null) {
                continue;
            }
            TokenControl tokenControl = token.GetComponent<TokenControl>();
            if (player.MoveOrder == currentPlayerIndex) {
                tokenControl.SetOrderInLayer(3);
            } else {
                tokenControl.SetOrderInLayer(tokenControl.GetOrderInLayer() - 1);
            }
        }
    }

    public void UpdateSqueezeAnimation() {
        foreach(PlayerControl player in _playerControls) {
            GameObject token = GameObject.Find(player.TokenName);
            if (token == null) {
                continue;
            }
            TokenControl tokenControl = token.GetComponent<TokenControl>();
            if (player.MoveOrder == currentPlayerIndex) {
                tokenControl.StartSqueeze();
            } else {
                tokenControl.StopSqueeze();
            }
        }
    }

    public void SetNextPlayerIndex() {
        if (currentPlayerIndex < 4) {
            currentPlayerIndex += 1;
        } else {
            currentPlayerIndex = 1;
        }
        SwitchPlayer();
    }

    public bool IsRaceOver() {
        int count = 0;
        foreach(PlayerControl player in _playerControls) {
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
        for (int i = 0; i < _playerControls.Length; i++) {
            if (_playerControls[i].MoveOrder == currentPlayerIndex) {
                if (_playerControls[i].IsFinished) {
                    SetNextPlayerIndex();
                    break;
                } else {
                    _currentPlayer = _playerControls[i];
                    _currentTokenControl = _playerControls[i].GetTokenControl();
                    UpdateTokenLayerOrder();
                    UpdateSqueezeAnimation();
                    UpdatePlayerInfo();
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
            _messages.AddMessage(messengerMessage);
        }
        _camera.FollowObject(_currentTokenControl.gameObject.transform);
    }

    public int CurrentPlayerIndex {
        get {
            return currentPlayerIndex;
        }
        set {
            if (value >= 1 && value <= 4) {
                currentPlayerIndex = value;
            } else {
                Debug.Log("Error while set current player " + value);
            }
        }
    }

    private IEnumerator MakeStepDefer() {
        yield return new WaitForSeconds(stepDelay);
        MakeStep();
    }

    // метод запускает серию проверок перед перемещением на следующую клетку

    private void InitTokenStep() {
        CheckBranch();
    }

    private void CheckBranch() {
        if (_currentCellControl.CellType == ECellTypes.Branch) {
            BranchControl branch = GameObject.Find(_currentCellControl.BranchName).GetComponent<BranchControl>();
            branch.ShowAllBranches();
            _topPanel.OpenWindow();
            string message = Utils.Wrap("Остаток: " + _stepsLeft, UIColors.Green);
            _cubicControl.WriteStatus(message);
            return;
        }

        StartCoroutine(MakeStepDefer());
    }

    private void MakeStep() {
        _stepsLeft--;
        _currentTokenControl.SetToNextCell(() => {
            // проверяем тип клетки, на которой сейчас находимся
            // некоторые типы прерывают движение, либо вызывают код во время движения

            _currentCellControl = _currentTokenControl.GetCurrentCellControl();

            if (_currentCellControl.CellType == ECellTypes.Checkpoint) {
                string message = Utils.Wrap(_currentPlayer.PlayerName, UIColors.Yellow) + " достигает " + Utils.Wrap("чекпойнта", UIColors.Blue);
                _messages.AddMessage(message);
            }

            if (_currentCellControl.CellType == ECellTypes.Finish) {
                _currentPlayer.ExecuteFinish();
                _camera.ClearFollow();
                return;
            }

            // если тип клетки не прерывает движение, то проверяем условие выхода из цикла шагов
            if (_stepsLeft > 0) {
                InitTokenStep();
            } else {
                StartCoroutine(ConfirmNewPositionDefer());
            }
        });
    }

    public void MakeMove(int score) {
        _stepsLeft = score;
        _currentCellControl = _currentTokenControl.GetCurrentCellControl();
        _currentCellControl.RemoveToken(_currentPlayer.TokenName);
        _currentCellControl.AlignTokens(alignTime);
        InitTokenStep();
    }

    private IEnumerator ConfirmNewPositionDefer() {
        yield return new WaitForSeconds(stepDelay);
        ConfirmNewPosition();
    }

    // подтверждение новой позиции по окончании движения

    public void ConfirmNewPosition() {
        _currentCellControl = _currentTokenControl.GetCurrentCellControl();
        _currentCellControl.AddToken(_currentPlayer.TokenName);
        _currentCellControl.AlignTokens(alignTime, () => {
            CheckCellEffects(_currentCellControl);
        });
    }

    // смена направления

    public void SwitchBranch(string nextCell) {
        if (_currentCellControl.CellType != ECellTypes.Branch) {
            Debug.Log("Error while switching branch");
            return;
        }
        
        BranchControl branch = GameObject.Find(_currentCellControl.BranchName).GetComponent<BranchControl>();
        branch.HideAllBranches();
        _topPanel.CloseWindow();
        _currentCellControl.NextCell = nextCell;
        _cubicControl.WriteStatus("");
        StartCoroutine(MakeStepDefer());
    }

    // Необходимые действия перед завершением хода:
    // 1.	Подбор бонуса.
    // 2.	Срабатывание капкана.
    // 3.	Исполнение эффекта.
    // 4.	Исполнение эффекта «стрелка», либо «синяя стрелка».
    // 5.	Атака на соперников.

    private void CheckCellEffects(CellControl cellControl) {
        if (cellControl.Effect == EControllableEffects.Green) {
            _movesLeft++;
            string message = Utils.Wrap(_currentPlayer.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("зелёный", UIColors.Green) + " эффект и ходит ещё раз";
            _messages.AddMessage(message);
        }

        if (cellControl.Effect == EControllableEffects.Yellow) {
            _currentPlayer.SkipMoveIncrease(_currentTokenControl);
            string message = Utils.Wrap(_currentPlayer.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("жёлтый", UIColors.Yellow) + " эффект и пропустит ход";
            _messages.AddMessage(message);
        }

        if (cellControl.Effect == EControllableEffects.Black) {
            _currentPlayer.ExecuteBlackEffect();
            return;
        }

        if (cellControl.Effect == EControllableEffects.Red) {
            _movesLeft = 0;
            _stepsLeft = 0;
            _currentPlayer.ExecuteRedEffect();
            return;
        }

        CheckCellArrows();
    }

    public void CheckCellArrows() {
        if (_currentCellControl.CellType == ECellTypes.Arrow) {
            _currentTokenControl.PutTokenToArrowSpline(_currentCellControl.ArrowSpline);
            return;
        }

        CheckCellRivals();
    }

    public void CheckCellRivals() {
        if (_skipRivalCheckTypes.Contains(_currentCellControl.CellType)) {
            StartCoroutine(EndMoveDefer());
            return;
        }
        
        List<string> tokens = _currentCellControl.CurrentTokens;

        if (tokens.Count > 1) {
            List<PlayerControl> rivals = new();
            foreach(PlayerControl player in _playerControls) {
                if (tokens.Contains(player.TokenName) && _currentPlayer.TokenName != player.TokenName) {
                    rivals.Add(player);
                }
            }
            
            _popupAttack.BuildContent(rivals);
            _popupAttack.OpenWindow();

            return;
        }

        // закончить ход, если нет соперников

        StartCoroutine(EndMoveDefer());
    }

    public void MoveAllTokensToPedestal() {
        foreach (PlayerControl player in _playerControls) {
            if (!player.IsFinished) {
                player.IsFinished = true;
                int place = _pedestal.SetPlayerToMinPlace(player);
                string name = "PlayerInfo" + player.MoveOrder;
                PlayerInfo info = GameObject.Find(name).GetComponent<PlayerInfo>();
                info.UpdatePlayerInfoDisplay(player, currentPlayerIndex);

                TokenControl tokenControl = player.GetTokenControl();
                IEnumerator coroutine = tokenControl.MoveToPedestalDefer(endMoveDelay, () => {
                    _pedestal.SetTokenToPedestal(player, place);
                });
                StartCoroutine(coroutine);
            }
        }
    }

    public void UpdatePlayerInfo() {
        foreach(PlayerControl player in _playerControls) {
            string name = "PlayerInfo" + player.MoveOrder;
            PlayerInfo info = GameObject.Find(name).GetComponent<PlayerInfo>();
            info.UpdatePlayerInfoDisplay(player, currentPlayerIndex);
        }
    }

    public IEnumerator EndMoveDefer() {
        yield return new WaitForSeconds(endMoveDelay);
        EndMove();
    }

    public IEnumerator SkipMoveDefer() {
        string message = Utils.Wrap(_currentPlayer.PlayerName, UIColors.Yellow) + " пропускает ход";
        _messages.AddMessage(message);
        message = Utils.Wrap("пропуск", UIColors.Yellow);
        _cubicControl.WriteStatus(message);
        yield return new WaitForSeconds(skipMoveDelay);
        _currentPlayer.SkipMoveDecrease(_currentTokenControl);
        EndMove();
    }

    public void EndMove() {
        // проверка на окончание гонки

        bool isRaceOver = IsRaceOver();

        if (isRaceOver) {
            UpdatePlayerInfo();
            MoveAllTokensToPedestal();
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

    public void AddMovesLeft(int count) {
        _movesLeft += count;
    }

    public IEnumerator RaceOverDefer() {
        yield return new WaitForSeconds(endMoveDelay);
        RaceOver();
    }

    public void RaceOver() {
        // раздача ресурсов

        foreach(PlayerControl player in _playerControls) {
            int coinsEarned = _levelData.PrizeCoins[player.PlaceAfterFinish - 1];
            int mallowsEarned = _levelData.PrizeMallows[player.PlaceAfterFinish - 1];
            int rubiesEarned = _levelData.PrizeRubies[player.PlaceAfterFinish - 1];
            player.AddCoins(coinsEarned);
            player.AddMallows(mallowsEarned);
            player.AddRubies(rubiesEarned);
        }

        CloseAllOptionalModals();
        _modalResults.OpenWindow(_playerControls);
    }

    public void CloseAllOptionalModals() {
        if (_modalLose.gameObject.activeInHierarchy) {
            _modalLose.CloseWindow();
        }

        if (_modalWin.gameObject.activeInHierarchy) {
            _modalWin.CloseWindow();
        }
    }

    public void DebugStatus() {
        Debug.Log("currentPlayerIndex " + currentPlayerIndex);
        Debug.Log("_stepsLeft " + _stepsLeft);
        Debug.Log("_movesLeft " + _movesLeft);
        Debug.Log("CurrentCell " + _currentTokenControl.CurrentCell);
        Debug.Log("_currentPlayer " + _currentPlayer.PlayerName);
        Debug.Log("Cerrent cell control name " + _currentCellControl.transform.name);
    }
}

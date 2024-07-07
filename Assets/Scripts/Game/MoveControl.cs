using System.Collections;
using System.Collections.Generic;
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
    private EffectFinish _effectFinish;
    private Pedestal _pedestal;
    private PlayerControl[] _playerControls = new PlayerControl[4];

    private CubicControl _cubicControl;
    private CellControl _startCellControl;
    private Messages _messages;
    private PopupAttack _popupAttack;

    private void Awake() {
        _cubicControl = GameObject.Find("Cubic").GetComponent<CubicControl>();
        _startCellControl = GameObject.Find("start").GetComponent<CellControl>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
        _effectFinish = GameObject.Find("Cells").GetComponent<EffectFinish>();
        _messages = GameObject.Find("Messages").GetComponent<Messages>();
        _popupAttack = GameObject.Find("GameScripts").GetComponent<PopupAttack>();
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
                    _currentTokenControl = GameObject.Find(_playerControls[i].TokenName).GetComponent<TokenControl>();
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

    public void PreparePlayerForMove(string cubicMessage, string messengerMessage = null) {
        _cubicControl.SetCubicInteractable(true);
        _cubicControl.WriteStatus(cubicMessage);
        if (messengerMessage != null) {
            _messages.AddMessage(messengerMessage);
        }
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

    private void MakeStep() {
        _stepsLeft--;
        _currentTokenControl.SetToNextCell(() => {

            // проверяем тип клетки, на которой сейчас находимся
            // некоторые типы прерывают движение
            CellControl cellControl = GameObject.Find(_currentTokenControl.CurrentCell).GetComponent<CellControl>();
            if (cellControl.CellType == ECellTypes.Finish) {
                _effectFinish.FinishPlayer();
                return;
            }

            // если тип клетки не прерывает движение, то проверяем условие выхода из цикла шагов
            if (_stepsLeft > 0) {
                StartCoroutine(MakeStepDefer());
            } else {
                StartCoroutine(ConfirmNewPositionDefer());
            }
        });
    }

    public void MakeMove(int score) {
        _stepsLeft = score;
        CellControl cellControl = GameObject.Find(_currentTokenControl.CurrentCell).GetComponent<CellControl>();
        cellControl.RemoveToken(_currentPlayer.TokenName);
        cellControl.AlignTokens(alignTime);
        MakeStep();
    }

    private IEnumerator ConfirmNewPositionDefer() {
        yield return new WaitForSeconds(stepDelay);
        ConfirmNewPosition();
    }

    // подтверждение новой позиции по окончании движения

    private void ConfirmNewPosition() {
        CellControl cellControl = GameObject.Find(_currentTokenControl.CurrentCell).GetComponent<CellControl>();
        cellControl.AddToken(_currentPlayer.TokenName);
        cellControl.AlignTokens(alignTime, () => {
            CheckCellEffects(cellControl);
        });
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

        CheckCellRivals();
    }

    public void CheckCellRivals() {
        CellControl cellControl = GameObject.Find(_currentTokenControl.CurrentCell).GetComponent<CellControl>();
        List<string> tokens = cellControl.CurrentTokens;

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
                TokenControl tokenControl = GameObject.Find(player.TokenName).GetComponent<TokenControl>();
                IEnumerator coroutine = tokenControl.MoveToPedestalDefer(endMoveDelay, () => {
                    _pedestal.SetPlayerToMinPlace(player);
                    string name = "PlayerInfo" + player.MoveOrder;
                    PlayerInfo info = GameObject.Find(name).GetComponent<PlayerInfo>();
                    info.UpdatePlayerInfoDisplay(player, currentPlayerIndex);
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

    /*
        Порядок проверок перед завершением хода:
        1. Закончился ли заезд.
        2. Есть ли дополнительные хода.
        3. Если нет доп ходов, то передать ход другому.
        4. Если есть доп ходы, то проверить на пропуски хода.
    */

    public void EndMove() {
        bool isRaceOver = IsRaceOver();

        if (isRaceOver) {
            Debug.Log("Race over");
            UpdatePlayerInfo();
            MoveAllTokensToPedestal();
            return;
        }

        _movesLeft--;

        if (_movesLeft <= 0) {
            SetNextPlayerIndex();
            return;
        }

        // у текущего игрока есть доп ход

        if (_currentPlayer.MovesSkip > 0) {
            StartCoroutine(SkipMoveDefer());
            return;
        }

        string cubicMessage = Utils.Wrap("бонусный ход!", UIColors.Green);
        PreparePlayerForMove(cubicMessage);

        // _cellControl.ShowTokensAtCells();
    }

    public void AddMovesLeft(int count) {
        _movesLeft += count;
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    [SerializeField] private int currentPlayerIndex = 1;
    private int _stepsLeft;
    [SerializeField] private float _stepDelay = 0.3f;
    [SerializeField] private float _endMoveDelay = 0.5f;
    private TokenControl _currentTokenControl;
    private PlayerControl _currentPlayer;
    private TextMeshProUGUI _uiTextCurrentPlayerIndex;
    private Pedestal _pedestal;
    private PlayerControl[] _playerControls = new PlayerControl[4];

    private CubicControl _cubicControl;
    private CellControl _startCellControl;

    private void Awake() {
        _uiTextCurrentPlayerIndex = GameObject.Find("CurrentPlayerIndex").GetComponent<TextMeshProUGUI>();
        _cubicControl = GameObject.Find("CubicImage").GetComponent<CubicControl>();
        _startCellControl = GameObject.Find("start").GetComponent<CellControl>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
    }

    private void Start() {
        _uiTextCurrentPlayerIndex.text = "Current player: " + currentPlayerIndex;
        _playerControls = GameObject.Find("GameScripts").GetComponent<PrepareLevel>().PlayerControls;
    }

    public void MoveTokensToStart() {
        _startCellControl.AddToken("token_1");
        _startCellControl.AddToken("token_2");
        _startCellControl.AddToken("token_3");
        _startCellControl.AddToken("token_4");
        _startCellControl.AlignTokens(2f);
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

    public void SetNextPlayer() {
        if (currentPlayerIndex < 4) {
            currentPlayerIndex += 1;
        } else {
            currentPlayerIndex = 1;
        }
        PrepareNextPlayer();
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
    
    public void PrepareNextPlayer() {
        for (int i = 0; i < _playerControls.Length; i++) {
            if (_playerControls[i].MoveOrder == currentPlayerIndex) {
                if (_playerControls[i].IsFinished) {
                    SetNextPlayer();
                    break;
                } else {
                    _currentPlayer = _playerControls[i];
                    _currentTokenControl = GameObject.Find(_playerControls[i].TokenName).GetComponent<TokenControl>();
                    UpdateTokenLayerOrder();
                    UpdateSqueezeAnimation();
                    _uiTextCurrentPlayerIndex.text = "Current player: " + currentPlayerIndex;
                }
            }
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
        yield return new WaitForSeconds(_stepDelay);
        MakeStep();
    }

    private void MakeStep() {
        _stepsLeft--;
        _currentTokenControl.SetToNextCell(() => {

            // проверяем тип клетки, на которой сейчас находимся
            CellControl cellControl = GameObject.Find(_currentTokenControl.CurrentCell).GetComponent<CellControl>();
            if (cellControl.CellType == ECellTypes.Finish) {
                FinishPlayer();
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
        cellControl.AlignTokens();
        MakeStep();
    }

    private IEnumerator ConfirmNewPositionDefer() {
        yield return new WaitForSeconds(_stepDelay);
        ConfirmNewPosition();
    }

    public void ConfirmNewPosition() {
        CellControl cellControl = GameObject.Find(_currentTokenControl.CurrentCell).GetComponent<CellControl>();
        cellControl.AddToken(_currentPlayer.TokenName);
        cellControl.AlignTokens();
        // здесь будут прочие проверки, прежде чем завершать ход
        StartCoroutine(EndMoveDefer());
    }

    public void MoveAllTokensToPedestal() {
        foreach (PlayerControl player in _playerControls) {
            if (!player.IsFinished) {
                TokenControl tokenControl = GameObject.Find(player.TokenName).GetComponent<TokenControl>();
                IEnumerator coroutine = tokenControl.MoveToPedestalDefer(_endMoveDelay, () => {
                    _pedestal.SetPlayerToMinPlace(player);
                });
                StartCoroutine(coroutine);
            }
        }
    }

    public IEnumerator EndMoveDefer() {
        yield return new WaitForSeconds(_endMoveDelay);
        EndMove();
    }

    public void EndMove() {
        bool isRaceOver = IsRaceOver();
        if (isRaceOver) {
            Debug.Log("Race over");
            MoveAllTokensToPedestal();
            return;
        }
        SetNextPlayer();
        _cubicControl.SetCubicInteractable(true);
        // _cellControl.ShowTokensAtCells();
    }

    public void FinishPlayer() {
        Debug.Log("finish player");
        _currentPlayer.IsFinished = true;
        IEnumerator coroutine = _currentTokenControl.MoveToPedestalDefer(_endMoveDelay, () => {
            _pedestal.SetPlayerToMaxPlace(_currentPlayer);
            StartCoroutine(EndMoveDefer());
        });
        StartCoroutine(coroutine);
    }
}

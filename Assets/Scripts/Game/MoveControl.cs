using System.Collections;
using TMPro;
using UnityEngine;

public class MoveControl : MonoBehaviour
{
    [SerializeField] private int currentPlayer = 1;
    private int _stepsLeft;
    [SerializeField] private float _stepDelay = 0.3f;
    [SerializeField] private float _endMoveDelay = 0.5f;
    private TokenControl _tokenControl;
    private TextMeshProUGUI _uiTextCurrentPlayerIndex;
    private PlayerControl[] _playerControls = new PlayerControl[4];

    private CubicControl _cubicControl;
    private CellControl _startCellControl;

    private void Awake() {
        _uiTextCurrentPlayerIndex = GameObject.Find("CurrentPlayerIndex").GetComponent<TextMeshProUGUI>();
        _cubicControl = GameObject.Find("CubicImage").GetComponent<CubicControl>();
        _startCellControl = GameObject.Find("start").GetComponent<CellControl>();
        
    }

    private void Start() {
        _uiTextCurrentPlayerIndex.text = "Current player: " + currentPlayer;
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
            TokenControl tokenControl = GameObject.Find(tokenName).GetComponent<TokenControl>();
            if (player.MoveOrder == currentPlayer) {
                tokenControl.SetOrderInLayer(3);
            } else {
                tokenControl.SetOrderInLayer(tokenControl.GetOrderInLayer() - 1);
            }
        }
    }
    
    public void SetNextPlayer() {
        if (currentPlayer < 4) {
            currentPlayer += 1;
        } else {
            currentPlayer = 1;
        }
        UpdateTokenLayerOrder();
        _uiTextCurrentPlayerIndex.text = "Current player: " + currentPlayer;
    }

    public int CurrentPlayer {
        get {
            return currentPlayer;
        }
        set {
            if (value >= 1 && value <= 4) {
                currentPlayer = value;
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
        _tokenControl.SetToNextCell(() => {
            if (_stepsLeft > 0) {
                StartCoroutine(MakeStepDefer());
            } else {
                StartCoroutine(ConfirmNewPositionDefer());
            }
        });
    }

    public void MakeMove(int score) {
        string tokenName = GetTokenNameByMoveOrder(currentPlayer);
        _tokenControl = GameObject.Find(tokenName).GetComponent<TokenControl>();
        _stepsLeft = score;
        CellControl cellControl = GameObject.Find(_tokenControl.CurrentCell).GetComponent<CellControl>();
        cellControl.RemoveToken(tokenName);
        cellControl.AlignTokens();
        MakeStep();
    }

    private IEnumerator ConfirmNewPositionDefer() {
        yield return new WaitForSeconds(_stepDelay);
        ConfirmNewPosition();
    }

    public void ConfirmNewPosition() {
        string tokenName = GetTokenNameByMoveOrder(currentPlayer);
        CellControl cellControl = GameObject.Find(_tokenControl.CurrentCell).GetComponent<CellControl>();
        cellControl.AddToken(tokenName);
        cellControl.AlignTokens();
        // здесь будут прочие проверки, прежде чем завершать ход
        StartCoroutine(EndMoveDefer());
    }

    private IEnumerator EndMoveDefer() {
        yield return new WaitForSeconds(_endMoveDelay);
        EndMove();
    }

    public void EndMove() {
        SetNextPlayer();
        _cubicControl.SetCubicInteractable(true);
        // _cellControl.ShowTokensAtCells();
    }
}

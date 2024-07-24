using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private string playerName;

    [SerializeField] private string tokenName;
    [SerializeField] private int moveOrder;
    private int _placeAfterFinish;
    private bool _isFinished = false;
    private int _movesSkip = 0;
    
    private List<EAttackTypes> _availableAttackTypes = new();
    private ModalWarning _modalWarning;
    private ModalLose _modalLose;
    private ModalWin _modalWin;
    
    private Pedestal _pedestal;
    private Sprite _tokenImage;

    // player resources
    [SerializeField] private int coins = 0;
    [SerializeField] private int mallows = 0;
    [SerializeField] private int rubies = 0;
    [SerializeField] private int power = 2;

    private void Awake() {
        _availableAttackTypes.Add(EAttackTypes.Usual);
        _modalWarning = GameObject.Find("GameScripts").GetComponent<ModalWarning>();
        _modalLose = GameObject.Find("GameScripts").GetComponent<ModalLose>();
        _modalWin = GameObject.Find("GameScripts").GetComponent<ModalWin>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
    }

    public int MoveOrder {
        get { return moveOrder; }
        set {
            if (value >= 1 && value <= 4) {
                moveOrder = value;
            }
        }
    }

    public int MovesSkip {
        get { return _movesSkip; }
        private set {}
    }

    public string TokenName {
        get { return tokenName; }
        set { tokenName = value; }
    }

    public int PlaceAfterFinish {
        get { return _placeAfterFinish; }
        set {
            if (value >= 1 && value <= 4) {
                _placeAfterFinish = value;
            }
        }
    }

    public bool IsFinished {
        get { return _isFinished; }
        set { _isFinished = value; }
    }

    public Sprite TokenImage {
        get { return _tokenImage; }
        set { _tokenImage = value; }
    }

    public string PlayerName {
        get { return playerName; }
        set {
            if (value.Length < 15) {
                playerName = value;
            } else {
                playerName = value[..14];
            }
        }
    }

    public int Coins {
        get { return coins; }
        private set {}
    }

    public int Mallows {
        get { return mallows; }
        private set {}
    }

    public int Rubies {
        get { return rubies; }
        private set {}
    }

    public int Power {
        get { return power; }
        private set {}
    }

    public void SkipMoveIncrease(TokenControl token) {
        _movesSkip++;
        token.UpdateSkips(_movesSkip);
    }

    public void SkipMoveDecrease(TokenControl token) {
        _movesSkip--;
        token.UpdateSkips(_movesSkip);
    }

    public TokenControl GetTokenControl() {
        return GameObject.Find(tokenName).GetComponent<TokenControl>();
    }

    public void OpenPowerWarningModal(Action callback = null) {
        _modalWarning.SetHeadingText("Предупреждение");
        _modalWarning.SetBodyText("Силы на нуле. Красная или чёрная клетки приведут к поражению!");
        _modalWarning.SetCallback(callback);
        _modalWarning.OpenWindow();
    }

    public void ConfirmLose() {
        _modalLose.OpenWindow();
        _isFinished = true;
        int place = _pedestal.SetPlayerToMinPlace(this);
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" ВЫЛЕТАЕТ С ТРАССЫ!", UIColors.Red);
        Messages.Instance.AddMessage(message);

        TokenControl tokenControl = GetTokenControl();
        IEnumerator coroutine = tokenControl.MoveToPedestalDefer(PlayersControl.Instance.LoseDelay, () => {
            _pedestal.SetTokenToPedestal(this, place);
            CellControl cellControl = tokenControl.GetCurrentCellControl();
            cellControl.RemoveToken(tokenName);
            StartCoroutine(MoveControl.Instance.EndMoveDefer());
        });
        StartCoroutine(coroutine);
    }

    // изменение ресурсов

    public void AddCoins(int value) {
        coins += value;
    }

    public void AddMallows(int value) {
        mallows += value;
    }

    public void AddRubies(int value) {
        rubies += value;
    }

    // атака

    public List<EAttackTypes> AvailableAttackTypes {
        get { return _availableAttackTypes; }
        private set {}
    }

    public void ExecuteAttackUsual(PlayerControl rival, int currentPlayerIndex) {
        power--;
        MoveControl.Instance.AddMovesLeft(1);
        rival.SkipMoveIncrease(rival.GetTokenControl());
        PlayersControl.Instance.UpdatePlayersInfo(currentPlayerIndex);
        string message1 = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" АТАКУЕТ ", UIColors.Red) + Utils.Wrap(rival.PlayerName, UIColors.Yellow) + "!";
        Messages.Instance.AddMessage(message1);
        string message2 = Utils.Wrap(rival.PlayerName, UIColors.Yellow) + " пропустит ход, а " + Utils.Wrap(PlayerName, UIColors.Yellow) + " ходит ещё раз";
        Messages.Instance.AddMessage(message2);

        if (power == 0) {
            OpenPowerWarningModal(() => {
                StartCoroutine(MoveControl.Instance.EndMoveDefer());
            });
            return;
        }

        StartCoroutine(MoveControl.Instance.EndMoveDefer());
    }

    // исполнение эффектов

    public void ExecuteBlackEffect(CellControl cellControl, TokenControl tokenControl, int currentPlayerIndex) {
        power--;
        PlayersControl.Instance.UpdatePlayersInfo(currentPlayerIndex);
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("ЧЁРНЫЙ", UIColors.Black) + " эффект! Минус 1 сила";
        Messages.Instance.AddMessage(message);

        if (power == 0) {
            OpenPowerWarningModal(() => {
                CellChecker.Instance.CheckCellRivals(cellControl, this);
            });
            return;
        }

        if (power < 0) {
            ConfirmLose();
            return;
        }

        CellChecker.Instance.CheckCellArrows(cellControl, this, tokenControl);
    }

    public void ExecuteRedEffect(int currentPlayerIndex) {
        power--;
        PlayersControl.Instance.UpdatePlayersInfo(currentPlayerIndex);
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("КРАСНЫЙ", UIColors.Red) + " эффект! Минус 1 сила";
        Messages.Instance.AddMessage(message);

        if (power == 0) {
            OpenPowerWarningModal(() => {
                RedEffectTokenMove();
            });
            return;
        }

        if (power < 0) {
            ConfirmLose();
            return;
        }

        StartCoroutine(RedEffectTokenMoveDefer());
    }

    private IEnumerator RedEffectTokenMoveDefer() {
        yield return new WaitForSeconds(PlayersControl.Instance.RedEffectDelay);
        RedEffectTokenMove();
    }

    private void RedEffectTokenMove() {
        TokenControl tokenControl = GetTokenControl();
        CellControl cellControl = tokenControl.GetCurrentCellControl();
        if (!GameObject.Find(tokenControl.CurrentCell).TryGetComponent(out RedCell redCell)) {
            Debug.Log("Red cell not found");
            return;
        }
        CellControl nextCellcontrol = GameObject.Find(redCell.PenaltyCell).GetComponent<CellControl>();
        tokenControl.SetToSpecifiedCell(nextCellcontrol, redCell.PenaltyCell, () => {
            cellControl.RemoveToken(TokenName);
            MoveControl.Instance.ConfirmNewPosition();
        });
    }

    public void ExecuteFinish() {
        _modalWin.OpenWindow();
        _isFinished = true;
        int place = _pedestal.SetPlayerToMaxPlace(this);
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" ФИНИШИРУЕТ ", UIColors.Green) + " на " + place + " месте!";
        Messages.Instance.AddMessage(message);

        TokenControl tokenControl = GetTokenControl();
        IEnumerator coroutine = tokenControl.MoveToPedestalDefer(PlayersControl.Instance.FinishDelay, () => {
            _pedestal.SetTokenToPedestal(this, place);
            StartCoroutine(MoveControl.Instance.EndMoveDefer());
        });
        StartCoroutine(coroutine);
    }
}

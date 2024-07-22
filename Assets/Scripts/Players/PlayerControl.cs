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
    private MoveControl _moveControl;
    private Messages _messages;
    private ModalWarning _modalWarning;
    private ModalLose _modalLose;
    private ModalWin _modalWin;
    
    private Pedestal _pedestal;
    private Sprite _tokenImage;
    private PlayersControl _playersControl;

    // player resources
    [SerializeField] private int coins = 0;
    [SerializeField] private int mallows = 0;
    [SerializeField] private int rubies = 0;
    [SerializeField] private int power = 2;

    private void Awake() {
        _availableAttackTypes.Add(EAttackTypes.Usual);
        _moveControl = GameObject.Find("GameScripts").GetComponent<MoveControl>();
        _messages = GameObject.Find("Messages").GetComponent<Messages>();
        _modalWarning = GameObject.Find("GameScripts").GetComponent<ModalWarning>();
        _modalLose = GameObject.Find("GameScripts").GetComponent<ModalLose>();
        _modalWin = GameObject.Find("GameScripts").GetComponent<ModalWin>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
        _playersControl = GameObject.Find("Players").GetComponent<PlayersControl>();
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
        _messages.AddMessage(message);

        TokenControl tokenControl = GetTokenControl();
        IEnumerator coroutine = tokenControl.MoveToPedestalDefer(_playersControl.LoseDelay, () => {
            _pedestal.SetTokenToPedestal(this, place);
            CellControl cellControl = tokenControl.GetCurrentCellControl();
            cellControl.RemoveToken(tokenName);
            StartCoroutine(_moveControl.EndMoveDefer());
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

    public void ExecuteAttackUsual(PlayerControl rival) {
        power--;
        _moveControl.AddMovesLeft(1);
        rival.SkipMoveIncrease(rival.GetTokenControl());
        _moveControl.UpdatePlayerInfo();
        string message1 = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" АТАКУЕТ ", UIColors.Red) + Utils.Wrap(rival.PlayerName, UIColors.Yellow) + "!";
        _messages.AddMessage(message1);
        string message2 = Utils.Wrap(rival.PlayerName, UIColors.Yellow) + " пропустит ход, а " + Utils.Wrap(PlayerName, UIColors.Yellow) + " ходит ещё раз";
        _messages.AddMessage(message2);

        if (power == 0) {
            OpenPowerWarningModal(() => {
                StartCoroutine(_moveControl.EndMoveDefer());
            });
            return;
        }

        StartCoroutine(_moveControl.EndMoveDefer());
    }

    // исполнение эффектов

    public void ExecuteBlackEffect() {
        power--;
        _moveControl.UpdatePlayerInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("ЧЁРНЫЙ", UIColors.Black) + " эффект! Минус 1 сила";
        _messages.AddMessage(message);

        if (power == 0) {
            OpenPowerWarningModal(() => {
                _moveControl.CheckCellRivals();
            });
            return;
        }

        if (power < 0) {
            ConfirmLose();
            return;
        }

        _moveControl.CheckCellArrows();
    }

    public void ExecuteRedEffect() {
        power--;
        _moveControl.UpdatePlayerInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("КРАСНЫЙ", UIColors.Red) + " эффект! Минус 1 сила";
        _messages.AddMessage(message);

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
        yield return new WaitForSeconds(_playersControl.RedEffectDelay);
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
            _moveControl.ConfirmNewPosition();
        });
    }

    public void ExecuteFinish() {
        _modalWin.OpenWindow();
        _isFinished = true;
        int place = _pedestal.SetPlayerToMaxPlace(this);
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" ФИНИШИРУЕТ ", UIColors.Green) + " на " + place + " месте!";
        _messages.AddMessage(message);

        TokenControl tokenControl = GetTokenControl();
        IEnumerator coroutine = tokenControl.MoveToPedestalDefer(_playersControl.FinishDelay, () => {
            _pedestal.SetTokenToPedestal(this, place);
            StartCoroutine(_moveControl.EndMoveDefer());
        });
        StartCoroutine(coroutine);
    }
}

using System;
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
    [SerializeField] private int coins = 0;
    [SerializeField] private int rubies = 0;
    [SerializeField] private int power = 2;
    private List<EAttackTypes> _availableAttackTypes = new();
    private MoveControl _moveControl;
    private Messages _messages;
    private ModalWarning _modalWarning;

    private void Awake() {
        _availableAttackTypes.Add(EAttackTypes.Usual);
        _moveControl = GameObject.Find("GameScripts").GetComponent<MoveControl>();
        _messages = GameObject.Find("Messages").GetComponent<Messages>();
        _modalWarning = GameObject.Find("GameScripts").GetComponent<ModalWarning>();
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

    public Sprite GetTokenSprite() {
        return GameObject.Find(tokenName).transform.Find("TokenImage").GetComponent<SpriteRenderer>().sprite;
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

    public void ExecuteBlackEffect() {
        power--;
        _moveControl.UpdatePlayerInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попался на " + Utils.Wrap("ЧЁРНЫЙ", UIColors.Black) + " эффект! Минус 1 сила";
        _messages.AddMessage(message);

        if (power == 0) {
            OpenPowerWarningModal(() => {
                _moveControl.CheckCellRivals();
            });
            return;
        }

        if (power < 0) {
            // todo
        }

        _moveControl.CheckCellRivals();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private string playerName;

    [SerializeField] private string tokenName;
    [SerializeField] private int moveOrder;
    [SerializeField] private bool isReverseMove;
    private int _placeAfterFinish;
    private bool _isFinished = false;
    private int _movesSkip = 0;
    
    private List<EAttackTypes> _availableAttackTypes = new();
    private ModalWarning _modalWarning;
    private ModalLose _modalLose;
    private ModalWin _modalWin;
    
    private Pedestal _pedestal;
    private Sprite _tokenImage;

    // Ресурсы игрока
    [SerializeField] private int coins = 0;
    [SerializeField] private int mallows = 0;
    [SerializeField] private int rubies = 0;
    [SerializeField] private int power = 2;

    // Эффекты
    [SerializeField] private int effectsGreen = 0;
    [SerializeField] private int effectsYellow = 0;
    [SerializeField] private int effectsBlack = 0;
    [SerializeField] private int effectsRed = 0;
    [SerializeField] private int effectsStar = 0;
    private bool _isEffectPlaced;

    // Усилители
    [SerializeField] private int boosterMagnet = 0;
    [SerializeField] private int boosterSuperMagnet = 0;

    // Кубик
    [SerializeField] private int cubicMaxScore = 6;

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

    public bool IsReverseMove {
        get { return isReverseMove; }
        set {}
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

    public int EffectsGreen {
        get { return effectsGreen; }
        set { effectsGreen = value; }
    }

    public int EffectsYellow {
        get { return effectsYellow; }
        set { effectsYellow = value; }
    }

    public int EffectsRed {
        get { return effectsRed; }
        set { effectsRed = value; }
    }

    public int EffectsBlack {
        get { return effectsBlack; }
        set { effectsBlack = value; }
    }

    public int EffectsStar {
        get { return effectsStar; }
        set { effectsStar = value; }
    }

    public bool IsEffectPlaced {
        get { return _isEffectPlaced; }
        set { _isEffectPlaced = value; }
    }

    public int BoosterMagnet {
        get { return boosterMagnet; }
        set { boosterMagnet = value; }
    }

    public int BoosterSuperMagnet {
        get { return boosterSuperMagnet; }
        set { boosterSuperMagnet = value; }
    }

    public int CubicMaxScore {
        get { return cubicMaxScore; }
        set { cubicMaxScore = value; }
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

    public void AddPower(int value) {
        power += value;
    }

    public void AddMallows(int value) {
        mallows += value;
    }

    public void AddRubies(int value) {
        rubies += value;
    }

    public void AddEffectGreen(int value) {
        effectsGreen += value;
    }

    public void AddEffectYellow(int value) {
        effectsYellow += value;
    }

    public void AddEffectBlack(int value) {
        effectsBlack += value;
    }

    public void AddEffectRed(int value) {
        effectsRed += value;
    }

    public void AddEffectStar(int value) {
        effectsStar += value;
    }

    public void AddMagnets(int value) {
        boosterMagnet += value;
    }

    public void AddMagnetsSuper(int value) {
        boosterSuperMagnet += value;
    }

    // атака

    public List<EAttackTypes> AvailableAttackTypes {
        get { return _availableAttackTypes; }
        private set {}
    }

    public void ExecuteAttackUsual(PlayerControl rival, int currentPlayerIndex) {
        AddPower(-1);
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
        AddPower(-1);
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
        AddPower(-1);
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
        if (!tokenControl.CurrentCell.TryGetComponent(out RedCell redCell)) {
            Debug.Log("Red cell not found");
            return;
        }
        CellControl nextCellcontrol = redCell.PenaltyCell.GetComponent<CellControl>();
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

    public void ExecuteReplaceEffect(EControllableEffects effect, int playerIndex) {
        ManualContent manual = Manual.Instance.GetEffectManual(effect);

        // todo уровень эффекта должен вычисляться из PlayerControl
        int effectLevel = 1;

        int cost = manual.GetCostToReplaceEffect(effectLevel);
        if (manual.ReplaceEffectResourceType == EResourceTypes.Power) {
            AddPower(-cost);
        } else {
            AddCoins(-cost);
        }
        PlayersControl.Instance.UpdatePlayersInfo(playerIndex);

        switch(effect) {
            case EControllableEffects.Green: {
                AddEffectGreen(-1);
                break;
            }
            case EControllableEffects.Red: {
                AddEffectRed(-1);
                break;
            }
            case EControllableEffects.Yellow: {
                AddEffectYellow(-1);
                break;
            }
            case EControllableEffects.Black: {
                AddEffectBlack(-1);
                break;
            }
        }

        if (power == 0) {
            OpenPowerWarningModal();
        }
    }

    // разное

    public bool IsEnoughEffects(EControllableEffects effect) {
        switch(effect) {
            case EControllableEffects.Green: {
                return effectsGreen > 0;
            }
            case EControllableEffects.Yellow: {
                return effectsYellow > 0;
            }
            case EControllableEffects.Black: {
                return effectsBlack > 0;
            }
            case EControllableEffects.Red: {
                return effectsRed > 0;
            }
            default: return false;
        }
    }
}

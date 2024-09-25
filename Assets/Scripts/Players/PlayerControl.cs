using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerEffects))]
[RequireComponent(typeof(PlayerBoosters))]

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private string _playerName;
    [SerializeField] private EPlayerTypes _type = EPlayerTypes.Me;
    [SerializeField] private EAiTypes _aiType = EAiTypes.Normal;
    [SerializeField] private GameObject _tokenObject;
    [SerializeField] private int _moveOrder;
    [SerializeField] private bool _isReverseMove;
    private int _placeAfterFinish;
    private bool _isFinished = false;
    private int _movesSkip = 0; // пропуски хода
    private int _movesToDo = 0; // сколько нужно сделать ходов с броском кубика
    private int _stepsLeft = 0; // сколько шагов фишкой осталось сделать
    
    private List<EAttackTypes> _availableAttackTypes = new();
    private ModalWarning _modalWarning;
    private ModalLose _modalLose;
    private Pedestal _pedestal;
    private Sprite _tokenImage;
    private PlayerEffects _effects;
    private PlayerBoosters _boosters;

    // Ресурсы игрока
    [SerializeField] private int _coins = 0;
    [SerializeField] private int _mallows = 0;
    [SerializeField] private int _rubies = 0;
    [SerializeField] private int _power = 2;

    // Кубик
    [SerializeField] private int _cubicMaxScore = 6;

    private void Awake() {
        _availableAttackTypes.Add(EAttackTypes.Usual);
        _availableAttackTypes.Add(EAttackTypes.MagicKick);
        _availableAttackTypes.Add(EAttackTypes.Knockout);
        _modalWarning = GameObject.Find("ModalScripts").GetComponent<ModalWarning>();
        _modalLose = GameObject.Find("ModalScripts").GetComponent<ModalLose>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
        _effects = GetComponent<PlayerEffects>();
        _boosters = GetComponent<PlayerBoosters>();
    }

    // Изменение свойств напрямую

    public int MoveOrder {
        get { return _moveOrder; }
        set {
            if (value >= 1 && value <= 4) {
                _moveOrder = value;
            }
        }
    }

    public EPlayerTypes Type {
        get { return _type; }
        set { _type = value; }
    }

    public bool IsMe() {
        return _type == EPlayerTypes.Me || !AiControl.Instance.EnableAi;
    }

    public bool IsAi() {
        if (!AiControl.Instance.EnableAi) {
            return false;
        }
        return _type == EPlayerTypes.Ai;
    }

    public EAiTypes AiType {
        get { return _aiType; }
        set { _aiType = value; }
    }

    public int MovesSkip {
        get { return _movesSkip; }
        private set {}
    }

    public int MovesToDo {
        get { return _movesToDo; }
        set { _movesToDo = value; }
    }

    public int StepsLeft {
        get { return _stepsLeft; }
        set { _stepsLeft = value; }
    }

    public bool IsReverseMove {
        get { return _isReverseMove; }
        set { _isReverseMove = value; }
    }

    public GameObject TokenObject {
        get { return _tokenObject; }
        set { _tokenObject = value; }
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
        get { return _playerName; }
        set {
            if (value.Length < 15) {
                _playerName = value;
            } else {
                _playerName = value[..14];
            }
        }
    }

    public int Coins {
        get { return _coins; }
        set { _coins = value; }
    }

    public int Mallows {
        get { return _mallows; }
        set { _mallows = value; }
    }

    public int Rubies {
        get { return _rubies; }
        set { _rubies = value; }
    }

    public int Power {
        get { return _power; }
        set { _power = value; }
    }

    public PlayerEffects Effects {
        get { return _effects; }
        private set {}
    }

    public PlayerBoosters Boosters {
        get { return _boosters; }
        private set {}
    }

    public int CubicMaxScore {
        get { return _cubicMaxScore; }
        set { _cubicMaxScore = value; }
    }

    // Изменение параметров движения с помощью инкремента или декремента

    public void AddMovesToDo(int count) {
        _movesToDo += count;
    }

    public void AddMovesSkip(int count) {
        _movesSkip += count;
    }

    public void AddStepsLeft(int count) {
        _stepsLeft += count;
    }

    public void SkipMoveIncrease() {
        _movesSkip++;
        GetTokenControl().UpdateSkips(_movesSkip);
    }

    public void SkipMoveDecrease() {
        _movesSkip--;
        GetTokenControl().UpdateSkips(_movesSkip);
    }

    // Изменение ресурсов с помощью инкремента или декремента

    public void AddCoins(int value) {
        _coins += value;
        GetTokenControl().AddBonusEventToQueue(value);
    }

    public void AddPower(int value) {
        _power += value;
    }

    public void AddMallows(int value) {
        _mallows += value;
    }

    public void AddRubies(int value) {
        _rubies += value;
    }

    // атака

    public void AddAvailableAttackType(EAttackTypes type) {
        if (!_availableAttackTypes.Contains(type)) {
            _availableAttackTypes.Add(type);
        }
    }

    public void RemoveAvailableAttackType(EAttackTypes type) {
        if (_availableAttackTypes.Contains(type)) {
            _availableAttackTypes.Remove(type);
        }
    }

    public List<EAttackTypes> AvailableAttackTypes {
        get { return _availableAttackTypes; }
        private set {}
    }

    public void ExecuteAttack(EAttackTypes type, PlayerControl selectedPlayer) {
        switch(type) {
            case EAttackTypes.MagicKick: {
                ExecuteAttackMagicKick(selectedPlayer);
                break;
            }
            case EAttackTypes.Vampire: {
                ExecuteAttackVampire(selectedPlayer);
                break;
            }
            case EAttackTypes.Knockout: {
                ExecuteAttackKnockout(selectedPlayer);
                break;
            }
            default: {
                ExecuteAttackUsual(selectedPlayer);
                break;
            }
        }
    }

    public void ExecuteAttackUsual(PlayerControl rival) {
        AddPower(-1);
        AddMovesToDo(1);
        rival.SkipMoveIncrease();
        PlayersControl.Instance.UpdatePlayersInfo();
        string message1 = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" АТАКУЕТ ", UIColors.Red) + Utils.Wrap(rival.PlayerName, UIColors.Yellow) + "!";
        Messages.Instance.AddMessage(message1);
        string message2 = Utils.Wrap(rival.PlayerName, UIColors.Yellow) + " пропустит ход, а " + Utils.Wrap(PlayerName, UIColors.Yellow) + " ходит ещё раз";
        Messages.Instance.AddMessage(message2);

        PlayersControl.Instance.CheckIsPlayerOutOfPower(this, () => StartCoroutine(MoveControl.Instance.EndMoveDefer()));
    }

    public void ExecuteAttackMagicKick(PlayerControl rival) {
        int powerSpend = Manual.Instance.AttackMagicKick.GetCost(1); // todo вычислять из уровня атаки
        AddPower(-powerSpend);
        PlayersControl.Instance.UpdatePlayersInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" ДАЁТ ПИНКА ", UIColors.Red) + Utils.Wrap(rival.PlayerName, UIColors.Yellow) + "!";
        Messages.Instance.AddMessage(message);

        int steps = Manual.Instance.AttackMagicKick.GetCauseEffect(1); // todo вычислять из уровня атаки
        MoveControl.Instance.MakeMagicKickMove(rival, rival.GetCurrentCell(), steps); 

        PlayersControl.Instance.CheckIsPlayerOutOfPower(this);
    }

    public void ExecuteAttackVampire(PlayerControl rival) {
        AddPower(1);
        _boosters.AddVampire(-1);
        AddMovesToDo(1);
        rival.AddPower(-1);
        rival.SkipMoveIncrease();
        PlayersControl.Instance.UpdatePlayersInfo();
        BoostersControl.Instance.UpdateBoostersFromPlayer(this);
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" КУСАЕТ ", UIColors.Red) + Utils.Wrap(rival.PlayerName, UIColors.Yellow) + " и забирает его силу!";
        Messages.Instance.AddMessage(message);

        PlayersControl.Instance.CheckIsPlayerOutOfPower(rival, () => StartCoroutine(MoveControl.Instance.EndMoveDefer()));
    }

    public void ExecuteAttackKnockout(PlayerControl rival) {
        int powerSpend = Manual.Instance.AttackKnockout.GetCost(3); // todo вычислять из уровня атаки
        int moneyBonus = Manual.Instance.AttackKnockout.GetCauseEffect(3); // todo вычислять из уровня атаки

        AddPower(-powerSpend);
        AddMovesToDo(1);
        rival.AddCoins(-moneyBonus);
        AddCoins(moneyBonus);
        string message1 = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" НОКАУТИРУЕТ ", UIColors.Red) + Utils.Wrap(rival.PlayerName, UIColors.Yellow) + "!";
        Messages.Instance.AddMessage(message1);
        rival.ConfirmLose();
        PlayersControl.Instance.UpdatePlayersInfo();

        PlayersControl.Instance.CheckIsPlayerOutOfPower(this);
    }

    public void ExecuteCancelAttack() {
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " отказался от атаки";
        Messages.Instance.AddMessage(message);
        StartCoroutine(MoveControl.Instance.EndMoveDefer());
    }

    // Модалки

    public void OpenPowerWarningModal(Action callback = null) {
        _modalWarning.SetHeadingText("Предупреждение");
        _modalWarning.SetBodyText("Силы на нуле. Красная или чёрная клетки приведут к поражению!");
        _modalWarning.SetCallback(callback);
        _modalWarning.OpenModal();
    }

    public void OpenSavedByShieldModal(Action callback = null) {
        _modalWarning.SetHeadingText("Железный щит");
        _modalWarning.SetBodyText("Благодаря <b>железному щиту</b> вы не теряете силу на этой клетке.");
        _modalWarning.SetCallback(callback);
        _modalWarning.OpenModal();
    }

    public void OpenAttackShieldModal(Action callback = null) {
        _modalWarning.SetHeadingText("Соперник защищён");
        _modalWarning.SetBodyText("Вы не можете атаковать соперника со щитом.");
        _modalWarning.SetCallback(callback);
        _modalWarning.OpenModal();
    }

    public void ConfirmLose() {
        if (IsMe()) {
            _modalLose.OpenModal();
        }
        _isFinished = true;
        int place = _pedestal.SetPlayerToMinPlace(this);
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" ВЫЛЕТАЕТ С ТРАССЫ!", UIColors.Red);
        Messages.Instance.AddMessage(message);

        TokenControl tokenControl = GetTokenControl();
        IEnumerator coroutine = tokenControl.MoveToPedestalDefer(PlayersControl.Instance.LoseDelay, () => {
            _pedestal.SetTokenToPedestal(this, place);
            MoveControl.Instance.ConfirmLosePlayer(this);
        });
        StartCoroutine(coroutine);
    }

    // Разное

    public TokenControl GetTokenControl() {
        return _tokenObject.GetComponent<TokenControl>();
    }

    public CellControl GetCurrentCell() {
        return GetTokenControl().GetCurrentCellControl();
    }

    // Вычислить среднее отставание от других игроков (в шагах)
    // Если число положительное, то есть отставание

    public float GetOtherPlayersMeanGap() {
        CellControl myCell = GetCurrentCell();
        int myStepsToFinish = CellsControl.Instance.GetStepsToFinish(myCell.gameObject);

        int otherPlayersCount = 0;

        int totalSteps = myStepsToFinish;

        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            if (player.MoveOrder != _moveOrder && !player.IsFinished) {
                GameObject cell = player.GetCurrentCell().gameObject;
                int steps = CellsControl.Instance.GetStepsToFinish(cell);
                otherPlayersCount++;
                totalSteps += steps;
            }
        }

        float average = totalSteps / (otherPlayersCount + 1);
        return myStepsToFinish - average;
    }

    public bool AmIBehingMyRivals(int criticalSteps = 10) {
        return GetOtherPlayersMeanGap() > criticalSteps;
    }
}

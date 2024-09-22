using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private int _lightningMoves = 0; // осталось ходов с молнией
    [SerializeField] private bool _isLightning = false; // режим молнии
    [SerializeField] private int _armor = 0; // сколько ходов осталось со щитом (включая ходы соперников)
    [SerializeField] private bool _isIronArmor = false;
    [SerializeField] private BoosterButton _selectedShieldButton;
    
    private List<EAttackTypes> _availableAttackTypes = new();
    private ModalWarning _modalWarning;
    private ModalLose _modalLose;
    private ModalWin _modalWin;
    
    private Pedestal _pedestal;
    private Sprite _tokenImage;
    private CameraControl _camera;

    // Ресурсы игрока
    [SerializeField] private int _coins = 0;
    [SerializeField] private int _mallows = 0;
    [SerializeField] private int _rubies = 0;
    [SerializeField] private int _power = 2;

    // Эффекты
    [SerializeField] private int _effectsGreen = 0;
    [SerializeField] private int _effectsYellow = 0;
    [SerializeField] private int _effectsBlack = 0;
    [SerializeField] private int _effectsRed = 0;
    [SerializeField] private int _effectsStar = 0;
    private bool _isEffectPlaced;

    // Усилители
    [SerializeField] private int _boosterMagnet = 0;
    [SerializeField] private int _boosterSuperMagnet = 0;
    [SerializeField] private int _boosterLasso = 0;
    [SerializeField] private int _boosterShield = 0;
    [SerializeField] private int _boosterShieldIron = 0;
    [SerializeField] private int _boosterVampire = 0;

    // Кубик
    [SerializeField] private int _cubicMaxScore = 6;

    private void Awake() {
        _availableAttackTypes.Add(EAttackTypes.Usual);
        _availableAttackTypes.Add(EAttackTypes.MagicKick);
        _availableAttackTypes.Add(EAttackTypes.Knockout);
        _modalWarning = GameObject.Find("GameScripts").GetComponent<ModalWarning>();
        _modalLose = GameObject.Find("GameScripts").GetComponent<ModalLose>();
        _modalWin = GameObject.Find("GameScripts").GetComponent<ModalWin>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
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

    public bool IsLightning {
        get { return _isLightning; }
        private set {}
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
        private set {}
    }

    public int Mallows {
        get { return _mallows; }
        private set {}
    }

    public int Rubies {
        get { return _rubies; }
        private set {}
    }

    public int Power {
        get { return _power; }
        private set {}
    }

    public int EffectsGreen {
        get { return _effectsGreen; }
        set { _effectsGreen = value; }
    }

    public int EffectsYellow {
        get { return _effectsYellow; }
        set { _effectsYellow = value; }
    }

    public int EffectsRed {
        get { return _effectsRed; }
        set { _effectsRed = value; }
    }

    public int EffectsBlack {
        get { return _effectsBlack; }
        set { _effectsBlack = value; }
    }

    public int EffectsStar {
        get { return _effectsStar; }
        set { _effectsStar = value; }
    }

    public bool IsEffectPlaced {
        get { return _isEffectPlaced; }
        set { _isEffectPlaced = value; }
    }

    public int BoosterMagnet {
        get { return _boosterMagnet; }
        set { _boosterMagnet = value; }
    }

    public int BoosterSuperMagnet {
        get { return _boosterSuperMagnet; }
        set { _boosterSuperMagnet = value; }
    }

    public int BoosterLasso {
        get { return _boosterLasso; }
        set { _boosterLasso = value; }
    }

    public int BoosterShield {
        get { return _boosterShield; }
        set { _boosterShield = value; }
    }

    public int BoosterShieldIron {
        get { return _boosterShieldIron; }
        set { _boosterShieldIron = value; }
    }

    public int BoosterVampire {
        get { return _boosterVampire; }
        set { _boosterVampire = value; }
    }

    public int CubicMaxScore {
        get { return _cubicMaxScore; }
        set { _cubicMaxScore = value; }
    }

    public int Armor {
        get { return _armor; }
        set { _armor = value; }
    }

    public bool IsIronArmor {
        get { return _isIronArmor; }
        set { _isIronArmor = value; }
    }

    public BoosterButton SelectedShieldButton {
        get { return _selectedShieldButton; }
        set { _selectedShieldButton = value; }
    }

    // Подсчет бустеров

    public int GetMagnetsCount() {
        return BoosterMagnet + BoosterSuperMagnet;
    }

    public int GetShieldsCount() {
        return BoosterShield + BoosterShieldIron;
    }

    // Для массового изменения бустеров

    public void AddTheBooster(EBoosters booster, int value) {
        switch(booster) {
            case EBoosters.Lasso: {
                BoosterLasso += value;
                break;
            }
            case EBoosters.Magnet: {
                BoosterMagnet += value;
                break;
            }
            case EBoosters.MagnetSuper: {
                BoosterSuperMagnet += value;
                break;
            }
            case EBoosters.Shield: {
                BoosterShield += value;
                break;
            }
            case EBoosters.ShieldIron: {
                BoosterShieldIron += value;
                break;
            }
            case EBoosters.Vampire: {
                BoosterVampire += value;
                break;
            }
        }
    }

    // Собирает все бустеры, не включая особые

    public List<EBoosters> CollectAllRegularBoosters() {
        List<EBoosters> result = new();

        for (int i = 0; i < BoosterLasso; i++) {
            result.Add(EBoosters.Lasso);
        }

        for (int i = 0; i < BoosterMagnet; i++) {
            result.Add(EBoosters.Magnet);
        }

        for (int i = 0; i < BoosterSuperMagnet; i++) {
            result.Add(EBoosters.MagnetSuper);
        }

        for (int i = 0; i < BoosterShield; i++) {
            result.Add(EBoosters.Shield);
        }

        for (int i = 0; i < BoosterShieldIron; i++) {
            result.Add(EBoosters.ShieldIron);
        }

        for (int i = 0; i < BoosterVampire; i++) {
            result.Add(EBoosters.Vampire);
        }

        return result;
    }

    // Изменение параметров движения с помощью инкремента или декремента

    public void AddMovesToDo(int count) {
        _movesToDo += count;
    }

    public void AddStepsLeft(int count) {
        _stepsLeft += count;
    }

    public void SkipMoveIncrease() {
        _movesSkip++;
        GetTokenControl().UpdateSkips(_movesSkip);
    }

    public void SkipMoveDecrease(TokenControl token) {
        _movesSkip--;
        token.UpdateSkips(_movesSkip);
    }

    // Изменение ресурсов с помощью инкремента или декремента

    public void AddCoins(int value) {
        _coins += value;
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

    public void AddEffectGreen(int value) {
        _effectsGreen += value;
    }

    public void AddEffectYellow(int value) {
        _effectsYellow += value;
    }

    public void AddEffectBlack(int value) {
        _effectsBlack += value;
    }

    public void AddEffectRed(int value) {
        _effectsRed += value;
    }

    public void AddEffectStar(int value) {
        _effectsStar += value;
    }

    public void AddMagnets(int value) {
        _boosterMagnet += value;
    }

    public void AddMagnetsSuper(int value) {
        _boosterSuperMagnet += value;
    }

    public void AddLasso(int value) {
        _boosterLasso += value;
    }

    public void AddShield(int value) {
        _boosterShield += value;
    }

    public void AddShieldIron(int value) {
        _boosterShieldIron += value;
    }

    public void AddVampire(int value) {
        _boosterVampire += value;
    }

    public void AddArmor(int value) {
        _armor += value;
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

        CheckIsPlayerOutOfPower(this, () => StartCoroutine(MoveControl.Instance.EndMoveDefer()));
    }

    public void ExecuteAttackMagicKick(PlayerControl rival) {
        int powerSpend = Manual.Instance.AttackMagicKick.GetCost(1); // todo вычислять из уровня атаки
        AddPower(-powerSpend);
        PlayersControl.Instance.UpdatePlayersInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" ДАЁТ ПИНКА ", UIColors.Red) + Utils.Wrap(rival.PlayerName, UIColors.Yellow) + "!";
        Messages.Instance.AddMessage(message);

        int steps = Manual.Instance.AttackMagicKick.GetCauseEffect(1); // todo вычислять из уровня атаки
        MoveControl.Instance.MakeMagicKickMove(rival, rival.GetCurrentCell(), steps); 

        CheckIsPlayerOutOfPower(this);
    }

    public void ExecuteAttackVampire(PlayerControl rival) {
        AddPower(1);
        AddVampire(-1);
        AddMovesToDo(1);
        rival.AddPower(-1);
        rival.SkipMoveIncrease();
        PlayersControl.Instance.UpdatePlayersInfo();
        BoostersControl.Instance.UpdateBoostersFromPlayer(this);
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" КУСАЕТ ", UIColors.Red) + Utils.Wrap(rival.PlayerName, UIColors.Yellow) + " и забирает его силу!";
        Messages.Instance.AddMessage(message);

        CheckIsPlayerOutOfPower(rival, () => StartCoroutine(MoveControl.Instance.EndMoveDefer()));
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

        CheckIsPlayerOutOfPower(this);
    }

    public void ExecuteCancelAttack() {
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " отказался от атаки";
        Messages.Instance.AddMessage(message);
        StartCoroutine(MoveControl.Instance.EndMoveDefer());
    }

    // исполнение эффектов

    public void ExecuteBlackEffect() {
        CellControl cell = GetCurrentCell();
        TokenControl token = GetTokenControl();

        if (_armor > 0 && _isIronArmor) {
            OpenSavedByShieldModal(() => {
                CellChecker.Instance.CheckCellArrows(this);
            });
            return;
        }

        AddPower(-1);
        PlayersControl.Instance.UpdatePlayersInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("ЧЁРНЫЙ", UIColors.Black) + " эффект! Минус 1 сила";
        Messages.Instance.AddMessage(message);

        CheckIsPlayerOutOfPower(this, () => CellChecker.Instance.CheckCellArrows(this));
    }

    public void ExecuteRedEffect() {
        _movesToDo = 0;
        _stepsLeft = 0;

        if (_armor > 0 && _isIronArmor) {
            OpenSavedByShieldModal(() => {
                string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("КРАСНЫЙ", UIColors.Red) + " эффект! Возврат на чекпойнт";
                Messages.Instance.AddMessage(message);
                RedEffectTokenMove();
            });
            return;
        }

        AddPower(-1);
        PlayersControl.Instance.UpdatePlayersInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("КРАСНЫЙ", UIColors.Red) + " эффект! Минус 1 сила. Возврат на чекпойнт";
        Messages.Instance.AddMessage(message);

        CheckIsPlayerOutOfPower(this, RedEffectTokenMove, () => StartCoroutine(RedEffectTokenMoveDefer()));
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
        float moveTime = MoveControl.Instance.SpecifiedMoveTime;
        tokenControl.SetToSpecifiedCell(redCell.PenaltyCell, moveTime, () => {
            cellControl.RemoveToken(_tokenObject);
            MoveControl.Instance.ConfirmNewPosition();
        });
    }

    public void ExecuteStarEffect() {
        AddPower(1);
        PlayersControl.Instance.UpdatePlayersInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("звезду", UIColors.DarkBlue) + " и получает 1 силу";
        Messages.Instance.AddMessage(message);
    }

    public void ExecuteFinish() {
        if (IsMe()) {
            _modalWin.OpenWindow();
        }
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
        _camera.ClearFollow();
    }

    public void ExecuteHedgehogArrow(List<EBoosters> selectedBoosters) {
        foreach(EBoosters booster in selectedBoosters) {
            AddTheBooster(booster, -1);
        }
    }

    public void ExecuteHedgehogFinishPay(int cost) {
        AddCoins(-cost);
        PlayersControl.Instance.UpdatePlayersInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " платит дань " + Utils.Wrap("ежу", UIColors.DarkGreen);
        Messages.Instance.AddMessage(message);
        ExecuteFinish();
    }

    public void ExecuteHedgehogFinishFight() {
        AddPower(-3);
        PlayersControl.Instance.UpdatePlayersInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + Utils.Wrap(" ПОБИЛ ", UIColors.Red) + Utils.Wrap("ежа!", UIColors.DarkGreen) + " Финиш свободен";
        Messages.Instance.AddMessage(message);
        // todo нужно разбрасывать дань по полю
        ExecuteFinish();
    }

    public void ExecuteReplaceEffect(EControllableEffects effect) {
        ManualContent manual = Manual.Instance.GetEffectManual(effect);

        // todo уровень эффекта должен вычисляться из PlayerControl
        int effectLevel = 1;

        int cost = manual.GetCost(effectLevel);
        if (manual.CostResourceType == EResourceTypes.Power) {
            AddPower(-cost);
        } else {
            AddCoins(-cost);
        }
        PlayersControl.Instance.UpdatePlayersInfo();

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

        CheckIsPlayerOutOfPower(this);
    }

    public void ExecuteMoneybox(MoneyboxVault vault) {
        (int, int, int) bonus = vault.GetBonus();
        AddPower(bonus.Item1);
        AddCoins(bonus.Item2);
        AddRubies(bonus.Item3);
        PlayersControl.Instance.UpdatePlayersInfo();
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " забрал бонус из " + Utils.Wrap("копилки", UIColors.Green);
        Messages.Instance.AddMessage(message);
        _movesSkip++;

        MoveControl.Instance.CheckMoveSkipAndPreparePlayer();

        vault.SetNextStep();

        if (vault.IsOver) {
            string vaultMessage = Utils.Wrap("Копилка", UIColors.Green) + " исчерпана";
            Messages.Instance.AddMessage(vaultMessage);
        }
    }

    public void LeaveMoneybox(MoneyboxVault vault) {
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " покидает " + Utils.Wrap("копилку", UIColors.Green);
        Messages.Instance.AddMessage(message);
        _movesToDo++;

        MoveControl.Instance.PreparePlayerForMove();
        
        vault.ReassignPlayers();
    }

    // Молнии

    // При попадании на клетку с молнией

    public void ExecuteLightning() {
        _lightningMoves = 3;
        _isLightning = true;
        TokenControl token = GetTokenControl();
        token.AddIndicator(ETokenIndicators.Lightning, _lightningMoves.ToString());
        string message = Utils.Wrap(PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("молнию", UIColors.Green) + "! Очки на кубике x2";
        Messages.Instance.AddMessage(message);
    }

    // При подготовке игрока к ходу (в т.ч. дополнительному)

    public void CheckLightningStartMove() {
        if (_isLightning) {
            CubicControl.Instance.ModifiersControl.ShowModifierLightning(true);
        }
    }

    // Сразу после кидания кубика или пропуска хода

    public void SpendLightning() {
        if (_isLightning) {
            _lightningMoves--;
        }
    }

    // В конце хода

    public void CheckLightningEndMove() {
        CubicControl.Instance.ModifiersControl.ShowModifierLightning(false);

        if (!_isLightning) {
            return;
        }

        TokenControl token = GetTokenControl();
        if (_lightningMoves == 0) {
            token.RemoveIndicator(ETokenIndicators.Lightning);
            string message = "У " + Utils.Wrap(PlayerName, UIColors.Yellow) + " закончилась " + Utils.Wrap("молния", UIColors.Green);
            Messages.Instance.AddMessage(message);
            _isLightning = false;
        } else {
            token.UpdateIndicator(ETokenIndicators.Lightning, _lightningMoves.ToString());
        }
    }

    // Трата щитов

    public void SpendArmor() {
        if (_armor == 0) {
            return;
        }

        TokenControl token = GetTokenControl();
        bool isMe = IsMe();
        _armor--;

        if (_isIronArmor) {
            if (_armor == 0) {
                token.UpdateShield(EBoosters.None);
                AddShieldIron(-1);
                if (isMe) {
                   BoostersControl.Instance.DeactivateArmorButtons(); 
                }
                string message = Utils.Wrap("Железный щит ", UIColors.ArmorIron) + Utils.Wrap(PlayerName, UIColors.Yellow) + " пришел в негодность";
                Messages.Instance.AddMessage(message);
            } else if (isMe) {
                BoostersControl.Instance.UpdatePlayersArmorButtons(this);
            }
        } else {
            if (_armor == 0) {
                token.UpdateShield(EBoosters.None);
                AddShield(-1);
                if (isMe) {
                    BoostersControl.Instance.DeactivateArmorButtons();
                }
                string message = Utils.Wrap("Щит ", UIColors.Armor) + Utils.Wrap(PlayerName, UIColors.Yellow) + " пришел в негодность";
                Messages.Instance.AddMessage(message);
            } else if (isMe) {
                BoostersControl.Instance.UpdatePlayersArmorButtons(this);
            }
        }
    }

    public void HarvestShieldBonus(List<PlayerControl> rivals) {
        foreach(PlayerControl rival in rivals) {
            int coinBonus = rival.IsIronArmor ? 250 : 80;
            AddCoins(-coinBonus);
            rival.AddCoins(coinBonus);
        }
        PlayersControl.Instance.UpdatePlayersInfo();
    }

    // Разное

    public bool IsEnoughEffects(EControllableEffects effect) {
        switch(effect) {
            case EControllableEffects.Green: {
                return _effectsGreen > 0;
            }
            case EControllableEffects.Yellow: {
                return _effectsYellow > 0;
            }
            case EControllableEffects.Black: {
                return _effectsBlack > 0;
            }
            case EControllableEffects.Red: {
                return _effectsRed > 0;
            }
            default: return false;
        }
    }

    public TokenControl GetTokenControl() {
        return _tokenObject.GetComponent<TokenControl>();
    }

    public CellControl GetCurrentCell() {
        return GetTokenControl().GetCurrentCellControl();
    }

    public void OpenPowerWarningModal(Action callback = null) {
        _modalWarning.SetHeadingText("Предупреждение");
        _modalWarning.SetBodyText("Силы на нуле. Красная или чёрная клетки приведут к поражению!");
        _modalWarning.SetCallback(callback);
        _modalWarning.OpenWindow();
    }

    public void OpenSavedByShieldModal(Action callback = null) {
        _modalWarning.SetHeadingText("Железный щит");
        _modalWarning.SetBodyText("Благодаря <b>железному щиту</b> вы не теряете силу на этой клетке.");
        _modalWarning.SetCallback(callback);
        _modalWarning.OpenWindow();
    }

    public void OpenAttackShieldModal(Action callback = null) {
        _modalWarning.SetHeadingText("Соперник защищён");
        _modalWarning.SetBodyText("Вы не можете атаковать соперника со щитом.");
        _modalWarning.SetCallback(callback);
        _modalWarning.OpenWindow();
    }

    private void ConfirmLose() {
        if (IsMe()) {
            _modalLose.OpenWindow();
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

    private void CheckIsPlayerOutOfPower(PlayerControl player, Action callback1 = null, Action callback2 = null) {
        if (player.Power == 0 && player.IsMe()) {
            OpenPowerWarningModal(() => {
                callback1?.Invoke();
            });
            return;
        }

        if (player.Power < 0) {
            player.ConfirmLose();
            return;
        }

        if (callback2 != null) {
            callback2.Invoke();
        } else {
            callback1?.Invoke();
        }
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]

public class PlayerBoosters : MonoBehaviour
{
    private PlayerControl _player;
    [SerializeField] private int _magnet = 0;
    [SerializeField] private int _superMagnet = 0;
    [SerializeField] private int _lasso = 0;
    [SerializeField] private int _shield = 0;
    [SerializeField] private int _shieldIron = 0;
    [SerializeField] private int _vampire = 0;
    [SerializeField] private int _boombaster = 0;
    [SerializeField] private int _stuck = 0;
    [SerializeField] private int _trap = 0;
    [SerializeField] private int _flash = 0;
    [SerializeField] private int _blot = 0;
    [SerializeField] private int _vacuum = 0;
    [SerializeField] private int _vacuumNozzle = 0;
    [SerializeField] private int _mop = 0;
    [SerializeField] private int _tameLightning = 0;
    [SerializeField] private int _flashMovesLeft = 0; // сколько ходов осталось до выключения флешки
    [SerializeField] private int _blotMovesLeft = 0; // сколько ходов осталось до выключения кляксы
    [SerializeField] private int _armor = 0; // сколько ходов осталось со щитом (включая ходы соперников)
    [SerializeField] private bool _isIronArmor = false;
    [SerializeField] private BoosterButton _selectedShieldButton;
    private TopPanel _topPanel;
    private ModalMop _modalMop;

    private void Awake() {
        _player = GetComponent<PlayerControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _modalMop = GameObject.Find("ModalScripts").GetComponent<ModalMop>();
    }

    public int Magnet {
        get { return _magnet; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxMagnets);
            (int newMagnetCount, int newSuperMagnetCount) = AjustItemsQuantity(newValue, _superMagnet, BoostersControl.Instance.MaxMagnetsTotal);
            _magnet = newMagnetCount;
            _superMagnet = newSuperMagnetCount;
        }
    }

    public int SuperMagnet {
        get { return _superMagnet; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxSuperMagnets);
            (int newMagnetCount, int newSuperMagnetCount) = AjustItemsQuantity(_magnet, newValue, BoostersControl.Instance.MaxMagnetsTotal);
            _magnet = newMagnetCount;
            _superMagnet = newSuperMagnetCount;
        }
    }

    public int Lasso {
        get { return _lasso; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxLasso);
            _lasso = newValue;
        }
    }

    public int Shield {
        get { return _shield; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxShields);
            (int newShieldsCount, int newIronShieldsCount) = AjustItemsQuantity(newValue, _shieldIron, BoostersControl.Instance.MaxShieldsTotal);
            _shield = newShieldsCount;
            _shieldIron = newIronShieldsCount;
        }
    }

    public int ShieldIron {
        get { return _shieldIron; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxShieldsIron);
            (int newShieldsCount, int newIronShieldsCount) = AjustItemsQuantity(_shield, newValue, BoostersControl.Instance.MaxShieldsTotal);
            _shield = newShieldsCount;
            _shieldIron = newIronShieldsCount;
        }
    }

    public int Vampire {
        get { return _vampire; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxVampires);
            _vampire = newValue;
        }
    }

    public int Boombaster {
        get { return _boombaster; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxBoombasters);
            _boombaster = newValue;
        }
    }

    public int Stuck {
        get { return _stuck; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxStuck);
            _stuck = newValue;
        }
    }

    public int Trap {
        get { return _trap; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxTrap);
            _trap = newValue;
        }
    }

    public int Flash {
        get { return _flash; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxFlash);
            _flash = newValue;
        }
    }

    public int Blot {
        get { return _blot; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxBlot);
            _blot = newValue;
        }
    }

    public int Vacuum {
        get { return _vacuum; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxVacuum);
            _vacuum = newValue;
        }
    }

    public int VacuumNozzle {
        get { return _vacuumNozzle; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxVacuumNozzle);
            _vacuumNozzle = newValue;
        }
    }

    public int Mop {
        get { return _mop; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxMop);
            _mop = newValue;
        }
    }

    public int TameLightning {
        get { return _tameLightning; }
        set {
            int newValue = Math.Clamp(value, 0, BoostersControl.Instance.MaxTameLightning);
            _tameLightning = newValue;
        }
    }

    public int FlashMovesLeft {
        get { return _flashMovesLeft; }
        set {
            int newValue = Math.Clamp(value, 0, 1000);
            int oldValue = _flashMovesLeft;
            _flashMovesLeft = newValue;
            _player.GetTokenControl().UpdateIndicatorFlash(newValue);
        }
    }

    public int BlotMovesLeft {
        get { return _blotMovesLeft; }
        set {
            int newValue = Math.Clamp(value, 0, 1000);
            int oldValue = _blotMovesLeft;
            _blotMovesLeft = newValue;
            _player.GetTokenControl().UpdateIndicatorBlot(newValue);
        }
    }

    public bool IsBlot() {
        return _blotMovesLeft > 0;
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

    public void AddMagnets(int value) {
        Magnet += value;
    }

    public void AddMagnetsSuper(int value) {
        SuperMagnet += value;
    }

    public void AddLasso(int value) {
        Lasso += value;
    }

    public void AddShield(int value) {
        Shield += value;
    }

    public void AddShieldIron(int value) {
        ShieldIron += value;
    }

    public void AddVampire(int value) {
        Vampire += value;
    }

    public void AddBoombaster(int value) {
        Boombaster += value;
    }

    public void AddStuck(int value) {
        Stuck += value;
    }

    public void AddTrap(int value) {
        Trap += value;
    }

    public void AddFlash(int value) {
        Flash += value;
    }

    public void AddBlot(int value) {
        Blot += value;
    }

    public void AddVacuum(int value) {
        Vacuum += value;
    }

    public void AddVacuumNozzle(int value) {
        VacuumNozzle += value;
    }

    public void AddMop(int value) {
        Mop += value;
    }

    public void AddTameLightning(int value) {
        TameLightning += value;
    }

    public void AddFlashMovesLeft(int value) {
        FlashMovesLeft += value;
    }

    public void AddBlotMovesLeft(int value) {
        BlotMovesLeft += value;
    }

    public void AddArmor(int value) {
        Armor += value;
    }

    // Если магнитов или щитов больше, чем может вместить инвентарь, то уменьшить их количество до приемлимого

    private (int, int) AjustItemsQuantity(int count1, int count2, int max) {
        if (count1 + count2 <= max) {
            return (count1, count2);
        }

        int newCount1 = 0;
        int newCount2 = 0;
        int newTotal = 0;

        for (int i = 0; i < count1; i++) {
            if (newTotal == 3) {
                break;
            }
            newCount1++;
            newTotal++;
        }

        if (newTotal < 3) {
            for (int i = 0; i < count2; i++) {
                if (newTotal == 3) {
                    break;
                }
                newCount2++;
                newTotal = newCount1 + newCount2;
            }
        }

        return (newCount1, newCount2);
    }

    // Для массового изменения бустеров. Вернет false, если не хватает места

    public bool AddTheBooster(EBoosters booster, int value) {
        bool isSuccess = true;

        switch(booster) {
            case EBoosters.Lasso: {
                if (Lasso + value > BoostersControl.Instance.MaxLasso) {
                    isSuccess = false;
                }
                Lasso += value;
                break;
            }
            case EBoosters.Magnet: {
                if (Magnet + SuperMagnet + value > BoostersControl.Instance.MaxMagnets) {
                    isSuccess = false;
                }
                Magnet += value;
                break;
            }
            case EBoosters.MagnetSuper: {
                if (Magnet + SuperMagnet + value > BoostersControl.Instance.MaxMagnets) {
                    isSuccess = false;
                }
                SuperMagnet += value;
                break;
            }
            case EBoosters.Shield: {
                if (Shield + ShieldIron + value > BoostersControl.Instance.MaxShields) {
                    isSuccess = false;
                }
                Shield += value;
                break;
            }
            case EBoosters.ShieldIron: {
                if (Shield + ShieldIron + value > BoostersControl.Instance.MaxShields) {
                    isSuccess = false;
                }
                ShieldIron += value;
                break;
            }
            case EBoosters.Vampire: {
                if (Vampire + value > BoostersControl.Instance.MaxVampires) {
                    isSuccess = false;
                }
                Vampire += value;
                break;
            }
            case EBoosters.Boombaster: {
                if (Boombaster + value > BoostersControl.Instance.MaxBoombasters) {
                    isSuccess = false;
                }
                Boombaster += value;
                break;
            }
            case EBoosters.Trap: {
                if (Trap + value > BoostersControl.Instance.MaxTrap) {
                    isSuccess = false;
                }
                Trap += value;
                break;
            }
            case EBoosters.Stuck: {
                if (Stuck + value > BoostersControl.Instance.MaxStuck) {
                    isSuccess = false;
                }
                Stuck += value;
                break;
            }
            case EBoosters.Flash: {
                if (Flash + value > BoostersControl.Instance.MaxFlash) {
                    isSuccess = false;
                }
                Flash += value;
                break;
            }
            case EBoosters.Blot: {
                if (Blot + value > BoostersControl.Instance.MaxBlot) {
                    isSuccess = false;
                }
                Blot += value;
                break;
            }
            case EBoosters.Vacuum: {
                if (Vacuum + value > BoostersControl.Instance.MaxVacuum) {
                    isSuccess = false;
                }
                Vacuum += value;
                break;
            }
            case EBoosters.VacuumNozzle: {
                if (VacuumNozzle + value > BoostersControl.Instance.MaxVacuumNozzle) {
                    isSuccess = false;
                }
                VacuumNozzle += value;
                break;
            }
            case EBoosters.Mop: {
                if (Mop + value > BoostersControl.Instance.MaxMop) {
                    isSuccess = false;
                }
                Mop += value;
                break;
            }
            case EBoosters.TameLightning: {
                if (TameLightning + value > BoostersControl.Instance.MaxTameLightning) {
                    isSuccess = false;
                }
                TameLightning += value;
                break;
            }
        }

        if (value > 0) {
            AfterAddingBooster(booster, value, isSuccess);
        }

        return isSuccess;
    }

    private void AfterAddingBooster(EBoosters booster, int value, bool isSuccess) {
        if (isSuccess) {
            ManualContent manual = Manual.Instance.GetBoosterManual(booster);
            string bonusName = manual.GetEntityName(true);
            string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " подбирает усилитель: " + Utils.Wrap(bonusName, UIColors.Orange);
            Messages.Instance.AddMessage(message);
            BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
            _player.GetTokenControl().AddBonusEventToQueue("+" + value, new Color32(3,74,0,255), manual.Sprite);
        } else {
            string message = "У " + Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " не хватило места в инвентаре для усилителя";
            Messages.Instance.AddMessage(message);
        }
    }

    // Собирает все бустеры, не включая особые

    public List<EBoosters> CollectAllRegularBoosters() {
        List<EBoosters> result = new();

        for (int i = 0; i < Lasso; i++) {
            result.Add(EBoosters.Lasso);
        }

        for (int i = 0; i < Magnet; i++) {
            result.Add(EBoosters.Magnet);
        }

        for (int i = 0; i < SuperMagnet; i++) {
            result.Add(EBoosters.MagnetSuper);
        }

        for (int i = 0; i < Shield; i++) {
            result.Add(EBoosters.Shield);
        }

        for (int i = 0; i < ShieldIron; i++) {
            result.Add(EBoosters.ShieldIron);
        }

        for (int i = 0; i < Vampire; i++) {
            result.Add(EBoosters.Vampire);
        }

        for (int i = 0; i < Trap; i++) {
            result.Add(EBoosters.Trap);
        }

        for (int i = 0; i < Stuck; i++) {
            result.Add(EBoosters.Stuck);
        }

        for (int i = 0; i < Boombaster; i++) {
            result.Add(EBoosters.Boombaster);
        }

        for (int i = 0; i < Flash; i++) {
            result.Add(EBoosters.Flash);
        }

        for (int i = 0; i < Blot; i++) {
            result.Add(EBoosters.Blot);
        }

        for (int i = 0; i < Vacuum; i++) {
            result.Add(EBoosters.Vacuum);
        }

        for (int i = 0; i < VacuumNozzle; i++) {
            result.Add(EBoosters.VacuumNozzle);
        }

        return result;
    }

    // Трата щитов

    public void SpendArmor() {
        if (_armor == 0) {
            return;
        }

        TokenControl token = _player.GetTokenControl();
        bool isMe = _player.IsMe();
        AddArmor(-1);

        if (_isIronArmor) {
            if (_armor == 0) {
                token.UpdateShield(EBoosters.None);
                AddShieldIron(-1);
                if (isMe) {
                   BoostersControl.Instance.DeactivateArmorButtons(); 
                }
                string message = Utils.Wrap("Железный щит ", UIColors.ArmorIron) + Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " пришел в негодность";
                Messages.Instance.AddMessage(message);
            } else if (isMe) {
                BoostersControl.Instance.UpdatePlayersArmorButtons(_player);
            }
        } else {
            if (_armor == 0) {
                token.UpdateShield(EBoosters.None);
                AddShield(-1);
                if (isMe) {
                    BoostersControl.Instance.DeactivateArmorButtons();
                }
                string message = Utils.Wrap("Щит ", UIColors.Armor) + Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " пришел в негодность";
                Messages.Instance.AddMessage(message);
            } else if (isMe) {
                BoostersControl.Instance.UpdatePlayersArmorButtons(_player);
            }
        }
    }

    public void HarvestShieldBonus(List<PlayerControl> rivals) {
        foreach(PlayerControl rival in rivals) {
            int coinBonus = rival.Boosters.IsIronArmor ? 250 : 80;
            _player.AddCoins(-coinBonus);
            rival.AddCoins(coinBonus);
        }
        PlayersControl.Instance.UpdatePlayersInfo();
    }

    // Вычислить и вернуть индекс первой попавшейся кнопки щита, годной для отображения оверлея щита

    public int GetActualBoosterButtonIndex() {
        int firstIndex;

        if (IsIronArmor) {
            firstIndex = Shield + ShieldIron - 1;
        } else {
            firstIndex = Shield - 1;
        }

        return firstIndex;
    }

    // Исполнение усилителей

    // Бумка

    public void ExecuteBoombasterAsVictim(int powerPenalty) {
        if (powerPenalty == 0) {
            return;
        }
        
        string resultText;
        if (Armor > 0) {
            resultText = "Щит слетает";
        } else {
            string powerText = powerPenalty == 1 ? " сила" : " силы";
            resultText = "-" + powerPenalty + powerText;
        }
        
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " задело " + Utils.Wrap("БУМКОЙ! ", UIColors.Red) + resultText;
        Messages.Instance.AddMessage(message);

        TokenControl token = _player.GetTokenControl();
        bool isMe = _player.IsMe();

        if (Armor > 0) {
            Armor = 0;
            token.UpdateShield(EBoosters.None);
            if (IsIronArmor) {
                AddShieldIron(-1);
                if (isMe) {
                   BoostersControl.Instance.DeactivateArmorButtons(); 
                }
            } else {
                AddShield(-1);
                if (isMe) {
                    BoostersControl.Instance.DeactivateArmorButtons();
                }
            }
        } else {
            _player.AddPower(-powerPenalty);
            if (_player.Power == 0 && _player.IsMe()) {
                _player.OpenPowerWarningModal();
            }
            PlayersControl.Instance.UpdatePlayersInfo();
        }
    }

    public void ExecuteBoombaster() {
        CellControl targetCell = _player.GetCurrentCell();

        if (targetCell.IsBoombaster) {
            _player.OpenBoombasterModal();
            return;
        }
        
        CellsControl.Instance.AddBoombaster(targetCell, _player.Grind.Boombaster);
        AddBoombaster(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " устанавливает " + Utils.Wrap("бумку", UIColors.DarkYellow) + " на клетке №" + targetCell.NameDisplay;
        Messages.Instance.AddMessage(message);
    }

    public void ExecuteFlash() {
        int level = _player.Grind.Flash;
        ManualContent manual = Manual.Instance.BoosterFlash;
        int steps = manual.GetCauseEffect(level);

        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            if (player.IsFinished) {
                continue;
            }

            if (player != _player) {
                player.Boosters.FlashMovesLeft = steps;
            }
        }

        _player.Boosters.AddFlash(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);

        string stepsText = steps < 5 ? " хода" : " ходов";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " взорвал " + Utils.Wrap("флешку.", UIColors.Blue) + " Соперники остались без усилителей на " + steps + stepsText + "!";
        Messages.Instance.AddMessage(message);
    }

    // Клякса

    public void ExecuteBlot() {
        int level = _player.Grind.Blot;
        ManualContent manual = Manual.Instance.BoosterBlot;
        int steps = manual.GetCauseEffect(level);

        string stepsText = steps < 5 ? " хода" : " ходов";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " выпустил " + Utils.Wrap("кляксу.", UIColors.Black) + " Соперники остались без бонусов на " + steps + stepsText + "!";
        Messages.Instance.AddMessage(message);

        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            if (player.IsFinished) {
                continue;
            }

            if (player != _player) {
                if (player.IsAbilitySoap) {
                    string soapMessage = "У " + Utils.Wrap(player.PlayerName, UIColors.Yellow) + " есть " + Utils.Wrap("мыло!", UIColors.Pink) + Utils.Wrap(" Клякса", UIColors.Black) + " не сработала";
                    Messages.Instance.AddMessage(soapMessage);
                } else {
                    player.Boosters.BlotMovesLeft = steps;
                }
            }
        }

        _player.Boosters.AddBlot(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
    }

    public void ExecuteBlotAsVictim(string bonusText) {
        string message = Utils.Wrap("Клякса", UIColors.Black) + " помешала " + Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " " + bonusText;
        Messages.Instance.AddMessage(message);
    }

    // Прилипала

    public void ExecuteStuck() {
        AddStuck(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
        string message = "Обнаружено заражение новой " + Utils.Wrap("прилипалой", UIColors.LimeGreen);
        Messages.Instance.AddMessage(message);
    }

    // Мыло

    public void ExecuteSoap() {
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " получает " + Utils.Wrap("мыло!", UIColors.Pink) + Utils.Wrap(" Кляксы", UIColors.Black) + " и " + Utils.Wrap("прилипалы", UIColors.LimeGreen) + " больше не страшны";
        Messages.Instance.AddMessage(message);
        BlotMovesLeft = 0;
        _player.StuckAttached = 0;
    }

    // Лассо

    public void ExecuteLasso(CellControl targetCell) {
        foreach(CellControl cell in CellsControl.Instance.AllCellsControls) {
            cell.TurnOffLassoMode();
        }
        _topPanel.CloseWindow();
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " решает прокатиться на " + Utils.Wrap("лассо", UIColors.Orange);
        Messages.Instance.AddMessage(message);
        AddLasso(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
        BoostersControl.Instance.UnselectAllButtons();
        MoveControl.Instance.MakeLassoMove(targetCell);
    }

    // Щит

    public void ExecuteShield(bool _isIron) {
        TokenControl token = _player.GetTokenControl();
        string shieldText;

        if (_isIron) {
            IsIronArmor = true;
            Armor = 12;
            token.UpdateShield(EBoosters.ShieldIron);
            shieldText = Utils.Wrap("железный щит", UIColors.ArmorIron);
        } else {
            IsIronArmor = false;
            Armor = 4;
            token.UpdateShield(EBoosters.Shield);
            shieldText = Utils.Wrap("щит", UIColors.Armor);
        }

        BoostersControl.Instance.UpdatePlayersArmorButtons(_player);

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " надевает " + shieldText;
        Messages.Instance.AddMessage(message);
    }

    // Капкан

    public void ExecuteTrap(CellControl targetCell) {
        CellsControl.Instance.TurnOffTrapPlacementMode();
        targetCell.PlaceTrap(true, _player);
        _topPanel.CloseWindow();
        AddTrap(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
        BoostersControl.Instance.UnselectAllButtons();
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " ставит " + Utils.Wrap("капкан", UIColors.Orange) + " на клетке № " + targetCell.NameDisplay;
        Messages.Instance.AddMessage(message);
        StartCoroutine(ExecuteTrapDefer());
    }

    private IEnumerator ExecuteTrapDefer() {
        yield return new WaitForSeconds(CellsControl.Instance.ChangingEffectDelay);
        EffectsControl.Instance.RestoreCamera();
        EffectsControl.Instance.TryToEnableAllEffectButtons();
        BoostersControl.Instance.EnableAllButtons();
        CubicControl.Instance.SetCubicInteractable(true);
    }

    // Пылесос

    public void ExecuteVacuum(bool isNozzle, int steps) {
        if (isNozzle) {
            AddVacuumNozzle(-1);
        } else {
            AddVacuum(-1);
        }
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
        string vacuumText = isNozzle ? Utils.Wrap("пылесос с клапаном!", UIColors.Orange) : Utils.Wrap("пылесос!", UIColors.Yellow);
        string cellsText = steps == 1 ? " клетку" : (steps > 1 && steps < 5 ? " клетки" : " клеток");
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " достал из кладовки " + vacuumText + " Все соперники едут назад на " + steps + cellsText;
        Messages.Instance.AddMessage(message);

        BoostersControl.Instance.DisableAllButtons();
        EffectsControl.Instance.DisableAllButtons(true);
        CubicControl.Instance.SetCubicInteractable(false);

        StartCoroutine(MakeViolateMoveSeriesDefer(steps));
    }

    private IEnumerator MakeViolateMoveSeriesDefer(int steps) {
        yield return new WaitForSeconds(BoostersControl.Instance.ExecuteVacuumDelay);
        MoveControl.Instance.MakeViolateMoveSeries(steps);
    }

    // Швабра

    public void TryExecuteMop(CellControl targetCell) {
        int elementsCount = 0;
        EMopOptions option = EMopOptions.None;

        if (targetCell.Effect != EControllableEffects.None) {
            elementsCount++;
            option = EMopOptions.Effect;
        }

        if (targetCell.WhosTrap != null) {
            elementsCount++;
            option = EMopOptions.Trap;
        }

        if (targetCell.IsBoombaster) {
            elementsCount++;
            option = EMopOptions.Boombaster;
        }

        if (elementsCount < 2) {
            _topPanel.CloseWindow();
            ExecuteMop(targetCell, option);
        } else {
            _modalMop.BuildContent(targetCell);
            _modalMop.OpenModal();
        }
    }

    public void ExecuteMop(CellControl targetCell, EMopOptions options) {
        BoostersControl.Instance.ExitMopMode();

        if (options == EMopOptions.Effect && targetCell.EffectLevel == 3) {
            _player.OpenMopWarningModal(() => {
                BoostersControl.Instance.ExitMopModePhase2();
            });
            return;
        }

        AddMop(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
        string itemText;

        switch(options) {
            case EMopOptions.Effect: {
                ManualContent manual = Manual.Instance.GetEffectManual(targetCell.Effect);
                itemText = manual.GetEntityName(true);
                targetCell.RemoveEffectByMop();
                break;
            }
            case EMopOptions.Trap: {
                targetCell.RemoveTrapByMop();
                itemText = "капкан";
                break;
            }
            case EMopOptions.Boombaster: {
                targetCell.RemoveBoombasterByMop();
                itemText = "бумку";
                break;
            }
            default: {
                itemText = "";
                break;
            }
        }

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + Utils.Wrap(" удаляет ", UIColors.Red) + itemText + " на клетке №" + targetCell.NameDisplay;
        Messages.Instance.AddMessage(message);

        StartCoroutine(ExitMopModeDefer());
    }

    private IEnumerator ExitMopModeDefer() {
        yield return new WaitForSeconds(CellsControl.Instance.ChangingEffectDelay);
        BoostersControl.Instance.ExitMopModePhase2();
    }

    // Ручная молния

    public void ExecuteTameLightning() {
        AddTameLightning(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
        _player.Effects.ExecuteLightning(true);
        _player.Effects.CheckLightningStartMove();
    }
}

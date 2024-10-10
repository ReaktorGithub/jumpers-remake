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
    [SerializeField] private int _flashMovesLeft = 0; // сколько ходов осталось до выключения флешки
    [SerializeField] private int _blotMovesLeft = 0; // сколько ходов осталось до выключения кляксы
    [SerializeField] private int _armor = 0; // сколько ходов осталось со щитом (включая ходы соперников)
    [SerializeField] private bool _isIronArmor = false;
    [SerializeField] private BoosterButton _selectedShieldButton;
    private TopPanel _topPanel;

    private void Awake() {
        _player = GetComponent<PlayerControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
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

    public int FlashMovesLeft {
        get { return _flashMovesLeft; }
        set {
            int newValue = Math.Clamp(value, 0, 1000);
            int oldValue = _flashMovesLeft;
            _flashMovesLeft = newValue;

            if (newValue > 0 && oldValue > 0) {
                _player.GetTokenControl().UpdateIndicator(ETokenIndicators.Flash, newValue.ToString());
            } else if (newValue > 0) {
                _player.GetTokenControl().AddIndicator(ETokenIndicators.Flash, newValue.ToString());
            } else {
                _player.GetTokenControl().RemoveIndicator(ETokenIndicators.Flash);
            }
        }
    }

    public int BlotMovesLeft {
        get { return _blotMovesLeft; }
        set {
            int newValue = Math.Clamp(value, 0, 1000);
            int oldValue = _blotMovesLeft;
            _blotMovesLeft = newValue;

            if (newValue > 0 && oldValue > 0) {
                _player.GetTokenControl().UpdateIndicator(ETokenIndicators.Blot, newValue.ToString());
            } else if (newValue > 0) {
                _player.GetTokenControl().AddIndicator(ETokenIndicators.Blot, newValue.ToString());
            } else {
                _player.GetTokenControl().RemoveIndicator(ETokenIndicators.Blot);
            }
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

    // Для массового изменения бустеров

    public void AddTheBooster(EBoosters booster, int value) {
        switch(booster) {
            case EBoosters.Lasso: {
                Lasso += value;
                break;
            }
            case EBoosters.Magnet: {
                Magnet += value;
                break;
            }
            case EBoosters.MagnetSuper: {
                SuperMagnet += value;
                break;
            }
            case EBoosters.Shield: {
                Shield += value;
                break;
            }
            case EBoosters.ShieldIron: {
                ShieldIron += value;
                break;
            }
            case EBoosters.Vampire: {
                Vampire += value;
                break;
            }
            case EBoosters.Boombaster: {
                Boombaster += value;
                break;
            }
            case EBoosters.Trap: {
                Trap += value;
                break;
            }
            case EBoosters.Stuck: {
                Stuck += value;
                break;
            }
            case EBoosters.Flash: {
                Flash += value;
                break;
            }
            case EBoosters.Blot: {
                Blot += value;
                break;
            }
            case EBoosters.Vacuum: {
                Vacuum += value;
                break;
            }
            case EBoosters.VacuumNozzle: {
                VacuumNozzle += value;
                break;
            }
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

        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            if (player.IsFinished) {
                continue;
            }

            if (player != _player) {
                player.Boosters.BlotMovesLeft = steps;
            }
        }

        _player.Boosters.AddBlot(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);

        string stepsText = steps < 5 ? " хода" : " ходов";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " выпустил " + Utils.Wrap("кляксу.", UIColors.Black) + " Соперники остались без бонусов на " + steps + stepsText + "!";
        Messages.Instance.AddMessage(message);
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
        targetCell.PlaceTrap(_player);
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
}

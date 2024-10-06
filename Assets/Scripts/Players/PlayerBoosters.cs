using System;
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

    [SerializeField] private int _armor = 0; // сколько ходов осталось со щитом (включая ходы соперников)
    [SerializeField] private bool _isIronArmor = false;
    [SerializeField] private BoosterButton _selectedShieldButton;

    private void Awake() {
        _player = GetComponent<PlayerControl>();
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
            case EBoosters.Trap: {
                Trap += value;
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

        return result;
    }

    // Щиты

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

    // Бумка

    public void ExecuteBoombaster(int powerPenalty) {
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

    // Прилипала

    public void ExecuteStuckAsAgressor() {
        AddStuck(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
        string message = "Обнаружено заражение новой " + Utils.Wrap("прилипалой", UIColors.LimeGreen);
        Messages.Instance.AddMessage(message);
    }

    // Капкан

    public void ExecuteTrapAsAgressor(CellControl cell) {
        AddTrap(-1);
        BoostersControl.Instance.UpdateBoostersFromPlayer(_player);
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " ставит " + Utils.Wrap("капкан", UIColors.Orange) + " на клетке № " + cell.NameDisplay;
        Messages.Instance.AddMessage(message);
    }
}

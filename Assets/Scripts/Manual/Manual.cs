using UnityEngine;

public class Manual : MonoBehaviour
{
    public static Manual Instance { get; private set; }
    private ManualContent _power, _mallow, _ruby, _greenEffect, _yellowEffect, _blackEffect, _redEffect, _coins, _starEffect, _teleportEffect, _lightningEffect, _attackUsual, _attackMagicKick, _attackVampire, _attackKnockout,
    _boosterLasso, _boosterMagnet, _boosterSuperMagnet, _boosterShield, _boosterIronShield, _boosterVampire, _boosterBoombaster, _boosterStuck, _boosterFlash, _boosterTrap, _boosterBlot, _boosterVacuum, _boosterVacuumNozzle, _boosterMop, _boosterTameLightning,
    _abilityLastChance, _abilityOreol, _abilityHammer, _abilitySoap;

    private void Awake() {
        Instance = this;
        _power = transform.Find("Power").GetComponent<ManualContent>();
        _coins = transform.Find("Coins").GetComponent<ManualContent>();
        _ruby = transform.Find("Ruby").GetComponent<ManualContent>();
        _mallow = transform.Find("Mallow").GetComponent<ManualContent>();
        _greenEffect = transform.Find("GreenEffect").GetComponent<ManualContent>();
        _yellowEffect = transform.Find("YellowEffect").GetComponent<ManualContent>();
        _blackEffect = transform.Find("BlackEffect").GetComponent<ManualContent>();
        _redEffect = transform.Find("RedEffect").GetComponent<ManualContent>();
        _starEffect = transform.Find("StarEffect").GetComponent<ManualContent>();
        _teleportEffect = transform.Find("TeleportEffect").GetComponent<ManualContent>();
        _lightningEffect = transform.Find("LightningEffect").GetComponent<ManualContent>();
        _attackUsual = transform.Find("AttackUsual").GetComponent<ManualContent>();
        _attackMagicKick = transform.Find("AttackMagicKick").GetComponent<ManualContent>();
        _attackVampire = transform.Find("AttackVampire").GetComponent<ManualContent>();
        _attackKnockout = transform.Find("AttackKnockout").GetComponent<ManualContent>();
        _boosterLasso = transform.Find("BoosterLasso").GetComponent<ManualContent>();
        _boosterMagnet = transform.Find("BoosterMagnet").GetComponent<ManualContent>();
        _boosterSuperMagnet = transform.Find("BoosterSuperMagnet").GetComponent<ManualContent>();
        _boosterShield = transform.Find("BoosterShield").GetComponent<ManualContent>();
        _boosterIronShield = transform.Find("BoosterIronShield").GetComponent<ManualContent>();
        _boosterVampire = transform.Find("BoosterVampire").GetComponent<ManualContent>();
        _boosterBoombaster = transform.Find("BoosterBoombaster").GetComponent<ManualContent>();
        _boosterStuck = transform.Find("BoosterStuck").GetComponent<ManualContent>();
        _boosterFlash = transform.Find("BoosterFlash").GetComponent<ManualContent>();
        _boosterTrap = transform.Find("BoosterTrap").GetComponent<ManualContent>();
        _boosterBlot = transform.Find("BoosterBlot").GetComponent<ManualContent>();
        _boosterVacuum = transform.Find("BoosterVacuum").GetComponent<ManualContent>();
        _boosterVacuumNozzle = transform.Find("BoosterVacuumNozzle").GetComponent<ManualContent>();
        _boosterMop = transform.Find("BoosterMop").GetComponent<ManualContent>();
        _boosterTameLightning = transform.Find("BoosterTameLightning").GetComponent<ManualContent>();
        _abilityLastChance = transform.Find("AbilityLastChance").GetComponent<ManualContent>();
        _abilityOreol = transform.Find("AbilityOreol").GetComponent<ManualContent>();
        _abilityHammer = transform.Find("AbilityHammer").GetComponent<ManualContent>();
        _abilitySoap = transform.Find("AbilitySoap").GetComponent<ManualContent>();
    }

    public ManualContent Power {
        get { return _power; }
        private set {}
    }

    public ManualContent GreenEffect {
        get { return _greenEffect; }
        private set {}
    }

    public ManualContent YellowEffect {
        get { return _yellowEffect; }
        private set {}
    }

    public ManualContent BlackEffect {
        get { return _blackEffect; }
        private set {}
    }

    public ManualContent RedEffect {
        get { return _redEffect; }
        private set {}
    }

    public ManualContent StarEffect {
        get { return _starEffect; }
        private set {}
    }

    public ManualContent TeleportEffect {
        get { return _teleportEffect; }
        private set {}
    }

    public ManualContent LightningEffect {
        get { return _lightningEffect; }
        private set {}
    }

    public ManualContent Coins {
        get { return _coins; }
        private set {}
    }

    public ManualContent Ruby {
        get { return _ruby; }
        private set {}
    }

    public ManualContent Mallow {
        get { return _mallow; }
        private set {}
    }

    public ManualContent AttackUsual {
        get { return _attackUsual; }
        private set {}
    }

    public ManualContent AttackMagicKick {
        get { return _attackMagicKick; }
        private set {}
    }

    public ManualContent AttackVampire {
        get { return _attackVampire; }
        private set {}
    }

    public ManualContent AttackKnockout {
        get { return _attackKnockout; }
        private set {}
    }

    public ManualContent BoosterLasso {
        get { return _boosterLasso; }
        private set {}
    }

    public ManualContent BoosterMagnet {
        get { return _boosterMagnet; }
        private set {}
    }

    public ManualContent BoosterSuperMagnet {
        get { return _boosterSuperMagnet; }
        private set {}
    }

    public ManualContent BoosterShield {
        get { return _boosterShield; }
        private set {}
    }

    public ManualContent BoosterIronShield {
        get { return _boosterIronShield; }
        private set {}
    }

    public ManualContent BoosterVampire {
        get { return _boosterVampire; }
        private set {}
    }

    public ManualContent BoosterBoombaster {
        get { return _boosterBoombaster; }
        private set {}
    }

    public ManualContent BoosterStuck {
        get { return _boosterStuck; }
        private set {}
    }

    public ManualContent BoosterFlash {
        get { return _boosterFlash; }
        private set {}
    }

    public ManualContent BoosterTrap {
        get { return _boosterTrap; }
        private set {}
    }

    public ManualContent BoosterBlot {
        get { return _boosterBlot; }
        private set {}
    }

    public ManualContent BoosterVacuum {
        get { return _boosterVacuum; }
        private set {}
    }

    public ManualContent BoosterVacuumNozzle {
        get { return _boosterVacuumNozzle; }
        private set {}
    }

    public ManualContent BoosterMop {
        get { return _boosterMop; }
        private set {}
    }

    public ManualContent BoosterTameLightning {
        get { return _boosterTameLightning; }
        private set {}
    }

    public ManualContent AbilityHammer {
        get { return _abilityHammer; }
        private set {}
    }

    public ManualContent AbilityLastChance {
        get { return _abilityLastChance; }
        private set {}
    }

    public ManualContent AbilityOreol {
        get { return _abilityOreol; }
        private set {}
    }

    public ManualContent AbilitySoap {
        get { return _abilitySoap; }
        private set {}
    }

    public ManualContent GetEffectManual(EControllableEffects effect) {
        switch(effect) {
            case EControllableEffects.Green: {
                return GreenEffect;
            }
            case EControllableEffects.Yellow: {
                return YellowEffect;
            }
            case EControllableEffects.Black: {
                return BlackEffect;
            }
            case EControllableEffects.Red: {
                return RedEffect;
            }
            default: {
                return StarEffect;
            }
        }
    }

    public EResourceCharacters GetEffectCharacter(EControllableEffects effect) {
        switch(effect) {
            case EControllableEffects.Green: {
                return GreenEffect.Character;
            }
            case EControllableEffects.Yellow: {
                return YellowEffect.Character;
            }
            case EControllableEffects.Black: {
                return BlackEffect.Character;
            }
            case EControllableEffects.Star: {
                return StarEffect.Character;
            }
            default: {
                return RedEffect.Character;
            }
        }
    }

    public ManualContent GetBoosterManual(EBoosters booster) {
        switch(booster) {
            case EBoosters.Magnet: {
                return BoosterMagnet;
            }
            case EBoosters.MagnetSuper: {
                return BoosterSuperMagnet;
            }
            case EBoosters.Lasso: {
                return BoosterLasso;
            }
            case EBoosters.Shield: {
                return BoosterShield;
            }
            case EBoosters.ShieldIron: {
                return BoosterIronShield;
            }
            case EBoosters.Vampire: {
                return BoosterVampire;
            }
            case EBoosters.Boombaster: {
                return BoosterBoombaster;
            }
            case EBoosters.Stuck: {
                return BoosterStuck;
            }
            case EBoosters.Flash: {
                return BoosterFlash;
            }
            case EBoosters.Trap: {
                return BoosterTrap;
            }
            case EBoosters.Blot: {
                return BoosterBlot;
            }
            case EBoosters.Vacuum: {
                return BoosterVacuum;
            }
            case EBoosters.VacuumNozzle: {
                return BoosterVacuumNozzle;
            }
            case EBoosters.Mop: {
                return BoosterMop;
            }
            case EBoosters.TameLightning: {
                return BoosterTameLightning;
            }
            default: return null;
        }
    }

    public ManualContent GetEffectManualBySurprise(ESurprise surprise) {
        switch(surprise) {
            case ESurprise.Green: {
                return GreenEffect;
            }
            case ESurprise.Yellow: {
                return YellowEffect;
            }
            case ESurprise.Black: {
                return BlackEffect;
            }
            case ESurprise.Star: {
                return StarEffect;
            }
            case ESurprise.Red: {
                return RedEffect;
            }
            case ESurprise.Teleport: {
                return TeleportEffect;
            }
            case ESurprise.Lightning: {
                return LightningEffect;
            }
            default: return null;
        }
    }
}

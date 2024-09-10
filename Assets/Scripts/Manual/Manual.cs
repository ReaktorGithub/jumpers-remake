using UnityEngine;

public class Manual : MonoBehaviour
{
    public static Manual Instance { get; private set; }
    private ManualContent _power, _greenEffect, _yellowEffect, _blackEffect, _redEffect, _coins, _starEffect, _attackUsual, _attackMagicKick, _attackVampire, _attackKnockout;

    private void Awake() {
        Instance = this;
        _power = transform.Find("Power").GetComponent<ManualContent>();
        _coins = transform.Find("Coins").GetComponent<ManualContent>();
        _greenEffect = transform.Find("GreenEffect").GetComponent<ManualContent>();
        _yellowEffect = transform.Find("YellowEffect").GetComponent<ManualContent>();
        _blackEffect = transform.Find("BlackEffect").GetComponent<ManualContent>();
        _redEffect = transform.Find("RedEffect").GetComponent<ManualContent>();
        _starEffect = transform.Find("StarEffect").GetComponent<ManualContent>();
        _attackUsual = transform.Find("AttackUsual").GetComponent<ManualContent>();
        _attackMagicKick = transform.Find("AttackMagicKick").GetComponent<ManualContent>();
        _attackVampire = transform.Find("AttackVampire").GetComponent<ManualContent>();
        _attackKnockout = transform.Find("AttackKnockout").GetComponent<ManualContent>();
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

    public ManualContent Coins {
        get { return _coins; }
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
}

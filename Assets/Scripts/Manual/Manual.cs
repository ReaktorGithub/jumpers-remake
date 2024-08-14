using UnityEngine;

public class Manual : MonoBehaviour
{
    public static Manual Instance { get; private set; }
    private ManualContent _power;
    private ManualContent _greenEffect;
    private ManualContent _yellowEffect;
    private ManualContent _blackEffect;
    private ManualContent _redEffect;
    private ManualContent _coins;

    private void Awake() {
        Instance = this;
        _power = transform.Find("Power").GetComponent<ManualContent>();
        _coins = transform.Find("Coins").GetComponent<ManualContent>();
        _greenEffect = transform.Find("GreenEffect").GetComponent<ManualContent>();
        _yellowEffect = transform.Find("YellowEffect").GetComponent<ManualContent>();
        _blackEffect = transform.Find("BlackEffect").GetComponent<ManualContent>();
        _redEffect = transform.Find("RedEffect").GetComponent<ManualContent>();
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

    public ManualContent Coins {
        get { return _coins; }
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
            default: {
                return RedEffect;
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
            default: {
                return RedEffect.Character;
            }
        }
    }
}

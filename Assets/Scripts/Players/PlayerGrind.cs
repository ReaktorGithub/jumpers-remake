using UnityEngine;

[RequireComponent(typeof(PlayerControl))]
public class PlayerGrind : MonoBehaviour
{
    [SerializeField] private int _green = 1;
    [SerializeField] private int _yellow = 1;
    [SerializeField] private int _red = 1;
    [SerializeField] private int _black = 1;
    [SerializeField] private int _star = 1;
    [SerializeField] private int _magicKick = 1;
    [SerializeField] private int _oreol = 1;
    [SerializeField] private int _knockout = 1;
    [SerializeField] private int _lasso = 1;
    [SerializeField] private int _boombaster = 1;
    [SerializeField] private int _flash = 1;
    [SerializeField] private int _blot = 1;
    [SerializeField] private int _mop = 1;
    [SerializeField] private int _cubic = 1;
    [SerializeField] private int _insurance = 1;

    public int Green {
        get { return _green; }
        private set {}
    }

    public int Yellow {
        get { return _yellow; }
        private set {}
    }

    public int Red {
        get { return _red; }
        private set {}
    }

    public int Black {
        get { return _black; }
        private set {}
    }

    public int Star {
        get { return _star; }
        private set {}
    }

    public int MagicKick {
        get { return _magicKick; }
        private set {}
    }

    public int Oreol {
        get { return _oreol; }
        private set {}
    }

    public int Knockout {
        get { return _knockout; }
        private set {}
    }

    public int Lasso {
        get { return _lasso; }
        private set {}
    }

    public int Boombaster {
        get { return _boombaster; }
        private set {}
    }

    public int Flash {
        get { return _flash; }
        private set {}
    }

    public int Blot {
        get { return _blot; }
        private set {}
    }

    public int Mop {
        get { return _mop; }
        private set {}
    }

    public int Cubic {
        get { return _cubic; }
        private set {}
    }

    public int Insurance {
        get { return _insurance; }
        private set {}
    }

    public void IncreaseGreen() {
        _green++;
    }

    public void IncreaseYellow() {
        _yellow++;
    }

    public void IncreaseRed() {
        _red++;
    }

    public void IncreaseBlack() {
        _black++;
    }

    public void IncreaseStar() {
        _star++;
    }

    public void IncreaseMagicKick() {
        _magicKick++;
    }

    public void IncreaseOreol() {
        _oreol++;
    }

    public void IncreaseKnockout() {
        _knockout++;
    }

    public void IncreaseLasso() {
        _lasso++;
    }

    public void IncreaseBoombaster() {
        _boombaster++;
    }

    public void IncreaseFlash() {
        _flash++;
    }

    public void IncreaseBlot() {
        _blot++;
    }

    public void IncreaseMop() {
        _mop++;
    }

    public void IncreaseCubic() {
        _cubic++;
    }

    public void IncreaseInsurance() {
        _insurance++;
    }

    public int GetEffectLevel(EControllableEffects effect) {
        switch(effect) {
            case EControllableEffects.Green: {
                return Green;
            }
            case EControllableEffects.Yellow: {
                return Yellow;
            }
            case EControllableEffects.Black: {
                return Black;
            }
            case EControllableEffects.Star: {
                return Star;
            }
            case EControllableEffects.Red: {
                return Red;
            }
            default: return 0;
        }
    }

    public int GetBoosterLevel(EBoosters booster) {
        switch(booster) {
            case EBoosters.Lasso: {
                return Lasso;
            }
            case EBoosters.Boombaster: {
                return Boombaster;
            }
            case EBoosters.Flash: {
                return Flash;
            }
            case EBoosters.Blot: {
                return Blot;
            }
            case EBoosters.Mop: {
                return Mop;
            }
            default: return 0;
        }
    }
}

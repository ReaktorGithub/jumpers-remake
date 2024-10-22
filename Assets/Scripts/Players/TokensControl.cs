using UnityEngine;

public class TokensControl : MonoBehaviour
{
    public static TokensControl Instance { get; private set; }
    [SerializeField] private float _pedestalMoveTime = 4f;
    [SerializeField] private float _squeezeTime = 0.8f;
    [SerializeField] private float _squeezeMaxValue = 5.2f;
    [SerializeField] private float _squeezeMinValue = 4.6f;
    [SerializeField] private float _squeezeDefaultValue = 4.6f;
    [SerializeField] private float _arrowMovingTime = 1.5f;
    [SerializeField] private float _bonusFlashTime = 3f;
    [SerializeField] private float _nextBonusTime = 1.5f;
    [SerializeField] GameObject _indicatorLightningObject, _bonusEventSample, _indicatorFlashObject, _indicatorBlotObject;
    [SerializeField] private float _teleportAnimationTime = 1f;
    [SerializeField] private float _oreolScale1 = 25f;
    [SerializeField] private float _oreolScale2 = 46f;

    private void Awake() {
        Instance = this;
    }

    public float PedestalMoveTime {
        get { return _pedestalMoveTime; }
        private set {}
    }

    public float SqueezeTime {
        get { return _squeezeTime; }
        private set {}
    }

    public float SqueezeMaxValue {
        get { return _squeezeMaxValue; }
        private set {}
    }

    public float SqueezeMinValue {
        get { return _squeezeMinValue; }
        private set {}
    }

    public float SqueezeDefaultValue {
        get { return _squeezeDefaultValue; }
        private set {}
    }

    public float OreolScale1 {
        get { return _oreolScale1; }
        private set {}
    }

    public float OreolScale2 {
        get { return _oreolScale2; }
        private set {}
    }

    public float ArrowMovingTime {
        get { return _arrowMovingTime; }
        private set {}
    }

    public float BonusFlashTime {
        get { return _bonusFlashTime; }
        private set {}
    }

    public float NextBonusTime {
        get { return _nextBonusTime; }
        private set {}
    }

    public GameObject BonusEventSample {
        get { return _bonusEventSample; }
        private set {}
    }

    public float TeleportAnimationTime {
        get { return _teleportAnimationTime; }
        private set {}
    }
}

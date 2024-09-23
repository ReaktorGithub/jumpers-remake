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
    [SerializeField] private float _indicatorWidthDefault = 3.4f;
    [SerializeField] private float _indicatorWidthSmall = 2f;
    private GameObject _tokenIndicatorSample;
    [SerializeField] GameObject _indicatorLightningObject, _bonusEventSample;
    private Sprite _indicatorLightningSprite;

    private void Awake() {
        Instance = this;
        _tokenIndicatorSample = GameObject.Find("Instances").transform.Find("TokenIndicator").gameObject;
        _indicatorLightningSprite = _indicatorLightningObject.GetComponent<SpriteRenderer>().sprite;
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

    public float ArrowMovingTime {
        get { return _arrowMovingTime; }
        private set {}
    }

    public float IndicatorWidthDefault {
        get { return _indicatorWidthDefault; }
        private set {}
    }

    public float IndicatorWidthSmall {
        get { return _indicatorWidthSmall; }
        private set {}
    }

    public GameObject TokenIndicatorSample {
        get { return _tokenIndicatorSample; }
        private set {}
    }

    public GameObject BonusEventSample {
        get { return _bonusEventSample; }
        private set {}
    }

    public Sprite IndicatorLightningSprite {
        get { return _indicatorLightningSprite; }
        private set {}
    }
}

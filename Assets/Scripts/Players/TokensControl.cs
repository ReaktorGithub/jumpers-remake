using System.Collections;
using System.Collections.Generic;
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

    public float ArrowMovingTime {
        get { return _arrowMovingTime; }
        private set {}
    }
}

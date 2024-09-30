using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class BranchButton : MonoBehaviour
{
    private IEnumerator _coroutine;
    [SerializeField] private float _pulseTime = 0.8f;
    [SerializeField] private float _pulseMaxValue = 0.6f;
    [SerializeField] private float _pulseMinValue = 0.4f;
    [SerializeField] private float _pulseDefaultValue = 0.6f;
    [SerializeField] private GameObject _nextCell;
    [SerializeField] private EAiBranchTypes _aiBranchType = EAiBranchTypes.Normal;
    [SerializeField] private bool _isDeadEnd = false;
    private float _currentX;
    private bool _pausePulse = false;
    private bool _disabled = false;
    
    private void Awake() {
        _currentX = transform.localScale.x;
    }

    public GameObject NextCell {
        get { return _nextCell; }
        private set {}
    }

    public bool IsDeadEnd {
        get { return _isDeadEnd; }
        private set {}
    }

    public EAiBranchTypes AiBranchType {
        get { return _aiBranchType; }
        private set {}
    }

    public bool Disabled {
        get { return _disabled; }
        set {
            _disabled = value;
            if (value) {
                StopPulse();
            } else {
                StartPulse();
            }
        }
    }

    // Pulse animation

    public void StartPulse() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _pausePulse = false;
        _coroutine = PulseAnimation();
        StartCoroutine(_coroutine);
    }

    public void StopPulse() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        transform.localScale = new Vector3(
            _pulseDefaultValue,
            _pulseDefaultValue,
            transform.localScale.z);
    }

    public void PausePulse() {
        if (_disabled) {
            return;
        }

        _pausePulse = true;
        transform.localScale = new Vector3(
            _pulseDefaultValue,
            _pulseDefaultValue,
            transform.localScale.z);
    }

    public void UnpausePulse() {
        if (_disabled) {
            return;
        }

        _pausePulse = false;
    }

    private IEnumerator PulseAnimation() {
        bool isIn = true;
        while (true) {
            while (isIn && _currentX > _pulseMinValue) {
                float scale = _currentX - (_pulseTime * Time.deltaTime);
                _currentX = scale;
                if (!_pausePulse) {
                    transform.localScale = new Vector3(
                    scale,
                    scale,
                    transform.localScale.z);
                }
                yield return null;
            }
            isIn = false;
            while (!isIn && _currentX < _pulseMaxValue) {
                float scale = _currentX + (_pulseTime * Time.deltaTime);
                _currentX = scale;
                if(!_pausePulse) {
                    transform.localScale = new Vector3(
                    scale,
                    scale,
                    transform.localScale.z);
                }
                yield return null;
            }
            isIn = true;
        }
    }

    public void ConfirmNewDirection() {
        if (_disabled) {
            return;
        }
        
        bool isHedgehog = transform.TryGetComponent(out BranchButtonHedge hedge);

        if (isHedgehog) {
            hedge.InitiateHedgehogChoice();
            return;
        }

        if (MoveControl.Instance.IsViolateMode) {
            MoveControl.Instance.SwitchBranchViolate(_nextCell);
            return;
        }

        MoveControl.Instance.SwitchBranch(_nextCell, _isDeadEnd);
    }

    public void ExecuteHedgehogChoice(SplineContainer spline) {
        MoveControl.Instance.SwitchBranchHedgehog(_nextCell, spline);
    }

    public int GetHedgehogTax() {
        transform.TryGetComponent(out BranchButtonHedge button);
        if (button != null) {
            return button.TaxCost;
        } else {
            return 0;
        }
    }

    public BranchButtonHedge GetHedgehogScript() {
        transform.TryGetComponent(out BranchButtonHedge button);
        return button;
    }
}

using System.Collections;
using UnityEngine;

public class BranchButton : MonoBehaviour
{
    private IEnumerator _coroutine;
    [SerializeField] private float _pulseTime = 0.8f;
    [SerializeField] private float _pulseMaxValue = 0.6f;
    [SerializeField] private float _pulseMinValue = 0.4f;
    [SerializeField] private float _pulseDefaultValue = 0.6f;
    [SerializeField] private GameObject _nextCell;
    [SerializeField] private EAiBranchTypes _aiBranchType = EAiBranchTypes.Normal;
    private float _currentX;
    private bool _pausePulse = false;
    
    private void Awake() {
        _currentX = transform.localScale.x;
    }

    public GameObject NextCell {
        get { return _nextCell; }
        private set {}
    }

    public EAiBranchTypes AiBranchType {
        get { return _aiBranchType; }
        private set {}
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
        _pausePulse = true;
        transform.localScale = new Vector3(
            _pulseDefaultValue,
            _pulseDefaultValue,
            transform.localScale.z);
    }

    public void UnpausePulse() {
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
        if (MoveControl.Instance.IsViolateMode) {
            MoveControl.Instance.SwitchBranchViolate(_nextCell);
        } else {
            MoveControl.Instance.SwitchBranch(_nextCell);
        }
    }
}

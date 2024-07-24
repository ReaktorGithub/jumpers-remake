using System.Collections;
using UnityEngine;

public class BranchButton : MonoBehaviour
{
    private IEnumerator _coroutine;
    [SerializeField] private float pulseTime = 0.8f;
    [SerializeField] private float pulseMaxValue = 0.6f;
    [SerializeField] private float pulseMinValue = 0.4f;
    [SerializeField] private float pulseDefaultValue = 0.6f;
    [SerializeField] private string nextCell;
    private float _currentX;
    private bool _pausePulse = false;
    
    private void Awake() {
        _currentX = transform.localScale.x;
    }

    public string NextCell {
        get { return nextCell; }
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
            pulseDefaultValue,
            pulseDefaultValue,
            transform.localScale.z);
    }

    public void PausePulse() {
        _pausePulse = true;
        transform.localScale = new Vector3(
            pulseDefaultValue,
            pulseDefaultValue,
            transform.localScale.z);
    }

    public void UnpausePulse() {
        _pausePulse = false;
    }

    private IEnumerator PulseAnimation() {
        bool isIn = true;
        while (true) {
            while (isIn && _currentX > pulseMinValue) {
                float scale = _currentX - (pulseTime * Time.deltaTime);
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
            while (!isIn && _currentX < pulseMaxValue) {
                float scale = _currentX + (pulseTime * Time.deltaTime);
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
        MoveControl.Instance.SwitchBranch(nextCell);
    }
}

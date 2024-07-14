using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera _cinemachineCamera;
    private CinemachineTransposer _transposer;
    private Transform _savedObjectToFollow = null;
    private int _savedDirection = 0;
    [SerializeField] private float offset = 15f;
    [SerializeField] private float offsetTime = 1f;
    private IEnumerator _coroutine;
    [SerializeField] private bool isFollow = true;
    
    // zoom
    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float minZoom = 38f;
    [SerializeField] private float maxZoom = 64f;
    [SerializeField] private float defaultZoom = 64f;
    private float _zoom;
    [SerializeField] private float smoothTime = 0.2f;
    private float _velocity = 0f;

    private void Awake() {
        _cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        _transposer = GetComponentInChildren<CinemachineTransposer>();
    }

    private void FixedUpdate() {
        float scroll = Input.mouseScrollDelta.y;
        _zoom -= scroll * zoomSpeed * Time.deltaTime;
        _zoom = Mathf.Clamp(_zoom, minZoom, maxZoom);
        _cinemachineCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(
            _cinemachineCamera.m_Lens.OrthographicSize,
            _zoom,
            ref _velocity,
            smoothTime
        );
    }

    public void FollowObject(Transform objectToFollow) {
        _savedObjectToFollow = objectToFollow;
        if (!isFollow || !_savedObjectToFollow) {
            return;
        }
        _cinemachineCamera.Follow = objectToFollow;
    }

    // direction: 0, 1, -1

    public void SetTransposerOffsetX(int direction) {
        _savedDirection = direction;
        float targetValue = isFollow ? direction * offset : 0;
        if (targetValue == _transposer.m_FollowOffset.x) {
            return;
        }
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _coroutine = OffsetDefer(targetValue);
        StartCoroutine(_coroutine);
    }

    private IEnumerator OffsetDefer(float targetValue) {
        float startTime = Time.time;
        float velocity = 0f;
        float initialValue = _transposer.m_FollowOffset.x;
        while (Time.time - startTime < offsetTime) {
            float progress = (Time.time - startTime) / offsetTime;
            float value = Mathf.SmoothDamp(initialValue, targetValue, ref velocity, 0.1f, Mathf.Infinity, progress); 
            _transposer.m_FollowOffset.x = value;
            yield return null;
        }
    }

    public void FollowOn() {
        isFollow = true;
        SetTransposerOffsetX(_savedDirection);
        FollowObject(_savedObjectToFollow);
    }

    public void FollowOff() {
        isFollow = false;
        _cinemachineCamera.m_Lens.OrthographicSize = defaultZoom;
        ClearFollow();
    }

    public void ClearFollow() {
        _cinemachineCamera.Follow = null;
        _savedObjectToFollow = null;
        _savedDirection = 0;
    }
}

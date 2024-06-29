using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera _cinemachineCamera;

    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float minZoom = 38f;
    [SerializeField] private float maxZoom = 64f;
    private float _zoom;
    [SerializeField] private float smoothTime = 0.2f;
    private float _velocity = 0f;

    private void Awake() {
        _cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        _zoom = _cinemachineCamera.m_Lens.OrthographicSize;
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
}

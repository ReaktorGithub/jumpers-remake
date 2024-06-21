using Cinemachine;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineCamera;

    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float minZoom = 38f;
    [SerializeField] private float maxZoom = 64f;
    private float _zoom;
    [SerializeField] private float smoothTime = 0.2f;
    private float _velocity = 0f;

    private void Awake() {
        cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        _zoom = cinemachineCamera.m_Lens.OrthographicSize;
    }

    private void FixedUpdate() {
        float scroll = Input.mouseScrollDelta.y;
        _zoom -= scroll * zoomSpeed * Time.fixedDeltaTime;
        _zoom = Mathf.Clamp(_zoom, minZoom, maxZoom);
        cinemachineCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(
            cinemachineCamera.m_Lens.OrthographicSize,
            _zoom,
            ref _velocity,
            smoothTime
        );
    }
}

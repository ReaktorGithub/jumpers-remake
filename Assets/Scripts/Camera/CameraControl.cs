using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

public class CameraControl : MonoBehaviour
{
    private CinemachineVirtualCamera _cinemachineCamera;
    private LevelData _levelData;
    private Transform _savedObjectToFollow = null;
    [SerializeField] private bool isFollow = true;
    
    // zoom
    [SerializeField] private float zoomSpeed = 100f;
    [SerializeField] private float minZoom = 38f;
    [SerializeField] private float maxZoom = 64f;
    [SerializeField] private float defaultZoom = 64f;
    [SerializeField] private bool isZoom = true;
    private float _zoom;
    private float _savedZoom = 0;
    private float _scroll;
    [SerializeField] private float zoomSmoothTime = 0.2f;
    private float _velocity = 0f;

    // wasd

    [SerializeField] private float panSpeed = 100f;
    [SerializeField] private Vector2 minPanBound = new(-130,-60);
    [SerializeField] private Vector2 maxPanBound = new(130,60);
    private GameObject _cameraTarget;

    private void Awake() {
        _cinemachineCamera = GetComponent<CinemachineVirtualCamera>();
        _cameraTarget = GameObject.Find("CameraTarget");
        _levelData = GameObject.Find("GameScripts").GetComponent<LevelData>();
        _zoom = defaultZoom;
    }

    private void FixedUpdate() {
        ZoomLogic();
        PanLogic();
        RaycastPointerAndLockZoom();
    }

    private void ZoomLogic() {
        if (isZoom) {
            _scroll = Input.mouseScrollDelta.y;
        }
        _zoom -= _scroll * zoomSpeed * Time.deltaTime;
        _zoom = Mathf.Clamp(_zoom, minZoom, maxZoom);
        _cinemachineCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(
            _cinemachineCamera.m_Lens.OrthographicSize,
            _zoom,
            ref _velocity,
            zoomSmoothTime
        );
    }

    private void PanLogic() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = new(horizontalInput, verticalInput, 0);
        Vector3 newPosition = _cameraTarget.transform.position;
        newPosition += panSpeed * Time.deltaTime * moveDirection;
        newPosition.x = Mathf.Clamp(newPosition.x, minPanBound.x, maxPanBound.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minPanBound.y, maxPanBound.y);
        _cameraTarget.transform.position = newPosition;
    }

    public void FollowObject(Transform objectToFollow) {
        _savedObjectToFollow = objectToFollow;
        if (!isFollow || !_savedObjectToFollow) {
            return;
        }
        _cinemachineCamera.Follow = objectToFollow;
    }

    public void FollowOn() {
        isFollow = true;
        _zoom = defaultZoom;
        FollowObject(_savedObjectToFollow);
    }

    public void FollowOff() {
        isFollow = false;
        if (_savedObjectToFollow != null) {
            _cameraTarget.transform.position = _savedObjectToFollow.transform.position;
        }
        _cinemachineCamera.Follow = _cameraTarget.transform;
    }

    public void ClearFollow() {
        if (isFollow) {
            _cinemachineCamera.Follow = null;
        }
    }

    public void SetIsZoom(bool value) {
        isZoom = value;
    }

    private void RaycastPointerAndLockZoom() {
        PointerEventData eventData = new(EventSystem.current) {
            position = Input.mousePosition
        };
        List<RaycastResult> raysastResults = new();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        bool changed = false;
        for (int i = 0; i < raysastResults.Count; i++) {
            if (raysastResults[i].gameObject.CompareTag("LockZoom")) {
                SetIsZoom(false);
                changed = true;
            }
        }
        if (!changed) {
            SetIsZoom(true);
        }
    }

    public void MoveCameraToLevelCenter() {
        _cameraTarget.transform.position = new Vector3(
            _levelData.LevelCenterPosition[0],
            _levelData.LevelCenterPosition[1],
            _cameraTarget.transform.position.z
        );
        _savedZoom = _zoom;
        _zoom = _levelData.LevelCenterCameraZoom;
    }

    public void RestoreSavedZoom() {
        if (_savedZoom != 0) {
            _zoom = _savedZoom;
        }
    }
}

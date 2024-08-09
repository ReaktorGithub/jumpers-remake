using UnityEngine;

public class CameraButton : MonoBehaviour
{
    [SerializeField] private GameObject _followOnImage;
    [SerializeField] private GameObject _followOffImage;
    private CameraControl _camera;
    private bool _disabled = false;
    private bool _isOn = true;

    private void Awake() {
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
    }

    private void Start() {
        UpdateImages();
    }

    public bool IsOn {
        get { return _isOn; }
        private set {}
    }

    private void UpdateImages() {
        _followOnImage.SetActive(_isOn);
        _followOffImage.SetActive(!_isOn);
    }

    public void OnClick() {
        if (_disabled) {
            return;
        }
        _isOn = !_isOn;
        UpdateImages();
        if (_isOn) {
            _camera.FollowOn();
        } else {
            _camera.FollowOff();
        }
    }

    public void SetDisabled(bool value) {
        _disabled = value;
        transform.gameObject.SetActive(!value);
    }
}

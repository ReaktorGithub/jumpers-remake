using UnityEngine;
using UnityEngine.UI;

public class CameraButton : MonoBehaviour
{
    private Toggle _toggle;
    private Image _image;
    private CameraControl _camera;

    private void Awake() {
        _toggle = GetComponent<Toggle>();
        _image = transform.Find("Background").GetComponent<Image>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
    }

    public void OnChangeValue() {
        if (_toggle.isOn) {
            _image.enabled = false;
            _camera.FollowOn();
        } else {
            _image.enabled = true;
            _camera.FollowOff();
        }
    }
}

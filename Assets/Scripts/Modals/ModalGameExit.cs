using System.Collections;
using UnityEngine;

public class ModalGameExit : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _windowControl;
    private IEnumerator _coroutine;

    private void Awake() {
        _modal = GameObject.Find("ModalGameExit");
        _windowControl = _modal.transform.Find("WindowExit").GetComponent<ModalWindow>();
    }

    private void Start() {
        _modal.SetActive(false);
    }

    public void ExitApplication() {
        Application.Quit();
    }

    private void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OpenWindow();
        }
    }

    public void OpenWindow() {
        if (!_modal.activeInHierarchy) {
            _modal.SetActive(true);
            _coroutine = _windowControl.FadeIn();
            StartCoroutine(_coroutine);
        }
    }

    public void CloseWindow() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _modal.SetActive(false);
        _windowControl.ResetScale();
    }
}

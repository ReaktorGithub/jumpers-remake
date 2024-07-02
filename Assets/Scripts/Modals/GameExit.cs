using UnityEngine;

public class GameExit : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _window;

    private void Awake() {
        _modal = GameObject.Find("ModalExitGame");
        _window = _modal.transform.Find("WindowExit").GetComponent<ModalWindow>();
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
            StartCoroutine(_window.FadeIn());
        }
    }
}

using UnityEngine;

public class ModalGameExit : MonoBehaviour
{
    private Modal _modal;

    private void Awake() {
        _modal = GameObject.Find("ModalGameExit").GetComponent<Modal>();
    }

    public void ExitApplication() {
        Application.Quit();
    }

    private void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OpenModal();
        }
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void CloseModal() {
        _modal.CloseModal();
    }
}

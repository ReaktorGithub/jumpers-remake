using UnityEngine;

public class ModalWin : MonoBehaviour
{
    private Modal _modal;

    private void Awake() {
        _modal = GameObject.Find("ModalWin").GetComponent<Modal>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void CloseModal() {
        _modal.CloseModal();
    }
}

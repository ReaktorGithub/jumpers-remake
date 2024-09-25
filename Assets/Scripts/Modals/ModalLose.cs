using UnityEngine;

public class ModalLose : MonoBehaviour
{
    private Modal _modal;

    private void Awake() {
        _modal = GameObject.Find("ModalLose").GetComponent<Modal>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void CloseModal() {
        _modal.CloseModal();
    }
}

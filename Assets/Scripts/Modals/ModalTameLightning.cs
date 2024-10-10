using UnityEngine;

public class ModalTameLightning : MonoBehaviour
{
    private Modal _modal;

    private void Awake() {
        _modal = GameObject.Find("ModalTameLightning").GetComponent<Modal>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void CancelTameLightning() {
        _modal.CloseModal();
    }

    public void ConfirmTameLightning() {
        _modal.CloseModal();
        MoveControl.Instance.CurrentPlayer.Boosters.ExecuteTameLightning();
    }
}

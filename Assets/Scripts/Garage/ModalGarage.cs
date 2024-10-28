using System.Collections;
using UnityEngine;

public class ModalGarage : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private float _showContentDelay = 0.6f;

    private void Awake() {
        _modal = GameObject.Find("ModalGarage").GetComponent<Modal>();
    }

    public void OpenModal() {
        StartCoroutine(ShowBodyDefer());
        _modal.OpenModal();
    }

    public void BuildContent(PlayerControl player) {
        GarageControl.Instance.BuildContent(player);
    }

    private IEnumerator ShowBodyDefer() {
        GarageControl.Instance.ShowBody(false);
        yield return new WaitForSeconds(_showContentDelay);
        GarageControl.Instance.ShowBody(true);
    }

    public void CloseModal() {
        _modal.CloseModal();
    }
}

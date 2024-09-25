using UnityEngine;

[RequireComponent(typeof(BoosterButton))]

public class BoosterButtonPick : MonoBehaviour
{
    private ModalHedgehogArrow _modal;
    private BoosterButton _boosterButton;

    private void Awake() {
        _modal = GameObject.Find("ModalScripts").GetComponent<ModalHedgehogArrow>();
        _boosterButton = transform.GetComponent<BoosterButton>();
    }

    public void OnHedgehogSelect() {
        _modal.OnItemClick(_boosterButton);
    }
}

using UnityEngine;

[RequireComponent(typeof(BoosterButton))]

public class BoosterButtonPick : MonoBehaviour
{
    private ModalHedgehogArrow _modal;
    private bool _isSelected = false;
    private BoosterButton _boosterButton;

    private void Awake() {
        _modal = GameObject.Find("GameScripts").GetComponent<ModalHedgehogArrow>();
        _boosterButton = transform.GetComponent<BoosterButton>();
    }

    public bool IsSelected {
        get { return _isSelected; }
        set { _isSelected = value; }
    }

    public void OnSelect() {
        _modal.OnItemClick(_boosterButton, this);
    }
}

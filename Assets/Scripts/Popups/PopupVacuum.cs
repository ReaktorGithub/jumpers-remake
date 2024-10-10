using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupVacuum : MonoBehaviour
{
    private Popup _popup;
    private Button _confirmButton;
    private int _selectedScore = 0;
    private List<CubicButton> _cubicButtons = new();
    [SerializeField] GameObject _boxButtons;

    private void Awake() {
        GameObject popupMagnet = GameObject.Find("PopupVacuum");
        _popup = popupMagnet.GetComponent<Popup>();
        _confirmButton = Utils.FindChildByName(popupMagnet, "ButtonOk").GetComponent<Button>();
        CubicButton[] allButtons = _boxButtons.GetComponentsInChildren<CubicButton>();
        foreach (CubicButton button in allButtons) {
            _cubicButtons.Add(button);
        }
    }

    public int SelectedScore {
        get { return _selectedScore; }
        set {
            _selectedScore = value;
            SetConfirmButtonInteractable(value > 0);
        }
    }

    private void SetConfirmButtonInteractable(bool value) {
        _confirmButton.interactable = value;
        _confirmButton.GetComponent<CursorManager>().Disabled = !value;
    }

    public void OnOpenWindow() {
        SetConfirmButtonInteractable(false);
        CubicControl.Instance.SetCubicInteractable(false);
        _popup.OpenWindow();
    }

    public void OnCloseWindow() {
        SetConfirmButtonInteractable(false);
        _popup.CloseWindow(() => {
            BoostersControl.Instance.EnableAllButtons();
            EffectsControl.Instance.TryToEnableAllEffectButtons();
            CubicControl.Instance.SetCubicInteractable(true);
        });
    }

    public void OnConfirm() {
        SetConfirmButtonInteractable(false);
        BoostersControl.Instance.UnselectAllButtons();
        _popup.CloseWindow(() => {
            MoveControl.Instance.CurrentPlayer.Boosters.ExecuteVacuum(true, _selectedScore);
        });
    }

    public void OnCubicButtonClick(int score) {
        SelectedScore = score;
        UpdateButonsSelection();
    }

    public void BuildContent() {
        foreach (CubicButton button in _cubicButtons) {
            button.SetSelected(false);
        }
        _selectedScore = 0;
    }

    public void UpdateButonsSelection() {
        foreach (CubicButton button in _cubicButtons) {
            button.SetSelected(button.Score == _selectedScore);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class EffectControl : MonoBehaviour
{
    private CubicControl _cubicControl;
    private CameraButton _cameraButton;
    private EControllableEffects _selectedEffect = EControllableEffects.None;
    private bool _isSelectionMode;
    private CameraControl _cameraControl;
    private TopPanel _topPanel;
    private CellsControl _cellsControl;
    [SerializeField] private List<EffectButton> _effectButtonsList;

    private void Awake() {
        _cubicControl = GameObject.Find("Cubic").GetComponent<CubicControl>();
        _cameraControl = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _cameraButton = GameObject.Find("CameraButton").GetComponent<CameraButton>();
        _cellsControl = GameObject.Find("Cells").GetComponent<CellsControl>();
    }

    public EControllableEffects SelectedEffect {
        get { return _selectedEffect; }
        set { _selectedEffect = value; }
    }

    public void UpdateButtonsSelection() {
        foreach (EffectButton button in _effectButtonsList) {
            button.SetSelected(button.GetComponent<EffectButton>().EffectType == _selectedEffect);
        }
    }

    public void ActivateSelectionMode() {
        if (_isSelectionMode) {
            return;
        }
        _isSelectionMode = true;
        _cubicControl.SetCubicInteractable(false);
        _cameraControl.FollowOff();
        _cameraControl.MoveCameraToLevelCenter();
        _cameraButton.SetDisabled(true);
        _topPanel.SetText("Выберите свободную клетку на поле");
        _topPanel.OpenWindow();
        _topPanel.SetCancelButtonActive(true, () => {
            DeactivateSelectionMode();
        });
        _cellsControl.TurnOnEffectPlacementMode();
    }

    public void DeactivateSelectionMode() {
        if (!_isSelectionMode) {
            return;
        }
        _isSelectionMode = false;
        _cellsControl.TurnOffEffectPlacementMode();
        _cubicControl.SetCubicInteractable(true);
        _topPanel.CloseWindow();
        _selectedEffect = EControllableEffects.None;
        UpdateButtonsSelection();
        _cameraControl.RestoreSavedZoom();
        _cameraButton.SetDisabled(false);
        if (_cameraButton.IsOn) {
            _cameraControl.FollowOn();
        } else {
            _cameraControl.FollowOff();
        }
    }
}

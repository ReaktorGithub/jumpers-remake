using System;
using UnityEngine;

public class CellSelection : MonoBehaviour
{
    public static CellSelection Instance { get; private set; }
    private CameraControl _cameraControl;
    private TopPanel _topPanel;
    private CameraButton _cameraButton;

    private void Awake() {
        Instance = this;
        _cameraControl = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _cameraButton = GameObject.Find("CameraButton").GetComponent<CameraButton>();
    }

    public void EnterSelectionMode(string topPanelText, Action onCancel, Action<CellControl> onCellClick, Func<CellControl, bool> cellSelectionPredicate) {
        _cameraControl.FollowOff();
        _cameraControl.MoveCameraToLevelCenter();
        _cameraButton.SetDisabled(true);
        _topPanel.SetText(topPanelText);
        _topPanel.OpenWindow();
        _topPanel.SetCancelButtonActive(true, () => {
            ExitSelectionMode();
            onCancel();
        });
        CellsControl.Instance.TurnOnSelectionMode(cellSelectionPredicate, onCellClick);
    }

    public void ExitSelectionMode() {
        CellsControl.Instance.TurnOffSelectionMode();
        _topPanel.CloseWindow();
    }

    public void RestoreCamera() {
        _cameraControl.RestoreSavedZoom();
        _cameraButton.SetDisabled(false);
        if (_cameraButton.IsOn) {
            _cameraControl.FollowOn();
        } else {
            _cameraControl.FollowOff();
        }
    }

    public void DisableInterface(bool disableEffects) {
        BoostersControl.Instance.DisableAllButtons();
        CubicControl.Instance.SetCubicInteractable(false);
        if (disableEffects) {
            EffectsControl.Instance.DisableAllButtons(true);
        }
    }

    public void EnableInterface() {
        EffectsControl.Instance.TryToEnableAllEffectButtons();
        BoostersControl.Instance.EnableAllButtons();
        CubicControl.Instance.SetCubicInteractable(true);
    }
}

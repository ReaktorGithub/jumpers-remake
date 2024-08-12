using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectsControl : MonoBehaviour
{
    public static EffectsControl Instance { get; private set; }
    private CubicControl _cubicControl;
    private CameraButton _cameraButton;
    private EControllableEffects _selectedEffect = EControllableEffects.None;
    private bool _isSelectionMode;
    private CameraControl _cameraControl;
    private TopPanel _topPanel;
    private List<EffectButton> _effectButtonsList = new();
    private EffectButton _greenEffectButton;
    private EffectButton _yellowEffectButton;
    private EffectButton _redEffectButton;
    private EffectButton _blackEffectButton;
    [SerializeField] private GameObject emptyCellSprite;
    [SerializeField] private GameObject greenCellSprite;
    [SerializeField] private GameObject yellowCellSprite;
    [SerializeField] private GameObject blackCellSprite;
    [SerializeField] private GameObject redCellSprite;
    private TextMeshProUGUI _greenQuantityText;
    private TextMeshProUGUI _yellowQuantityText;
    private TextMeshProUGUI _redQuantityText;
    private TextMeshProUGUI _blackQuantityText;

    private void Awake() {
        Instance = this;
        _cubicControl = GameObject.Find("Cubic").GetComponent<CubicControl>();
        _cameraControl = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _cameraButton = GameObject.Find("CameraButton").GetComponent<CameraButton>();
        _greenQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityGreen").GetComponent<TextMeshProUGUI>();
        _yellowQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityYellow").GetComponent<TextMeshProUGUI>();
        _redQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityRed").GetComponent<TextMeshProUGUI>();
        _blackQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityBlack").GetComponent<TextMeshProUGUI>();
        _greenEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonGreen").GetComponent<EffectButton>();
        _yellowEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonYellow").GetComponent<EffectButton>();
        _redEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonRed").GetComponent<EffectButton>();
        _blackEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonBlack").GetComponent<EffectButton>();
        _effectButtonsList.Add(_greenEffectButton);
        _effectButtonsList.Add(_yellowEffectButton);
        _effectButtonsList.Add(_redEffectButton);
        _effectButtonsList.Add(_blackEffectButton);
    }

    private void Start() {
        emptyCellSprite.SetActive(false);
        greenCellSprite.SetActive(false);
        yellowCellSprite.SetActive(false);
        blackCellSprite.SetActive(false);
        redCellSprite.SetActive(false);
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
        CellsControl.Instance.TurnOnEffectPlacementMode();
    }

    public void DeactivateSelectionMode() {
        DeactivateSelectionModePhase1();
        DeactivateSelectionModePhase2();
    }

    public IEnumerator DeactivateSelectionModeDefer() {
        DeactivateSelectionModePhase1();
        yield return new WaitForSeconds(CellsControl.Instance.ChangingEffectDelay);
        DeactivateSelectionModePhase2();
    }

    public void DeactivateSelectionModePhase1() {
        if (!_isSelectionMode) {
            return;
        }
        _isSelectionMode = false;
        CellsControl.Instance.TurnOffEffectPlacementMode();
        _topPanel.CloseWindow();
        _selectedEffect = EControllableEffects.None;
        UpdateButtonsSelection();
    }

    public void DeactivateSelectionModePhase2() {
        _cubicControl.SetCubicInteractable(true);
        _cameraControl.RestoreSavedZoom();
        _cameraButton.SetDisabled(false);
        if (_cameraButton.IsOn) {
            _cameraControl.FollowOn();
        } else {
            _cameraControl.FollowOff();
        }
    }

    // public void OnChangeEffect(CellControl cellControl) {
    //     Sprite sprite;
    //     PlayerControl player = MoveControl.Instance.CurrentPlayer;

    //     switch(_selectedEffect) {
    //         case EControllableEffects.Green: {
    //             sprite = greenCellSprite.GetComponent<SpriteRenderer>().sprite;
    //             player.AddEffectGreen(-1);
    //             break;
    //         }
    //         case EControllableEffects.Red: {
    //             sprite = redCellSprite.GetComponent<SpriteRenderer>().sprite;
    //             player.AddEffectRed(-1);
    //             break;
    //         }
    //         case EControllableEffects.Yellow: {
    //             sprite = yellowCellSprite.GetComponent<SpriteRenderer>().sprite;
    //             player.AddEffectYellow(-1);
    //             break;
    //         }
    //         case EControllableEffects.Black: {
    //             sprite = blackCellSprite.GetComponent<SpriteRenderer>().sprite;
    //             player.AddEffectBlack(-1);
    //             break;
    //         }
    //         default: {
    //             sprite = emptyCellSprite.GetComponent<SpriteRenderer>().sprite;
    //             break;
    //         }
    //     }
    //     UpdateQuantityText(player);
    //     UpdateEffectEmptiness(player);
    //     SetDisabledEffectButtons(true);
    //     cellControl.ChangeEffect(_selectedEffect, sprite);
    //     StartCoroutine(DeactivateSelectionModeDefer());
    // }

    public void OnChangeEffect(CellControl cellControl) {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;

        switch(_selectedEffect) {
            case EControllableEffects.Green: {
                player.AddEffectGreen(-1);
                break;
            }
            case EControllableEffects.Red: {
                player.AddEffectRed(-1);
                break;
            }
            case EControllableEffects.Yellow: {
                player.AddEffectYellow(-1);
                break;
            }
            case EControllableEffects.Black: {
                player.AddEffectBlack(-1);
                break;
            }
            default: {
                break;
            }
        }

        UpdateQuantityText(player);
        UpdateEffectEmptiness(player);
        SetDisabledEffectButtons(true);
        CellsControl.Instance.SaveAndRemoveCell(cellControl);
        CellsControl.Instance.PlaceNewCell(_selectedEffect);
        StartCoroutine(DeactivateSelectionModeDefer());
    }

    public void UpdateEffectEmptiness(PlayerControl player) {
        _greenEffectButton.SetIsEmpty(player.EffectsGreen == 0);
        _yellowEffectButton.SetIsEmpty(player.EffectsYellow == 0);
        _redEffectButton.SetIsEmpty(player.EffectsRed == 0);
        _blackEffectButton.SetIsEmpty(player.EffectsBlack == 0);
    }

    public void UpdateQuantityText(PlayerControl player) {
        _greenQuantityText.text = "x " + player.EffectsGreen;
        _yellowQuantityText.text = "x " + player.EffectsYellow;
        _redQuantityText.text = "x " + player.EffectsRed;
        _blackQuantityText.text = "x " + player.EffectsBlack;
    }

    public void SetDisabledEffectButtons(bool value) {
        foreach (EffectButton button in _effectButtonsList) {
            button.SetDisabled(value);
        }
    }
}

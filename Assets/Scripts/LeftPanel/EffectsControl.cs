using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectsControl : MonoBehaviour
{
    public static EffectsControl Instance { get; private set; }
    private CameraButton _cameraButton;
    private EControllableEffects _selectedEffect = EControllableEffects.None;
    private bool _isSelectionMode;
    private bool _isReplaceMode;
    private CameraControl _cameraControl;
    private TopPanel _topPanel;
    private List<EffectButton> _effectButtonsList = new();
    private EffectButton _greenEffectButton, _yellowEffectButton, _redEffectButton, _blackEffectButton, _starEffectButton;
    [SerializeField] private GameObject emptyCellSprite, greenCellSprite, yellowCellSprite, blackCellSprite, redCellSprite, starCellSprite;
    [SerializeField] private GameObject yellowBrush, redBrush, blackBrush;
    private TextMeshProUGUI _greenQuantityText, _yellowQuantityText, _redQuantityText, _blackQuantityText, _starQuantityText;
    private GameObject _cellsObject;
    [SerializeField] private float replaceTime = 1f;
    private IEnumerator _coroutine;

    private void Awake() {
        Instance = this;
        _cameraControl = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _cameraButton = GameObject.Find("CameraButton").GetComponent<CameraButton>();
        _greenQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityGreen").GetComponent<TextMeshProUGUI>();
        _yellowQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityYellow").GetComponent<TextMeshProUGUI>();
        _redQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityRed").GetComponent<TextMeshProUGUI>();
        _blackQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityBlack").GetComponent<TextMeshProUGUI>();
        _starQuantityText = Utils.FindChildByName(transform.gameObject, "QuantityStar").GetComponent<TextMeshProUGUI>();
        _greenEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonGreen").GetComponent<EffectButton>();
        _yellowEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonYellow").GetComponent<EffectButton>();
        _redEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonRed").GetComponent<EffectButton>();
        _blackEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonBlack").GetComponent<EffectButton>();
        _starEffectButton = Utils.FindChildByName(transform.gameObject, "EffectButtonStar").GetComponent<EffectButton>();
        _effectButtonsList.Add(_greenEffectButton);
        _effectButtonsList.Add(_yellowEffectButton);
        _effectButtonsList.Add(_redEffectButton);
        _effectButtonsList.Add(_blackEffectButton);
        _effectButtonsList.Add(_starEffectButton);
        _cellsObject = GameObject.Find("Cells");
    }

    private void Start() {
        emptyCellSprite.SetActive(false);
        greenCellSprite.SetActive(false);
        yellowCellSprite.SetActive(false);
        blackCellSprite.SetActive(false);
        redCellSprite.SetActive(false);
        redBrush.SetActive(false);
        yellowBrush.SetActive(false);
        blackBrush.SetActive(false);
        starCellSprite.SetActive(false);
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

    public void ActivateSelectionMode(bool isReplace = false) {
        if (_isSelectionMode) {
            return;
        }
        _isSelectionMode = true;
        _isReplaceMode = isReplace;
        BoostersControl.Instance.DisableAllButtons();
        CubicControl.Instance.SetCubicInteractable(false);
        _cameraControl.FollowOff();
        _cameraControl.MoveCameraToLevelCenter();
        _cameraButton.SetDisabled(true);
        _topPanel.SetText("Выберите свободную клетку на поле");
        _topPanel.OpenWindow();
        _topPanel.SetCancelButtonActive(true, OnCancel);
        CellsControl.Instance.TurnOnEffectPlacementMode();
    }

    private void OnCancel() {
        if (_isReplaceMode) {
            DeactivateReplaceMode();
            MoveControl.Instance.CheckCellEffects();
        } else {
            BoostersControl.Instance.EnableAllButtons();
            DeactivateSelectionMode();
        }
    }

    // отмена во время установки нового эффекта

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
        CubicControl.Instance.SetCubicInteractable(true);
        BoostersControl.Instance.EnableAllButtons();
        RestoreCamera();
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

    // отмена во время перемещения эффекта

    public void DeactivateReplaceMode() {
        DeactivateSelectionModePhase1();
        RestoreCamera();
    }

    // отключение режима перемещения в момент подтверждения

    public void DeactivateOnConfirmReplacePhase1() {
        if (!_isSelectionMode) {
            return;
        }
        _isSelectionMode = false;
        CellsControl.Instance.TurnOffEffectPlacementMode();
        _topPanel.CloseWindow();
    }

    // окончательное отключение режима перемещения

    public void DeactivateOnConfirmReplacePhase2() {
        _selectedEffect = EControllableEffects.None;
        RestoreCamera();
        MoveControl.Instance.CheckCellEffects();
    }

    public void OnConfirmChangeEffect(CellControl cell) {
        if (_isReplaceMode) {
            OnReplaceEffect(cell);
        } else {
            OnChangeEffect(cell);
        }
    }

    // установка нового эффекта

    public void OnChangeEffect(CellControl cell) {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        player.IsEffectPlaced = true;
        Sprite sprite;
        string effectName = "";

        switch(_selectedEffect) {
            case EControllableEffects.Green: {
                sprite = greenCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.AddEffectGreen(-1);
                effectName = "зеленый";
                break;
            }
            case EControllableEffects.Red: {
                sprite = redCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.AddEffectRed(-1);
                effectName = "красный";
                break;
            }
            case EControllableEffects.Yellow: {
                sprite = yellowCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.AddEffectYellow(-1);
                effectName = "желтый";
                break;
            }
            case EControllableEffects.Black: {
                sprite = blackCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.AddEffectBlack(-1);
                effectName = "черный";
                break;
            }
            case EControllableEffects.Star: {
                sprite = starCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.AddEffectStar(-1);
                effectName = "звезда";
                break;
            }
            default: {
                sprite = emptyCellSprite.GetComponent<SpriteRenderer>().sprite;
                break;
            }
        }

        string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " ставит эффект <b>" + effectName + "</b> на клетку " + cell.GetCellText();
        Messages.Instance.AddMessage(message);

        cell.ChangeEffect(_selectedEffect, sprite);
        cell.StartChanging();

        UpdateQuantityText(player);
        UpdateEffectEmptiness(player);
        DisableAllButtons(true);
        StartCoroutine(DeactivateSelectionModeDefer());
    }

    public void UpdateEffectEmptiness(PlayerControl player) {
        _greenEffectButton.SetIsEmpty(player.EffectsGreen == 0);
        _yellowEffectButton.SetIsEmpty(player.EffectsYellow == 0);
        _redEffectButton.SetIsEmpty(player.EffectsRed == 0);
        _blackEffectButton.SetIsEmpty(player.EffectsBlack == 0);
        _starEffectButton.SetIsEmpty(player.EffectsStar == 0);
    }

    public void UpdateQuantityText(PlayerControl player) {
        _greenQuantityText.text = "x " + player.EffectsGreen;
        _yellowQuantityText.text = "x " + player.EffectsYellow;
        _redQuantityText.text = "x " + player.EffectsRed;
        _blackQuantityText.text = "x " + player.EffectsBlack;
        _starQuantityText.text = "x " + player.EffectsStar;
    }

    public void DisableAllButtons(bool value) {
        foreach (EffectButton button in _effectButtonsList) {
            button.SetDisabled(value);
        }
    }

    // перемещение эффекта

    public void OnReplaceEffect(CellControl newCell) {
        DeactivateOnConfirmReplacePhase1();

        // изменить ресурсы игрока

        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        player.ExecuteReplaceEffect(_selectedEffect, MoveControl.Instance.CurrentPlayerIndex);
        UpdateQuantityText(player);
        UpdateEffectEmptiness(player);

        // удалить эффект на текущей клетке

        CellControl oldCell = MoveControl.Instance.CurrentCell;
        Sprite sprite = emptyCellSprite.GetComponent<SpriteRenderer>().sprite;
        oldCell.ChangeEffect(EControllableEffects.None, sprite);
        Sprite newCellSprite;

        // спавнить спрайт краски на текущей клетке

        GameObject brushSprite;
        string effectName = "";

        switch(_selectedEffect) {
            case EControllableEffects.Red: {
                brushSprite = Instantiate(redBrush);
                newCellSprite = redCellSprite.GetComponent<SpriteRenderer>().sprite;
                effectName = "красный";
                break;
            }
            case EControllableEffects.Yellow: {
                brushSprite = Instantiate(yellowBrush);
                newCellSprite = yellowCellSprite.GetComponent<SpriteRenderer>().sprite;
                effectName = "желтый";
                break;
            }
            case EControllableEffects.Black: {
                brushSprite = Instantiate(blackBrush);
                newCellSprite = blackCellSprite.GetComponent<SpriteRenderer>().sprite;
                effectName = "черный";
                break;
            }
            default: {
                brushSprite = emptyCellSprite;
                newCellSprite = emptyCellSprite.GetComponent<SpriteRenderer>().sprite;
                break;
            }
        }

        brushSprite.transform.position = oldCell.transform.position;
        brushSprite.transform.SetParent(_cellsObject.transform);
        brushSprite.SetActive(true);

        // сообщения в мессенджер
        string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " перемещает эффект <b>" + effectName + "</b> на клетку " + newCell.GetCellText();
        Messages.Instance.AddMessage(message);

        // начать анимацию - отправить спрайт на координаты новой клетки
        
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _coroutine = Utils.MoveTo(brushSprite, newCell.transform.position, replaceTime, () => {
            // по окончании анимации удалить спрайт

            Destroy(brushSprite);

            // добавить эффект на новой клетке

            newCell.ChangeEffect(_selectedEffect, newCellSprite);
            newCell.StartChanging();

            // выйти из режима перемещения

            DeactivateOnConfirmReplacePhase2();
        });
        StartCoroutine(_coroutine);
    }
}

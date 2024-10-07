using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectsControl : MonoBehaviour
{
    public static EffectsControl Instance { get; private set; }
    private EControllableEffects _selectedEffect = EControllableEffects.None;
    private List<EffectButton> _effectButtonsList = new();
    private EffectButton _greenEffectButton, _yellowEffectButton, _redEffectButton, _blackEffectButton, _starEffectButton;
    [SerializeField] private GameObject _emptyCellSprite, _greenCellSprite, _yellowCellSprite, _blackCellSprite, _redCellSprite, _starCellSprite;
    [SerializeField] private GameObject _yellowBrush, _redBrush, _blackBrush;
    private TextMeshProUGUI _greenQuantityText, _yellowQuantityText, _redQuantityText, _blackQuantityText, _starQuantityText;
    private GameObject _cellsObject;
    [SerializeField] private float _replaceTime = 1f;
    private IEnumerator _coroutine;

    private void Awake() {
        Instance = this;
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
        _emptyCellSprite.SetActive(false);
        _greenCellSprite.SetActive(false);
        _yellowCellSprite.SetActive(false);
        _blackCellSprite.SetActive(false);
        _redCellSprite.SetActive(false);
        _redBrush.SetActive(false);
        _yellowBrush.SetActive(false);
        _blackBrush.SetActive(false);
        _starCellSprite.SetActive(false);
    }

    public EControllableEffects SelectedEffect {
        get { return _selectedEffect; }
        set {
            _selectedEffect = value;
            UpdateButtonsSelection();
        }
    }

    private bool CellSelectionPredicate(CellControl cell) {
        return cell.CellType == ECellTypes.None && cell.Effect == EControllableEffects.None && cell.IsNoTokens();
    }

    // Режим установки нового эффекта

    public void ActivatePlaceNewEffectMode(EControllableEffects effect) {
        SelectedEffect = effect;
        CellSelection.Instance.DisableInterface(false);

        CellSelection.Instance.EnterSelectionMode(
            "Выберите свободную клетку на поле",
            OnCancelPlaceNew,
            OnPlaceNewEffect,
            CellSelectionPredicate
        );
    }

    private void OnCancelPlaceNew() {
        SelectedEffect = EControllableEffects.None;
        CellSelection.Instance.RestoreCamera();
        CellSelection.Instance.EnableInterface();
    }

    private IEnumerator DeactivatePlaceNewEffectModeDefer() {
        SelectedEffect = EControllableEffects.None;
        CellSelection.Instance.ExitSelectionMode();
        yield return new WaitForSeconds(CellsControl.Instance.ChangingEffectDelay);
        CellSelection.Instance.RestoreCamera();
        CellSelection.Instance.EnableInterface();
    }

    private void OnPlaceNewEffect(CellControl cell) {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        player.Effects.IsEffectPlaced = true;
        Sprite sprite;
        string effectName = "";
        int effectLevel;

        switch(_selectedEffect) {
            case EControllableEffects.Green: {
                sprite = _greenCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.Effects.AddGreen(-1);
                effectName = "зеленый";
                effectLevel = player.Grind.Green;
                break;
            }
            case EControllableEffects.Red: {
                sprite = _redCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.Effects.AddRed(-1);
                effectName = "красный";
                effectLevel = player.Grind.Red;
                break;
            }
            case EControllableEffects.Yellow: {
                sprite = _yellowCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.Effects.AddYellow(-1);
                effectName = "желтый";
                effectLevel = player.Grind.Yellow;
                break;
            }
            case EControllableEffects.Black: {
                sprite = _blackCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.Effects.AddBlack(-1);
                effectName = "черный";
                effectLevel = player.Grind.Black;
                break;
            }
            case EControllableEffects.Star: {
                sprite = _starCellSprite.GetComponent<SpriteRenderer>().sprite;
                player.Effects.AddStar(-1);
                effectName = "звезда";
                effectLevel = player.Grind.Star;
                break;
            }
            default: {
                sprite = _emptyCellSprite.GetComponent<SpriteRenderer>().sprite;
                effectLevel = 1;
                break;
            }
        }

        string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " ставит эффект <b>" + effectName + "</b> на клетку №" + cell.NameDisplay;
        Messages.Instance.AddMessage(message);

        cell.ChangeEffect(_selectedEffect, sprite, effectLevel);
        cell.StartChanging();

        UpdateQuantityText(player);
        UpdateButtonsVisual(player);
        DisableAllButtons(true);
        StartCoroutine(DeactivatePlaceNewEffectModeDefer());
    }

    // Режим перемещения эффекта

    public void ActivateReplaceEffectMode() {
        CellSelection.Instance.DisableInterface(true);

        CellSelection.Instance.EnterSelectionMode(
            "Выберите свободную клетку на поле",
            OnCancelReplace,
            OnReplaceEffect,
            CellSelectionPredicate
        );
    }

    private void OnCancelReplace() {
        MoveControl.Instance.CheckCellEffects();
        CellSelection.Instance.RestoreCamera();
    }

    private IEnumerator DeactivateReplaceEffectModeDefer() {
        yield return new WaitForSeconds(CellsControl.Instance.ChangingEffectDelay);
        CellSelection.Instance.RestoreCamera();
        MoveControl.Instance.CheckCellEffects();
    }

    private void OnReplaceEffect(CellControl newCell) {
        CellSelection.Instance.ExitSelectionMode();

        // изменить ресурсы игрока

        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        EControllableEffects selectedEffect = player.GetCurrentCell().Effect;
        player.Effects.ExecuteReplaceEffect(selectedEffect);
        UpdateQuantityText(player);
        UpdateButtonsVisual(player);

        // удалить эффект на текущей клетке

        CellControl oldCell = player.GetCurrentCell();
        int effectLevel = oldCell.EffectLevel;
        Sprite sprite = _emptyCellSprite.GetComponent<SpriteRenderer>().sprite;
        oldCell.ChangeEffect(EControllableEffects.None, sprite, 1);
        Sprite newCellSprite;

        // спавнить спрайт краски на текущей клетке

        GameObject brushSprite;
        string effectName = "";

        switch(selectedEffect) {
            case EControllableEffects.Red: {
                brushSprite = Instantiate(_redBrush);
                newCellSprite = _redCellSprite.GetComponent<SpriteRenderer>().sprite;
                effectName = "красный";
                break;
            }
            case EControllableEffects.Yellow: {
                brushSprite = Instantiate(_yellowBrush);
                newCellSprite = _yellowCellSprite.GetComponent<SpriteRenderer>().sprite;
                effectName = "желтый";
                break;
            }
            case EControllableEffects.Black: {
                brushSprite = Instantiate(_blackBrush);
                newCellSprite = _blackCellSprite.GetComponent<SpriteRenderer>().sprite;
                effectName = "черный";
                break;
            }
            default: {
                brushSprite = _emptyCellSprite;
                newCellSprite = _emptyCellSprite.GetComponent<SpriteRenderer>().sprite;
                break;
            }
        }

        brushSprite.transform.position = oldCell.transform.position;
        brushSprite.transform.SetParent(_cellsObject.transform);
        brushSprite.SetActive(true);

        // сообщения в мессенджер
        string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " перемещает эффект <b>" + effectName + "</b> на клетку №" + newCell.NameDisplay;
        Messages.Instance.AddMessage(message);

        // начать анимацию - отправить спрайт на координаты новой клетки
        
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _coroutine = Utils.MoveTo(brushSprite, newCell.transform.position, _replaceTime, () => {
            // по окончании анимации удалить спрайт

            Destroy(brushSprite);

            // добавить эффект на новой клетке

            newCell.ChangeEffect(selectedEffect, newCellSprite, effectLevel);
            newCell.StartChanging();

            // выйти из режима перемещения

            StartCoroutine(DeactivateReplaceEffectModeDefer());
        });
        StartCoroutine(_coroutine);
    }

    // Апдейты визуала

    public void UpdateButtonsSelection() {
        foreach (EffectButton button in _effectButtonsList) {
            button.SetSelected(button.GetComponent<EffectButton>().EffectType == _selectedEffect);
        }
    }

    public void UpdateButtonsVisual(PlayerControl player) {
        PlayerGrind grind = player.Grind;
        PlayerEffects effects = player.Effects;

        _greenEffectButton.SetIsEmpty(effects.Green == 0, grind.Green);
        _yellowEffectButton.SetIsEmpty(effects.Yellow == 0, grind.Yellow);
        _redEffectButton.SetIsEmpty(effects.Red == 0, grind.Red);
        _blackEffectButton.SetIsEmpty(effects.Black == 0, grind.Black);
        _starEffectButton.SetIsEmpty(effects.Star == 0, grind.Star);
    }

    public void UpdateQuantityText(PlayerControl player) {
        _greenQuantityText.text = "x " + player.Effects.Green;
        _yellowQuantityText.text = "x " + player.Effects.Yellow;
        _redQuantityText.text = "x " + player.Effects.Red;
        _blackQuantityText.text = "x " + player.Effects.Black;
        _starQuantityText.text = "x " + player.Effects.Star;
    }

    public void DisableAllButtons(bool value) {
        foreach (EffectButton button in _effectButtonsList) {
            button.SetDisabled(value);
        }
    }

    // Кнопки эффектов энейблятся, только если игрок их еще не использовал

    public void TryToEnableAllEffectButtons() {
        if (!MoveControl.Instance.CurrentPlayer.Effects.IsEffectPlaced) {
            DisableAllButtons(false);
        }
    }
}

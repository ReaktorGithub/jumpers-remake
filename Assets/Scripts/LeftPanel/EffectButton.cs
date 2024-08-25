using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectButton : MonoBehaviour
{
    private GameObject _effect;
    private SpriteRenderer _effectRenderer;
    private GameObject _selected;
    private Button _button;
    private bool _isEmpty = false;
    [SerializeField] GameObject _quantityTextObject;
    private TextMeshProUGUI _quantityText;
    private CursorManager _cursorManager;
    [SerializeField] private EControllableEffects effectType;

    private void Awake() {
        _effect = transform.Find("effect").gameObject;
        _selected = transform.Find("effect-selected").gameObject;
        _button = GetComponent<Button>();
        _effectRenderer = _effect.GetComponent<SpriteRenderer>();
        _quantityText = _quantityTextObject.GetComponent<TextMeshProUGUI>();
        _cursorManager = GetComponent<CursorManager>();
    }

    public EControllableEffects EffectType {
        get { return effectType; }
        private set {}
    }

    public void OnSelect() {
        EffectsControl.Instance.SelectedEffect = effectType;
        EffectsControl.Instance.UpdateButtonsSelection();
        EffectsControl.Instance.ActivateSelectionMode();
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public void SetIsEmpty(bool value) {
        _isEmpty = value;
        _effect.SetActive(!value);
        _button.interactable = !value;
    }

    // если кнопка энейблится, но при этом она пустая, то ничего не делать

    public void SetDisabled(bool value) {
        if (!value && _isEmpty) {
            return;
        }
        _button.interactable = !value;
        Color color = new(1f, 1f, 1f, value ? 0.2f : 1f);
        _effectRenderer.color = color;
        _quantityText.color = color;
        _cursorManager.Disabled = value;
    }
}

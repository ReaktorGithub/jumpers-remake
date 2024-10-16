using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectButton : MonoBehaviour
{
    private GameObject _effect, _selected;
    private SpriteRenderer _grind;
    private SpriteRenderer _effectRenderer;
    private Button _button;
    private bool _isEmpty = false;
    [SerializeField] GameObject _quantityTextObject;
    private TextMeshProUGUI _quantityText;
    private CursorManager _cursorManager;
    [SerializeField] private EControllableEffects _effectType;

    private void Awake() {
        _effect = transform.Find("effect").gameObject;
        _selected = transform.Find("effect-selected").gameObject;
        _grind = transform.Find("grind").GetComponent<SpriteRenderer>();
        _button = GetComponent<Button>();
        _effectRenderer = _effect.GetComponent<SpriteRenderer>();
        _quantityText = _quantityTextObject.GetComponent<TextMeshProUGUI>();
        _cursorManager = GetComponent<CursorManager>();
    }

    public EControllableEffects EffectType {
        get { return _effectType; }
        private set {}
    }

    public void OnSelect() {
        EffectsControl.Instance.SelectedEffect = _effectType;
        EffectsControl.Instance.UpdateButtonsSelection();
        EffectsControl.Instance.ActivateSelectionMode();
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public void SetIsEmpty(bool value, int grindLevel) {
        _isEmpty = value;
        _effect.SetActive(!value);
        _button.interactable = !value;
        
        int level = value ? 0 : grindLevel;

        switch(level) {
            case 1: {
                _grind.sprite = CellsControl.Instance.Grind1Sprite;
                break;
            }
            case 2: {
                _grind.sprite = CellsControl.Instance.Grind2Sprite;
                break;
            }
            case 3: {
                _grind.sprite = CellsControl.Instance.Grind3Sprite;
                break;
            }
            default: {
                _grind.sprite = null;
                break;
            }
        }
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

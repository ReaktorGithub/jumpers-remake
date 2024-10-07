using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectButton : MonoBehaviour
{
    private GameObject _effect, _selected, _grind1, _grind2, _grind3;
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
        _grind1 = transform.Find("grind1").gameObject;
        _grind2 = transform.Find("grind2").gameObject;
        _grind3 = transform.Find("grind3").gameObject;
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
        EffectsControl.Instance.ActivatePlaceNewEffectMode(_effectType);
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public void SetIsEmpty(bool value, int grindLevel) {
        _isEmpty = value;
        _effect.SetActive(!value);
        _button.interactable = !value;
        
        int level = value ? 0 : grindLevel;
        _grind1.SetActive(level == 1);
        _grind2.SetActive(level == 2);
        _grind3.SetActive(level == 3);
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

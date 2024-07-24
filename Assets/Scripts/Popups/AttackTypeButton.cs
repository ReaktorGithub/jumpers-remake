using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttackTypeButton : MonoBehaviour
{
    private GameObject _selected, _hover;
    private Image _sprite, _ownImage;
    private Button _button;
    private TextMeshProUGUI _attackNameUI;
    [SerializeField] private EAttackTypes attackType = EAttackTypes.None;
    [SerializeField] private string attackNameText = "";

    private void Awake() {
        _selected = transform.Find("SelectedImage").gameObject;
        _hover = transform.Find("HoverImage").gameObject;
        _attackNameUI = transform.Find("AttackName").gameObject.GetComponent<TextMeshProUGUI>();
        _sprite = transform.Find("Image").gameObject.GetComponent<Image>();
        _ownImage = transform.gameObject.GetComponent<Image>();
        _button = GetComponent<Button>();
        _selected.SetActive(false);
        _hover.SetActive(false);
    }

    public void OnHoverIn() {
        _hover.SetActive(true);
    }

    public void OnHoverOut() {
        _hover.SetActive(false);
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public EAttackTypes AttackType {
        get { return attackType; }
        private set {}
    }

    public string AttackNameText {
        get { return attackNameText; }
        set {
            _attackNameUI.text = value;
        }
    }

    public void SetAsDisabled() {
        _sprite.color = new Color(1f, 1f, 1f, 0.45f);
        _attackNameUI.alpha = 0.45f;
        AttackNameText = "Недоступно";
        _ownImage.raycastTarget = false;
        _button.interactable = false;
    }

    public void SetAsEnabled() {
        _sprite.color = new Color(1f, 1f, 1f, 1f);
        _attackNameUI.alpha = 1f;
        AttackNameText = attackNameText;
        _ownImage.raycastTarget = true;
        _button.interactable = true;
    }
}

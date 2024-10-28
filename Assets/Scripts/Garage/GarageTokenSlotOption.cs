using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GarageTokenSlotOption : MonoBehaviour
{
    private EAbilities _ability = EAbilities.None;
    [SerializeField] GameObject _icon;
    [SerializeField] TextMeshProUGUI _text;
    private Image _bg;

    private void Awake() {
        _bg = GetComponent<Image>();
        OnHoverOut();
    }

    public void SetAbility(EAbilities ability, int level) {
        _ability = ability;

        ManualContent manual = Manual.Instance.GetAbilityManual(ability);
        _text.text = manual.GetEntityNameWithLevel(level);
        _icon.GetComponent<Image>().sprite = manual.Sprite;
    }

    public void OnHoverIn() {
        _bg.color = new Color(1f,1f,1f,0.4f);
    }

    public void OnHoverOut() {
        _bg.color = new Color(1f,1f,1f,0f);
    }

    public void OnClick() {
        // GarageControl.Instance.TabToken.OnTokenSlotOptionClick(_ability);
    }
}

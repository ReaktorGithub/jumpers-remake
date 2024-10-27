using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageTokenAbilityCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private GameObject _iconObject, _chamomileObject, _grindObject;
    private Image _icon, _grind, _bg;
    private EAbilities _ability = EAbilities.None;

    private void Awake() {
        _icon = _iconObject.GetComponent<Image>();
        _grind = _grindObject.GetComponent<Image>();
        _bg = GetComponent<Image>();
    }

    public void BuildContent(EAbilities ability, int level, Sprite grindSprite) {
        _ability = ability;
        ManualContent manual = Manual.Instance.GetAbilityManual(ability);
        _icon.sprite = manual.Sprite;
        _name.text = manual.GetEntityNameWithLevel(level);
        _name.color = new Color32(0,0,0,255);
        _chamomileObject.SetActive(manual.NeedChamomile);
        _grindObject.SetActive(true);
        _grind.sprite = grindSprite;
        _grind.color = grindSprite == null ? new Color32(255,255,255,0) : new Color32(255,255,255,255);
    }

    public void SetSelected(bool value) {
        float alpha = value ? 1f: 0.3f;
        _bg.color = new Color(_bg.color.r, _bg.color.g, _bg.color.b, alpha);
    }

    public void SetDisabled(EAbilities ability) {
        ManualContent manual = Manual.Instance.GetAbilityManual(ability);
        _ability = ability;
        _icon.sprite = manual.Sprite;
        _name.text = "НЕДОСТУПНО";
        _name.color = new Color32(109,0,0,255);
        _chamomileObject.SetActive(false);
        _grindObject.SetActive(false);
        SetSelected(false);
    }

    public void OnClick() {
        GarageControl.Instance.OnAbilityClick(_ability);
    }
}

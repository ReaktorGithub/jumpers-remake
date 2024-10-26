using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageTokenAbilityCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private GameObject _iconObject, _chamomileObject, _grindObject;
    private Image _icon, _grind, _bg;

    private void Awake() {
        _icon = _iconObject.GetComponent<Image>();
        _grind = _grindObject.GetComponent<Image>();
        _bg = GetComponent<Image>();
    }

    public void BuildContent(Sprite icon, string name, bool needChamomile, int level) {
        _icon.sprite = icon;
        _name.text = name;
        _name.color = new Color32(0,0,0,255);
        _chamomileObject.SetActive(needChamomile);
        
        switch(level) {
            case 1: {
                _grind.sprite = CellsControl.Instance.Grind1Sprite;
                _grindObject.SetActive(true);
                break;
            }
            case 2: {
                _grind.sprite = CellsControl.Instance.Grind2Sprite;
                _grindObject.SetActive(true);
                break;
            }
            case 3: {
                _grind.sprite = CellsControl.Instance.Grind3Sprite;
                _grindObject.SetActive(true);
                break;
            }
            default: {
                _grindObject.SetActive(false);
                break;
            }
        }
    }

    public void SetSelected(bool value) {
        float alpha = value ? 1f: 0.3f;
        _bg.color = new Color(_bg.color.r, _bg.color.g, _bg.color.b, alpha);
    }

    public void SetDisabled(Sprite icon) {
        _icon.sprite = icon;
        _name.text = "НЕДОСТУПНО";
        _name.color = new Color32(109,0,0,255);
        _chamomileObject.SetActive(false);
        _grindObject.SetActive(false);
        SetSelected(false);
    }
}

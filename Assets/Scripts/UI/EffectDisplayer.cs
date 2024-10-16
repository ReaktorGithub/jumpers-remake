using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EffectDisplayer : MonoBehaviour
{
    private Image _image, _grind;
    private TextMeshProUGUI _text;

    private void Awake() {
        GameObject imageObject = transform.Find("EffectIcon").gameObject;
        _image = imageObject.GetComponent<Image>();
        _grind = imageObject.transform.Find("grind").GetComponent<Image>();
        _text = transform.Find("EffectName").GetComponent<TextMeshProUGUI>();
    }

    public void BuildContent(string text, Sprite sprite, int level) {
        _image.sprite = sprite;
        
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

        _text.text = text;
    }
}

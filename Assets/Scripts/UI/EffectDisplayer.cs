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

    public void BuildContent(string text, Sprite sprite, Sprite grindSprite) {
        _image.sprite = sprite;
        _grind.sprite = grindSprite;
        _text.text = text;
    }
}

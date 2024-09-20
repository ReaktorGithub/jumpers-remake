using TMPro;
using UnityEngine;

public class TokenIndicator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private TextMeshPro _text;
    private RectTransform _rect;
    private ETokenIndicators _type;

    private void Awake() {
        UpdateLinks();
    }

    public void UpdateLinks() {
        _spriteRenderer = transform.Find("Image").GetComponent<SpriteRenderer>();
        _text = transform.Find("Text").GetComponent<TextMeshPro>();
        _rect = GetComponent<RectTransform>();
    }

    public ETokenIndicators Type {
        get { return _type; }
        set { _type = value; }
    }

    public void SetSprite(Sprite sprite) {
        _spriteRenderer.sprite = sprite;
    }

    public void SetText(float widthSmall, float widthDefault, string newText = "") {
        _text.text = newText;

        if (newText == "") {
            _text.gameObject.SetActive(false);
            _rect.sizeDelta = new Vector2(widthSmall, _rect.sizeDelta.y);
        } else {
            _text.gameObject.SetActive(true);
            _rect.sizeDelta = new Vector2(widthDefault, _rect.sizeDelta.y);
        }
    }

    public void SetTextColor(Color32 color) {
        _text.color = color;
    }
}

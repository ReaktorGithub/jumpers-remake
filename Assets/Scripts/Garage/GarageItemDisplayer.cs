using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GarageItemDisplayer : MonoBehaviour
{
    private Image _image;
    private TextMeshProUGUI _text;

    private void Awake() {
        _image = transform.Find("Image").GetComponent<Image>();
        _text = transform.Find("CostText").GetComponent<TextMeshProUGUI>();
    }

    public void SetCost(int value) {
        _text.text = value.ToString();
    }

    public void SetImage(Sprite sprite) {
        _image.sprite = sprite;
    }
}

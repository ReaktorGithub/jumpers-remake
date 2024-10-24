using UnityEngine;
using UnityEngine.UI;

public class GarageOwnedTokenButton : MonoBehaviour
{
    private Image _image;
    [SerializeField] private GameObject _selected, _hover, _imageObject;

    private void Awake() {
        _image = _imageObject.GetComponent<Image>();
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

    public void SetImage(Sprite sprite) {
        _image.sprite = sprite;
    }
}

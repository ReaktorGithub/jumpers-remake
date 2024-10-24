using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class GarageShopTokenButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tokenName, _costText;
    [SerializeField] private GameObject _imageObject, _garageTabShopObject;
    private GarageTabShop _garageTabShop;
    private GarageShopToken _token;
    private Image _image;
    private Image _bg;

    private void Awake() {
        _image = _imageObject.GetComponent<Image>();
        _bg = GetComponent<Image>();
        _garageTabShop = _garageTabShopObject.GetComponent<GarageTabShop>();
    }

    public GarageShopToken Token {
        get { return _token; }
        private set {}
    }

    public void SetToken(GarageShopToken token) {
        _token = token;
        _image.sprite = token.TokenSprite;
        _tokenName.text = token.Name;
        _costText.text = token.Cost.ToString();
    }

    public void SetSelected(bool value) {
        _bg.color = new Color(_bg.color.r, _bg.color.g, _bg.color.b, value ? 1f : 0.27f);
    }

    public void OnClick() {
        _garageTabShop.OnSelectToken(_token);
    }
}

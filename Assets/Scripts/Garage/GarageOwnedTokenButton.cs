using UnityEngine;
using UnityEngine.UI;

public class GarageOwnedTokenButton : MonoBehaviour
{
    private PlayerTokenInGarage _garageToken;
    [SerializeField] private GameObject _selected, _imageObject;

    public PlayerTokenInGarage GarageToken {
        get { return _garageToken; }
        private set {}
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public void SetGarageToken(PlayerTokenInGarage garageToken) {
        _garageToken = garageToken;
        _imageObject.GetComponent<Image>().sprite = garageToken.Token.TokenSprite;
    }

    public void OnClick() {
        GarageControl.Instance.TabToken.OnOwnedTokenButtonClick(_garageToken);
    }
}

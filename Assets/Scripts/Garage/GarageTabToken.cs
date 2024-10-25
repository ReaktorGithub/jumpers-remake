using System.Collections.Generic;
using UnityEngine;

public class GarageTabToken : MonoBehaviour
{
    [SerializeField] private GameObject _ownedTokenButtonSample, _ownedListObject, _bigTokenObject;
    private GarageBigToken _bigToken;

    private void Awake() {
        _bigToken = _bigTokenObject.GetComponent<GarageBigToken>();
    }

    private void Start() {
        _ownedTokenButtonSample.SetActive(false);
    }

    public void BuildContent() {
        PlayerControl player = GarageControl.Instance.Player;

        foreach(Transform child in _ownedListObject.transform) {
            if (child.TryGetComponent(out GarageOwnedTokenButton button)) {
                Destroy(button.gameObject);
            }
        }

        List<PlayerTokenInGarage> ownedList = player.GetAllGarageTokens();
        List<PlayerTokenInGarage> sortedList = GarageControl.Instance.GetSortedPlayerTokensInGarage(ownedList);

        foreach(PlayerTokenInGarage token in sortedList) {
            GameObject clone = Instantiate(_ownedTokenButtonSample);
            GarageOwnedTokenButton button = clone.GetComponent<GarageOwnedTokenButton>();
            button.SetGarageToken(token);
            clone.transform.SetParent(_ownedListObject.transform);
            clone.transform.localScale = new Vector3(1f,1f,1f);
            clone.SetActive(true);
        }

        UpdateContent();
    }

    private void UpdateContent() {
        foreach(Transform child in _ownedListObject.transform) {
            if (child.TryGetComponent(out GarageOwnedTokenButton button)) {
                bool selected = button.GarageToken.Selected;
                button.SetSelected(selected);
                if (selected) {
                    PlayerControl player = GarageControl.Instance.Player;
                    Sprite symbolSprite = player.GetTokenControl().GetTokenSymbolSprite();
                    _bigToken.SetToken(button.GarageToken.Token, symbolSprite);
                }
            }
        }
    }

    public void OnOwnedTokenButtonClick(PlayerTokenInGarage garageToken) {
        GarageControl.Instance.Player.ReselectTokens(garageToken.Token);
        UpdateContent();
    }

    public void StartAllAnimations() {
        _bigToken.SetSqueezeAnimation(true);
    }
}

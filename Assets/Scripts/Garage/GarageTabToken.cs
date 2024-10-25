using System.Collections.Generic;
using UnityEngine;

public class GarageTabToken : MonoBehaviour
{
    [SerializeField] private GameObject _ownedTokenButtonSample, _ownedListObject;

    private void Start() {
        _ownedTokenButtonSample.SetActive(false);
    }

    public void BuildContent(PlayerControl player) {
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
            button.SetSelected(token.Selected);
            clone.transform.SetParent(_ownedListObject.transform);
            clone.transform.localScale = new Vector3(1f,1f,1f);
            clone.SetActive(true);
        }
    }

    private void UpdateOwnedTokensSelection() {
        foreach(Transform child in _ownedListObject.transform) {
            if (child.TryGetComponent(out GarageOwnedTokenButton button)) {
                button.SetSelected(button.GarageToken.Selected);
            }
        }
    }

    public void OnOwnedTokenButtonClick(PlayerTokenInGarage garageToken) {
        GarageControl.Instance.Player.ReselectTokens(garageToken.Token);
        UpdateOwnedTokensSelection();
    }
}

using System.Collections.Generic;
using UnityEngine;

public class GarageTabToken : MonoBehaviour
{
    [SerializeField] private GameObject _ownedTokenButtonSample, _ownedListObject;
    private PlayerTokenInGarage _selectedGarageToken;

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
            if (_selectedGarageToken == null) {
                _selectedGarageToken = token;
            }
            button.SetSelected(_selectedGarageToken == token);
            clone.transform.SetParent(_ownedListObject.transform);
            clone.transform.localScale = new Vector3(1f,1f,1f);
            clone.SetActive(true);
        }
    }

    private void UpdateOwnedTokensSelection() {
        foreach(Transform child in _ownedListObject.transform) {
            if (child.TryGetComponent(out GarageOwnedTokenButton button)) {
                button.SetSelected(_selectedGarageToken == button.GarageToken);
            }
        }
    }

    public void OnOwnedTokenButtonClick(PlayerTokenInGarage garageToken) {
        _selectedGarageToken = garageToken;
        UpdateOwnedTokensSelection();
    }
}

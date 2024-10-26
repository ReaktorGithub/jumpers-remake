using System.Collections.Generic;
using UnityEngine;

public class GarageTabToken : MonoBehaviour
{
    [SerializeField] private GameObject _ownedTokenButtonSample, _ownedListObject, _bigTokenObject, _abilitiesListObject;
    private GarageBigToken _bigToken;
    [SerializeField] private List<GameObject> _slotButtonsListObjects = new();
    private List<GarageTokenSlotButton> _slotButtonsList = new();
    private PlayerTokenInGarage _selectedGarageToken;
    private List<GarageTokenAbilityCard> _cardsList = new();
    [SerializeField] private List<EAbilities> _allAbilitiesList = new();

    private void Awake() {
        _bigToken = _bigTokenObject.GetComponent<GarageBigToken>();
        foreach(GameObject obj in _slotButtonsListObjects) {
            _slotButtonsList.Add(obj.GetComponent<GarageTokenSlotButton>());
        }

        Transform[] children = _abilitiesListObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.TryGetComponent(out GarageTokenAbilityCard card)) {
                _cardsList.Add(card);
            }
        }
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
            _selectedGarageToken = token;
            clone.transform.SetParent(_ownedListObject.transform);
            clone.transform.localScale = new Vector3(1f,1f,1f);
            clone.SetActive(true);
        }

        List<EAbilities> permittedAbilities = player.GetAllPermittedAbilities();

        for (int i = 0; i < _allAbilitiesList.Count; i++) {
            EAbilities ability = _allAbilitiesList[i];
            ManualContent manual = Manual.Instance.GetAbilityManual(ability);

            if (permittedAbilities.Contains(ability)) {
                int level = player.Grind.GetAbilityLevel(ability);
                _cardsList[i].BuildContent(
                    manual.Sprite,
                    manual.GetEntityNameWithLevel(level),
                    manual.NeedChamomile,
                    level
                );
            } else {
                _cardsList[i].SetDisabled(manual.Sprite);
            }
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

        for (int i = 0; i < _slotButtonsList.Count; i++) {
            PlayerTokenSlot slot = _selectedGarageToken.SlotsList[i];
            _slotButtonsList[i].UpdateContent(slot);
        }

        List<EAbilities> placedAbilities = _selectedGarageToken.GetAllPlacedAbilities();

        for (int i = 0; i < _allAbilitiesList.Count; i++) {
            EAbilities ability = _allAbilitiesList[i];
            bool isSelected = placedAbilities.Contains(ability);
            _cardsList[i].SetSelected(isSelected);
        }
    }

    public void OnOwnedTokenButtonClick(PlayerTokenInGarage garageToken) {
        GarageControl.Instance.Player.ReselectTokens(garageToken.Token);
        _selectedGarageToken = garageToken;
        UpdateContent();
    }

    public void StartAllAnimations() {
        _bigToken.SetSqueezeAnimation(true);
    }
}

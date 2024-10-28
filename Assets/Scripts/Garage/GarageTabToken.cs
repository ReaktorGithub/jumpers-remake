using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageTabToken : MonoBehaviour
{
    [SerializeField] private GameObject _ownedTokenButtonSample, _ownedListObject, _bigTokenObject, _abilitiesListObject, _detailsBox, _detailsIcon, _detailsGrindIcon, _detailsChamomile, _detailsButtonSetObject, _detailsButtonRemoveObject;
    [SerializeField] private TextMeshProUGUI _detailsEmptyText, _detailsName, _detailsText, _tokenName, _tokenType, _tokenPower, _tokenSlots;
    private GarageBigToken _bigToken;
    [SerializeField] private List<GameObject> _slotButtonsListObjects = new();
    private List<GarageTokenSlotButton> _slotButtonsList = new();
    private PlayerTokenInGarage _selectedGarageToken;
    private List<GarageTokenAbilityCard> _cardsList = new();
    [SerializeField] private List<EAbilities> _allAbilitiesList = new();
    [SerializeField] private EAbilities _selectedAbility = EAbilities.None;

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

    public EAbilities SelectedAbility {
        get { return _selectedAbility; }
        set { _selectedAbility = value; }
    }

    public void BuildContent() {
        PlayerControl player = GarageControl.Instance.Player;

        _selectedAbility = EAbilities.None;

        // Построение списка фишек во владении

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

        UpdateContent();
    }

    private void UpdateContent() {
        PlayerControl player = GarageControl.Instance.Player;
        PlayerTokenInGarage garageToken = player.GetSelectedPlayerTokenInGarage();

        // Построение списка навыков

        List<EAbilities> permittedAbilities = player.GetAllPermittedAbilities();

        for (int i = 0; i < _allAbilitiesList.Count; i++) {
            EAbilities ability = _allAbilitiesList[i];

            if (permittedAbilities.Contains(ability)) {
                int grindLevel = player.Grind.GetAbilityLevel(ability);
                Sprite sprite = player.Grind.GetGrindSprite(grindLevel);
                _cardsList[i].BuildContent(ability, grindLevel, sprite);
            } else {
                _cardsList[i].SetDisabled(ability);
            }
        }

        // Фишки во владении

        foreach(Transform child in _ownedListObject.transform) {
            if (child.TryGetComponent(out GarageOwnedTokenButton button)) {
                bool selected = button.GarageToken.Selected;
                button.SetSelected(selected);
                if (selected) {
                    Sprite symbolSprite = player.GetTokenControl().GetTokenSymbolSprite();
                    _bigToken.SetToken(button.GarageToken.Token, symbolSprite);
                }
            }
        }

        // Детали о фишке
        
        GarageShopToken token = _selectedGarageToken.Token;
        _tokenName.text = token.Name;
        _tokenType.text = GarageControl.Instance.GetTokenTypeText(token.Type);
        _tokenPower.text = token.InitialPower.ToString();
        _tokenSlots.text = garageToken.GetEnabledSlotsCount().ToString();

        // Слоты фишки

        for (int i = 0; i < _slotButtonsList.Count; i++) {
            PlayerTokenSlot slot = _selectedGarageToken.SlotsList[i];
            _slotButtonsList[i].UpdateContent(slot);
        }

        // Список навыков

        List<EAbilities> placedAbilities = _selectedGarageToken.GetAllPlacedAbilities();

        for (int i = 0; i < _allAbilitiesList.Count; i++) {
            EAbilities ability = _allAbilitiesList[i];
            bool isSelected = placedAbilities.Contains(ability);
            _cardsList[i].SetSelected(isSelected);
        }

        // Подробности о выбранном навыке

        bool isSomeSelected = _selectedAbility != EAbilities.None;

        _detailsEmptyText.gameObject.SetActive(!isSomeSelected);
        _detailsBox.SetActive(isSomeSelected);

        if (!isSomeSelected) {
            return;
        }

        bool isAbilityPlaced = _selectedGarageToken.IsAbilityPlaced(_selectedAbility);
        bool isAbilityEnabled = IsAbilityEnabled(_selectedAbility);

        ManualContent manual = Manual.Instance.GetAbilityManual(_selectedAbility);
        int level = player.Grind.GetAbilityLevel(_selectedAbility);

        _detailsIcon.GetComponent<Image>().sprite = manual.Sprite;
        _detailsButtonSetObject.SetActive(isAbilityEnabled && !isAbilityPlaced);
        _detailsButtonRemoveObject.SetActive(isAbilityEnabled && isAbilityPlaced);

        if (!isAbilityEnabled) {
            _detailsName.text = "НЕДОСТУПНО";
            _detailsName.color = new Color32(109,0,0,255);
            _detailsText.text = manual.UnlockCondition;
            _detailsChamomile.SetActive(false);
            _detailsGrindIcon.SetActive(false);
            return;
        } else {
            _detailsName.text = manual.GetEntityNameWithLevel(level);
            _detailsName.color = new Color32(255,255,255,255);
        }
        
        Sprite grindSprite = player.Grind.GetGrindSprite(level);
        _detailsGrindIcon.GetComponent<Image>().sprite = grindSprite;
        _detailsGrindIcon.SetActive(grindSprite != null);
        _detailsText.text = manual.GetDescriptionAndAdditionalInfo(level == 0 ? 1 : level);
        _detailsChamomile.SetActive(manual.NeedChamomile);
    }

    private bool IsAbilityEnabled(EAbilities ability) {
        List<EAbilities> permittedAbilities = GarageControl.Instance.Player.GetAllPermittedAbilities();
        return permittedAbilities.Contains(ability);
    }

    public void OnOwnedTokenButtonClick(PlayerTokenInGarage garageToken) {
        GarageControl.Instance.Player.ReselectTokens(garageToken.Token);
        _selectedGarageToken = garageToken;
        _selectedAbility = EAbilities.None;
        UpdateContent();
    }

    public void StartAllAnimations() {
        _bigToken.SetSqueezeAnimation(true);
    }

    public void OnSelectAbilityCard(EAbilities ability) {
        _selectedAbility = ability;
        UpdateContent();
    }

    public void OnSetAbility() {
        bool isSuccess = _selectedGarageToken.PlaceAbility(_selectedAbility);

        if (isSuccess) {
            UpdateContent();
        } else {
            GarageControl.Instance.Player.OpenShopLackOfSlotsModal();
        }
    }

    public void OnRemoveAbility() {
        bool isSuccess = _selectedGarageToken.RemoveAbility(_selectedAbility);

        if (isSuccess) {
            UpdateContent();
        } else {
            GarageControl.Instance.Player.OpenShopRemoveAbilityNotSuccessModal();
        }
    }
}

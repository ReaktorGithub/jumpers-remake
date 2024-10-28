using System.Collections.Generic;
using UnityEngine;

public class GarageControl : MonoBehaviour
{
    public static GarageControl Instance { get; private set; }
    [SerializeField] private EGarageTabs _currentTab = EGarageTabs.Token;
    [SerializeField] private List<GarageTabButton> _tabButtonsList = new();
    [SerializeField] private GameObject _garageBody, _shopTabObject, _awardsTabObject, _boostersTabObject, _grindTabObject, _tokenTabObject, _tokensListObject;
    [SerializeField] private int _slotsForBuyCount = 2;
    [SerializeField] private int _slotCost = 500;
    private List<GarageShopToken> _shopTokensList = new(); // список всех фишек в игре
    private PlayerControl _player;
    private GarageTabShop _tabShop;
    private GarageTabAwards _tabAwards;
    private GarageTabBoosters _tabBoosters;
    private GarageTabGrind _tabGrind;
    private GarageTabToken _tabToken;
    private ModalByuItem _modalBuyItem;
    private ModalSellItem _modalSellItem;
    private Sprite _newSlotSprite;
    private EGarageProductTypes _productType;

    private void Awake() {
        Instance = this;
        _tabShop = _shopTabObject.GetComponent<GarageTabShop>();
        _tabAwards = _awardsTabObject.GetComponent<GarageTabAwards>();
        _tabBoosters = _boostersTabObject.GetComponent<GarageTabBoosters>();
        _tabGrind = _grindTabObject.GetComponent<GarageTabGrind>();
        _tabToken = _tokenTabObject.GetComponent<GarageTabToken>();
        _modalBuyItem = GameObject.Find("GarageScripts").GetComponent<ModalByuItem>();
        _newSlotSprite = GameObject.Find("Instances").transform.Find("ability-node-plus").GetComponent<SpriteRenderer>().sprite;
        _modalSellItem = GameObject.Find("GarageScripts").GetComponent<ModalSellItem>();

        foreach(Transform child in _tokensListObject.transform) {
            if (child.TryGetComponent(out GarageShopToken token)) {
                _shopTokensList.Add(token);
            }
        }
    }

    public List<GarageShopToken> ShopTokensList {
        get { return _shopTokensList; }
        private set {}
    }

    public int SlotsForBuyCount {
        get { return _slotsForBuyCount; }
        private set {}
    }

    public EGarageProductTypes ProductType {
        get { return _productType; }
        private set {}
    }

    public int SlotCost {
        get { return _slotCost; }
        private set {}
    }

    public PlayerControl Player {
        get { return _player; }
        private set {}
    }

    public GarageTabShop TabShop {
        get { return _tabShop; }
        private set {}
    }

    public GarageTabAwards TabAwards {
        get { return _tabAwards; }
        private set {}
    }

    public GarageTabBoosters TabBoosters {
        get { return _tabBoosters; }
        private set {}
    }

    public GarageTabGrind TabGrind {
        get { return _tabGrind; }
        private set {}
    }

    public GarageTabToken TabToken {
        get { return _tabToken; }
        private set {}
    }

    public void ShowBody(bool value) {
        _garageBody.SetActive(value);
        StartAllAnimations();
    }

    public void BuildContent(PlayerControl player) {
        _player = player;
        UpdateTabButtonsDisplay();
        UpdateTabContentDisplay();
    }

    public void OnTabClick(EGarageTabs tab) {
        _currentTab = tab;
        UpdateTabButtonsDisplay();
        UpdateTabContentDisplay();
    }

    private void UpdateTabButtonsDisplay() {
        foreach(GarageTabButton button in _tabButtonsList) {
            button.SetSelected(button.Tab == _currentTab);
        }
    }

    public void UpdateTabContentDisplay() {
        switch(_currentTab) {
            case EGarageTabs.Token: {
                _tabToken.BuildContent();
                break;
            }
            case EGarageTabs.Shop: {
                _tabShop.BuildContent();
                break;
            }
            case EGarageTabs.Boosters: {
                _tabBoosters.BuildContent();
                break;
            }
            case EGarageTabs.Grind: {
                _tabGrind.BuildContent();
                break;
            }
            case EGarageTabs.Awards: {
                _tabAwards.BuildContent();
                break;
            }
        }

        _shopTabObject.SetActive(_currentTab == EGarageTabs.Shop);
        _tokenTabObject.SetActive(_currentTab == EGarageTabs.Token);
        _boostersTabObject.SetActive(_currentTab == EGarageTabs.Boosters);
        _grindTabObject.SetActive(_currentTab == EGarageTabs.Grind);
        _awardsTabObject.SetActive(_currentTab == EGarageTabs.Awards);

        StartAllAnimations();
    }

    public string GetTokenTypeText(ETokenTypes type) {
        return type switch
        {
            ETokenTypes.Standart => "Стандарт",
            ETokenTypes.Professional => "Профи",
            ETokenTypes.Elite => "Элита",
            _ => "Базовая",
        };
    }

    public List<GarageShopToken> GetSortedGarageShopTokens() {
        List<GarageShopToken> array = new();

        foreach(GarageShopToken token in _shopTokensList) {
            array.Add(token);
        }

        array.Sort((a, b) => a.SortingOrder - b.SortingOrder);

        return array;
    }

    public List<PlayerTokenInGarage> GetSortedPlayerTokensInGarage(List<PlayerTokenInGarage> list) {
        List<PlayerTokenInGarage> array = new();

        foreach(PlayerTokenInGarage token in list) {
            array.Add(token);
        }

        array.Sort((a, b) => a.Token.SortingOrder - b.Token.SortingOrder);

        return array;
    }

    public void OnProductBuy() {
        switch(_productType) {
            case EGarageProductTypes.Token: {
                AddNewTokenToGarage();
                break;
            }
            case EGarageProductTypes.Slot: {
                AddNewSlotToToken();
                break;
            }
            default: {
                break;
            }
        }
    }

    public void OnProductSell() {
        switch(_productType) {
            case EGarageProductTypes.Token: {
                SellToken();
                break;
            }
            default: {
                break;
            }
        }
    }

    private void AddNewTokenToGarage() {
        _player.AddNewTokenToGarage(TabShop.SelectedToken);
        OnTabClick(EGarageTabs.Token);
    }

    private void SellToken() {
        _player.SellToken();
        // UpdateTabContentDisplay происходит в корутине
    }

    private void AddNewSlotToToken() {
        _player.AddNewSlotToToken();
        UpdateTabContentDisplay();
    }

    public void OnBuyToken() {
        if (_player.IsTokenInGarageAlreadyExist(TabShop.SelectedToken)) {
            _player.OpenShopTokenAlreadyExistModal();
            return;
        }

        int cost = TabShop.SelectedToken.Cost;

        if (cost > _player.Coins) {
            _player.OpenShopLackOfCoinsModal();
        } else {
            _productType = EGarageProductTypes.Token;
            _modalBuyItem.BuildContent(TabShop.SelectedToken.Name, cost, TabShop.SelectedToken.TokenSprite);
            _modalBuyItem.OpenModal();
        }
    }

    public void OnBuySlot() {
        if (_slotCost > _player.Coins) {
            _player.OpenShopLackOfCoinsModal();
        } else {
            _productType = EGarageProductTypes.Slot;
            _modalBuyItem.BuildContent("Слот для навыков", _slotCost, _newSlotSprite);
            _modalBuyItem.OpenModal();
        }
    }

    public void OnSellToken() {
        _productType = EGarageProductTypes.Token;
        GarageShopToken token = _player.GetSelectedPlayerTokenInGarage().Token;

        if (token.Type == ETokenTypes.Base) {
            _player.OpenShopSellBaseTokenModal();
        } else {
            _productType = EGarageProductTypes.Token;
            _modalSellItem.BuildContent(token.Name, token.SellCost, token.TokenSprite);
            _modalSellItem.OpenModal();
        }
    }

    private void StartAllAnimations() {
        TabToken.StartAllAnimations();
    }

    public Sprite GetAbilitySprite(EAbilities ability) {
        switch(ability) {
            case EAbilities.AttackUsual: {
                return Manual.Instance.AttackUsual.SpriteAlt;
            }
            case EAbilities.Hammer: {
                return Manual.Instance.AbilityHammer.SpriteAlt;
            }
            case EAbilities.Knockout: {
                return Manual.Instance.AttackKnockout.SpriteAlt;
            }
            case EAbilities.LastChance: {
                return Manual.Instance.AttackUsual.SpriteAlt;
            }
            case EAbilities.MagicKick: {
                return Manual.Instance.AttackUsual.SpriteAlt;
            }
            case EAbilities.Oreol: {
                return Manual.Instance.AttackUsual.SpriteAlt;
            }
            case EAbilities.Soap: {
                return Manual.Instance.AttackUsual.SpriteAlt;
            }
            default: return null;
        }
    }
}

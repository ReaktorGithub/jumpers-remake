using System.Collections.Generic;
using UnityEngine;

public class GarageControl : MonoBehaviour
{
    public static GarageControl Instance { get; private set; }
    [SerializeField] private EGarageTabs _currentTab = EGarageTabs.Token;
    [SerializeField] private List<GarageTabButton> _tabButtonsList = new();
    [SerializeField] private GameObject _garageBody, _shopTabObject, _awardsTabObject, _boostersTabObject, _grindTabObject, _tokenTabObject, _tokensListObject;
    private List<GarageShopToken> _shopTokensList = new(); // список всех фишек в игре
    private PlayerControl _player;
    private GarageTabShop _tabShop;
    private GarageTabAwards _tabAwards;
    private GarageTabBoosters _tabBoosters;
    private GarageTabGrind _tabGrind;
    private GarageTabToken _tabToken;
    private ModalByuItem _modalBuyItem;

    private void Awake() {
        Instance = this;
        _tabShop = _shopTabObject.GetComponent<GarageTabShop>();
        _tabAwards = _awardsTabObject.GetComponent<GarageTabAwards>();
        _tabBoosters = _boostersTabObject.GetComponent<GarageTabBoosters>();
        _tabGrind = _grindTabObject.GetComponent<GarageTabGrind>();
        _tabToken = _tokenTabObject.GetComponent<GarageTabToken>();
        _modalBuyItem = GameObject.Find("ModalScripts").GetComponent<ModalByuItem>();

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

    private void UpdateTabContentDisplay() {
        switch(_currentTab) {
            case EGarageTabs.Token: {
                _tabToken.BuildContent(_player);
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

    public void AddNewTokenToGarage() {
        // todo IsTokenInGarageAlreadyExist
        _player.AddNewTokenToGarage(TabShop.SelectedToken);
        OnTabClick(EGarageTabs.Token);
    }

    public void OnBuyToken() {
        int cost = TabShop.SelectedToken.Cost;

        if (cost > _player.Coins) {
            _player.OpenShopLackOfCoinsModal();
        } else {
            _modalBuyItem.BuildContent(TabShop.SelectedToken.Name, cost, TabShop.SelectedToken.TokenSprite);
            _modalBuyItem.OpenModal();
        }
    }
}

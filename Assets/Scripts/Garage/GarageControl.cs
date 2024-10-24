using System.Collections.Generic;
using UnityEngine;

public class GarageControl : MonoBehaviour
{
    public static GarageControl Instance { get; private set; }
    [SerializeField] private EGarageTabs _currentTab = EGarageTabs.Token;
    [SerializeField] private List<GarageTabButton> _tabButtonsList = new();
    [SerializeField] private GameObject _garageBody, _shopTabObject, _awardsTabObject, _boostersTabObject, _grindTabObject, _tokenTabObject;
    private PlayerControl _player;
    private GarageTabShop _shopTab;
    private GarageTabAwards _awardsTab;
    private GarageTabBoosters _boostersTab;
    private GarageTabGrind _grindTab;
    private GarageTabToken _tokenTab;

    private void Awake() {
        Instance = this;
        _shopTab = _shopTabObject.GetComponent<GarageTabShop>();
        _awardsTab = _awardsTabObject.GetComponent<GarageTabAwards>();
        _boostersTab = _boostersTabObject.GetComponent<GarageTabBoosters>();
        _grindTab = _grindTabObject.GetComponent<GarageTabGrind>();
        _tokenTab = _tokenTabObject.GetComponent<GarageTabToken>();
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
                _tokenTab.BuildContent();
                break;
            }
            case EGarageTabs.Shop: {
                _shopTab.BuildContent();
                break;
            }
            case EGarageTabs.Boosters: {
                _boostersTab.BuildContent();
                break;
            }
            case EGarageTabs.Grind: {
                _grindTab.BuildContent();
                break;
            }
            case EGarageTabs.Awards: {
                _awardsTab.BuildContent();
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
}

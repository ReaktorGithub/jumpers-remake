using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageTabShop : MonoBehaviour
{
    [SerializeField] private GameObject _shopTokenButtonSample, _shopTokensList, _itemDisplayerObject, _abilityMagicKick, _abilityLastChance, _abilityOreol, _abilityKnockout, _abilityHammer, _tokenIconSample, _tokenIconsList;
    
    private GarageShopToken _selectedToken;
    private GarageItemDisplayer _itemDisplayer;
    [SerializeField] private TextMeshProUGUI _detailName, _detailType, _detailPower, _detailSlots, _description, _playerBalance;

    private void Awake() {
        _itemDisplayer = _itemDisplayerObject.GetComponent<GarageItemDisplayer>();
    }

    private void Start() {
        _shopTokenButtonSample.SetActive(false);
        _tokenIconSample.SetActive(false);
    }

    public GarageShopToken SelectedToken {
        get { return _selectedToken; }
        private set {}
    }

    public void BuildContent() {
        foreach(Transform child in _shopTokensList.transform) {
            if (child.TryGetComponent(out GarageShopTokenButton button)) {
                Destroy(button.gameObject);
            }
        }

        List<GarageShopToken> sorted = GarageControl.Instance.GetSortedGarageShopTokens();

        foreach(GarageShopToken token in sorted) {
            if (!token.EnableInShop) {
                continue;
            }

            GameObject clone = Instantiate(_shopTokenButtonSample);
            GarageShopTokenButton button = clone.GetComponent<GarageShopTokenButton>();
            button.UpdateLinks();
            button.SetToken(token);
            clone.transform.SetParent(_shopTokensList.transform);
            clone.transform.localScale = new Vector3(1f,1f,1f);
            clone.SetActive(true);

            if (!_selectedToken) {
                _selectedToken = token;
            }
        }

        UpdateContent();
    }

    public void OnSelectToken(GarageShopToken token) {
        _selectedToken = token;
        UpdateContent();
    }

    private void UpdateContent() {
        PlayerControl player = GarageControl.Instance.Player;

        foreach(Transform child in _shopTokensList.transform) {
            if (child.TryGetComponent(out GarageShopTokenButton button)) {
                button.SetSelected(_selectedToken == button.Token);
            }
        }

        _itemDisplayer.SetCost(_selectedToken.Cost);
        _itemDisplayer.SetImage(_selectedToken.TokenSprite);

        _detailName.text = _selectedToken.Name;
        _detailType.text = GarageControl.Instance.GetTokenTypeText(_selectedToken.Type);
        _detailPower.text = _selectedToken.InitialPower.ToString();
        _detailSlots.text = _selectedToken.InitialAbilitySlots.ToString();
        
        _abilityHammer.SetActive(_selectedToken.UnlockAbilities.Contains(EAbilities.Hammer));
        _abilityKnockout.SetActive(_selectedToken.UnlockAbilities.Contains(EAbilities.Knockout));
        _abilityLastChance.SetActive(_selectedToken.UnlockAbilities.Contains(EAbilities.LastChance));
        _abilityMagicKick.SetActive(_selectedToken.UnlockAbilities.Contains(EAbilities.MagicKick));
        _abilityOreol.SetActive(_selectedToken.UnlockAbilities.Contains(EAbilities.Oreol));

        _description.text = _selectedToken.Description;

        _playerBalance.text = player.Coins.ToString();

        // Список своих фишек

        foreach(Transform child in _tokenIconsList.transform) {
            if (child.TryGetComponent(out Image image)) {
                Destroy(image.gameObject);
            }
        }

        List<PlayerTokenInGarage> tokens = player.GetAllGarageTokens();

        foreach(PlayerTokenInGarage garageToken in tokens) {
            GameObject clone = Instantiate(_tokenIconSample);
            clone.GetComponent<Image>().sprite = garageToken.Token.TokenSprite;
            clone.transform.SetParent(_tokenIconsList.transform);
            clone.transform.localScale = new Vector3(1f,1f,1f);
            clone.SetActive(true);
        }
    }
}

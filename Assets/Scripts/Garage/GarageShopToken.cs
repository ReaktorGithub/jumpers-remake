using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GarageShopToken : MonoBehaviour
{
    private Sprite _tokenSprite;
    [SerializeField] private string _name = "";
    [SerializeField] private ETokenTypes _type = ETokenTypes.Base;
    [SerializeField] private int _initialPower = 2;
    [SerializeField] private int _initialAbilitySlots = 3;
    [SerializeField] private List<EAbilities> _unlockAbilities = new();
    [SerializeField] private int _cost = 0;
    [SerializeField] private int _sellCost = 0;
    [SerializeField] private string _description = "";
    [SerializeField] private int _sortingOrder = 0;
    [SerializeField] private bool _enableInShop = true;

    private void Awake() {
        _tokenSprite = GetComponent<Image>().sprite;
    }

    public Sprite TokenSprite {
        get { return _tokenSprite; }
        private set {}
    }

    public string Name {
        get { return _name; }
        private set {}
    }

    public ETokenTypes Type {
        get { return _type; }
        private set {}
    }

    public int InitialPower {
        get { return _initialPower; }
        private set {}
    }

    public int InitialAbilitySlots {
        get { return _initialAbilitySlots; }
        private set {}
    }

    public List<EAbilities> UnlockAbilities {
        get { return _unlockAbilities; }
        private set {}
    }

    public int Cost {
        get { return _cost; }
        private set {}
    }

    public int SellCost {
        get { return _sellCost; }
        private set {}
    }

    public string Description {
        get { return _description; }
        private set {}
    }

    public int SortingOrder {
        get { return _sortingOrder; }
        private set {}
    }

    public bool EnableInShop {
        get { return _enableInShop; }
        private set {}
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ManualContent : MonoBehaviour
{
    [SerializeField] private string _entityName;
    private Sprite _sprite;
    [SerializeField] private List<string> _shortDescriptionLevel1 = new();
    [SerializeField] private List<string> _shortDescriptionLevel2 = new();
    [SerializeField] private List<string> _shortDescriptionLevel3 = new();
    [SerializeField] private List<string> _additionalInfo = new();
    [SerializeField] private List<int> _cost = new();
    [SerializeField] private EResourceTypes _costResourceType;
    [SerializeField] private EResourceCharacters _character;
    [SerializeField] private List<int> _causeEffect = new();

    private void Awake() {
        _sprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void Start() {
        transform.gameObject.SetActive(false);
    }

    public Sprite Sprite {
        get { return _sprite; }
        private set {}
    }

    public EResourceTypes CostResourceType {
        get { return _costResourceType; }
        private set {}
    }

    public EResourceCharacters Character {
        get { return _character; }
        private set {}
    }

    public string GetEntityName(bool lowercase = false) {
        return lowercase && _entityName != null ? _entityName.ToLower() : _entityName;
    }
    
    public string GetEntityNameWithLevel(int level, bool lowercase = false) {
        if (level == 0) {
            return GetEntityName(lowercase);
        }
        string suffix = " (ур." + level + ")";
        string myEntityName = GetEntityName(lowercase);
        return myEntityName + suffix;
    }

    public string GetShortDescription(int level = 1) {
        List<string> found;

        switch(level) {
            case 2: {
                found = _shortDescriptionLevel2;
                break;
            }
            case 3: {
                found = _shortDescriptionLevel3;
                break;
            }
            default: {
                found = _shortDescriptionLevel1;
                break;
            }
        }

        string result = "";

        for (int i = 0; i < found.Count; i++) {
            string myText = found.Count == i - 1 ? found[i] : found[i] + "<br><br>";
            result += myText;
        }

        return result;
    }

    public string GetAdditionalInfo(int level = 1) {
        return _additionalInfo[level - 1];
    }

    public int GetCost(int level = 1) {
        return _cost[level - 1];
    }

    public int GetCauseEffect(int level = 1) {
        return _causeEffect[level - 1];
    }
}

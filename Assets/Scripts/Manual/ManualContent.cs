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
    [SerializeField] private List<int> _costToReplaceEffect = new();
    [SerializeField] private EResourceTypes _replaceEffectResourceType;
    [SerializeField] private EResourceCharacters _character;

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

    public EResourceTypes ReplaceEffectResourceType {
        get { return _replaceEffectResourceType; }
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
        string suffix = " (ур. " + level + ")";
        string myEntityName = GetEntityName(lowercase);
        return myEntityName + suffix;
    }

    public List<string> GetShortDescription(int level = 1) {
        switch(level) {
            case 1: {
                return _shortDescriptionLevel1;
            }
            case 2: {
                return _shortDescriptionLevel2;
            }
            default: {
                return _shortDescriptionLevel3;
            }
        }
    }

    public string GetAdditionalInfo(int level = 1) {
        return _additionalInfo[level - 1];
    }

    public int GetCostToReplaceEffect(int level = 1) {
        return _costToReplaceEffect[level - 1];
    }
}

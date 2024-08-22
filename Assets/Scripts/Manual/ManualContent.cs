using System.Collections.Generic;
using UnityEngine;

public class ManualContent : MonoBehaviour
{
    [SerializeField] private string entityName;
    private Sprite _sprite;
    [SerializeField] private List<string> shortDescriptionLevel1 = new();
    [SerializeField] private List<string> shortDescriptionLevel2 = new();
    [SerializeField] private List<string> shortDescriptionLevel3 = new();
    [SerializeField] private List<string> additionalInfo = new();
    [SerializeField] private List<int> costToReplaceEffect = new();
    [SerializeField] private EResourceTypes replaceEffectResourceType;
    [SerializeField] private EResourceCharacters character;

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
        get { return replaceEffectResourceType; }
        private set {}
    }

    public EResourceCharacters Character {
        get { return character; }
        private set {}
    }

    public string GetEntityName(bool lowercase = false) {
        return lowercase && entityName != null ? entityName.ToLower() : entityName;
    }
    
    public string GetEntityNameWithLevel(int level, bool lowercase = false) {
        string suffix = " (ур. " + level + ")";
        string myEntityName = GetEntityName(lowercase);
        return myEntityName + suffix;
    }

    public List<string> GetShortDescription(int level = 1) {
        switch(level) {
            case 1: {
                return shortDescriptionLevel1;
            }
            case 2: {
                return shortDescriptionLevel2;
            }
            default: {
                return shortDescriptionLevel3;
            }
        }
    }

    public string GetAdditionalInfo(int level = 1) {
        return additionalInfo[level - 1];
    }

    public int GetCostToReplaceEffect(int level = 1) {
        return costToReplaceEffect[level - 1];
    }
}
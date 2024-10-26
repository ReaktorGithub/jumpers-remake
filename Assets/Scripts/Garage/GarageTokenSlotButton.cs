using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GarageTokenSlotButton : MonoBehaviour
{
    [SerializeField] private GameObject _instances, _bgObject, _nodeObject, _nodeIconObject, _abilityIconObject, _slotObject;
    [SerializeField] private TextMeshProUGUI _abilityText;
    private Sprite _bgFilledSprite, _bgLockedSprite, _nodeSprite, _nodeHoverSprite, _iconLockSprite, _iconPlusSprite, _iconMinusSprite;
    private bool _isLocked, _isEmpty, _isFilled;

    private void Awake() {
        _bgFilledSprite = _instances.transform.Find("bg-filled").GetComponent<Image>().sprite;
        _bgLockedSprite = _instances.transform.Find("bg-locked").GetComponent<Image>().sprite;
        _nodeSprite = _instances.transform.Find("node").GetComponent<Image>().sprite;
        _nodeHoverSprite = _instances.transform.Find("node-hover").GetComponent<Image>().sprite;
        _iconLockSprite = _instances.transform.Find("icon-lock").GetComponent<Image>().sprite;
        _iconPlusSprite = _instances.transform.Find("icon-plus").GetComponent<Image>().sprite;
        _iconMinusSprite = _instances.transform.Find("icon-minus").GetComponent<Image>().sprite;
    }

    private void Start() {
        _instances.SetActive(false);
        OnNodeHoverOut();
    }

    public void OnNodeHoverIn() {
        _nodeObject.GetComponent<Image>().sprite = _nodeHoverSprite;
    }

    public void OnNodeHoverOut() {
        _nodeObject.GetComponent<Image>().sprite = _nodeSprite;
    }

    public void UpdateContent(PlayerTokenSlot data) {
        if (data.Disabled) {
            _slotObject.SetActive(false);
            return;
        } else {
            _slotObject.SetActive(true);
        }
        
        _isLocked = data.Locked;
        _isEmpty = !_isLocked && data.Ability == EAbilities.None;
        _isFilled = !_isLocked && data.Ability != EAbilities.None;

        _bgObject.GetComponent<Image>().sprite = _isLocked ? _bgLockedSprite : _bgFilledSprite;

        Sprite nodeIconSprite;
        
        if (_isLocked) {
            nodeIconSprite = _iconLockSprite;
        } else {
            nodeIconSprite = _isFilled ? _iconMinusSprite : _iconPlusSprite;
        }

        _nodeIconObject.GetComponent<Image>().sprite = nodeIconSprite;

        Sprite abilitySprite = null;
        
        if (_isFilled) {
            abilitySprite = GarageControl.Instance.GetAbilitySprite(data.Ability);
        }

        if (abilitySprite == null) {
            _abilityIconObject.SetActive(false);
        } else {
            _abilityIconObject.SetActive(true);
            _abilityIconObject.GetComponent<Image>().sprite = abilitySprite;
        }

        string text;

        if (_isFilled) {
            ManualContent manual = Manual.Instance.GetAbilityManual(data.Ability);
            text = manual.GetEntityName();
            _abilityText.color = new Color(_abilityText.color.r, _abilityText.color.g, _abilityText.color.b, 255);
        } else {
            text = _isEmpty ? "добавьте навык" : "купите слот";
            _abilityText.color = new Color(_abilityText.color.r, _abilityText.color.g, _abilityText.color.b, 0.2f);
        }

        _abilityText.text = text;
    }

    public void OnNodeClick() {
        // todo
    }

    public void OnBodyClick() {
        // todo
    }

}

using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GarageTokenSlotButton : MonoBehaviour
{
    [SerializeField] private GameObject _instances, _bgObject, _nodeObject, _nodeIconObject, _abilityIconObject;
    [SerializeField] private TextMeshProUGUI _abilityText;
    private Sprite _bgFilledSprite, _bgLockedSprite, _nodeSprite, _nodeHoverSprite, _iconLockSprite, _iconPlusSprite, _iconMinusSprite;

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

    }
}

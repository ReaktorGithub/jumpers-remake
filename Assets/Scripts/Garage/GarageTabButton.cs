using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GarageTabButton : MonoBehaviour
{
    private GameObject _instances;
    private Sprite _activeSprite, _inactiveSprite, _activeBg, _inactiveBg;
    private Image _icon, _bg;
    private TextMeshProUGUI _text;
    private CursorManager _cursorManager;
    [SerializeField] private EGarageTabs _tab;

    private void Awake() {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _bg = GetComponent<Image>();
        _instances = transform.Find("OwnInstances").gameObject;
        _activeSprite = _instances.transform.Find("ActiveIcon").GetComponent<Image>().sprite;
        _inactiveSprite = _instances.transform.Find("InactiveIcon").GetComponent<Image>().sprite;
        _activeBg = _instances.transform.Find("ActiveBg").GetComponent<Image>().sprite;
        _inactiveBg = _instances.transform.Find("InactiveBg").GetComponent<Image>().sprite;
        _text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _cursorManager = GetComponent<CursorManager>();
    }

    private void Start() {
        _instances.SetActive(false);
        SetSelected(false);
    }

    public EGarageTabs Tab {
        get { return _tab; }
        private set {}
    }

    public void SetSelected(bool value) {
        _bg.sprite = value ? _activeBg : _inactiveBg;
        _icon.sprite = value ? _activeSprite : _inactiveSprite;
        _text.color = value ? new Color32(255,255,255,255) : new Color32(145,145,130,255);
        _cursorManager.Disabled = value;
    }

    public void OnClick() {
        GarageControl.Instance.OnTabClick(_tab);
    }
}

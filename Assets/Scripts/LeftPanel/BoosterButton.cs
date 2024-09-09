using UnityEngine;
using UnityEngine.UI;

public class BoosterButton : MonoBehaviour
{
    private SpriteRenderer _booster;
    private GameObject _selected, _armorOverlay;
    private Image _armorOverlayImage;
    private Button _button;
    private bool _disabled = false;
    private EBoosters _boosterType = EBoosters.None;
    private bool _isArmorMode = false;
    private int _armor = 0;
    private int _armorIron = 0;
    private CursorManager _cursorManager;
    

    private void Awake() {
        _button = GetComponent<Button>();
        _booster = transform.Find("booster").GetComponent<SpriteRenderer>();
        _selected = transform.Find("booster-selected").gameObject;
        _armorOverlay = transform.Find("armor").gameObject;
        _armorOverlayImage = _armorOverlay.GetComponent<Image>();
        _cursorManager = GetComponent<CursorManager>();
    }

    public EBoosters BoosterType {
        get { return _boosterType; }
        set {
            OnChangeBooster(value);
        }
    }

    public bool IsArmorMode {
        get { return _isArmorMode; }
        set { _isArmorMode = value; }
    }

    public int Armor {
        get { return _armor; }
        set {
            UpdateShield(value);
        }
    }

    public int ArmorIron {
        get { return _armorIron; }
        set {
            UpdateShieldIron(value);
        }
    }

    private void UpdateShield(int armor) {
        _armorOverlayImage.color = new Color32(41, 163, 246, 80);
        float newScale = 1f / 4f * armor;
        _armorOverlay.transform.localScale = new Vector3(1f, newScale, 1f);
        _armorOverlay.SetActive(armor > 0);
    }

    private void UpdateShieldIron(int armor) {
        _armorOverlayImage.color = new Color32(177, 0, 255, 80);
        float newScale = 1f / 12f * armor;
        _armorOverlay.transform.localScale = new Vector3(1f, newScale, 1f);
        _armorOverlay.SetActive(armor > 0);
    }

    public void OnSelect() {
        if (IsShield()) {
            PlayerControl player = PlayersControl.Instance.GetMe();
            if (player != null) {
                // мы должны запомнить, на какой кнопке игрок активировал щит
                player.SelectedShieldButton = this;
                BoostersControl.Instance.ExecuteShield(player, _boosterType == EBoosters.ShieldIron);
            }
            return;
        }
        SetSelected(true);
        BoostersControl.Instance.DisableAllButtons(this);
        EffectsControl.Instance.DisableAllButtons(true);
        BoostersControl.Instance.ActivateBooster(_boosterType);
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public bool IsEmpty() {
        return _boosterType == EBoosters.None;
    }

    public bool IsShield() {
        return _boosterType == EBoosters.Shield || _boosterType == EBoosters.ShieldIron;
    }

    public void OnChangeBooster(EBoosters booster) {
        _boosterType = booster;

        switch(booster) {
            case EBoosters.Magnet: {
                _booster.sprite = BoostersControl.Instance.MagnetSprite;
                break;
            }
            case EBoosters.MagnetSuper: {
                _booster.sprite = BoostersControl.Instance.MagnetSuperSprite;
                break;
            }
            case EBoosters.Lasso: {
                _booster.sprite = BoostersControl.Instance.LassoSprite;
                break;
            }
            case EBoosters.Shield: {
                _booster.sprite = BoostersControl.Instance.ShieldSprite;
                break;
            }
            case EBoosters.ShieldIron: {
                _booster.sprite = BoostersControl.Instance.ShieldIronSprite;
                break;
            }
            default: {
                _booster.sprite = null;
                break;
            }
        }

        bool isEnabled = !IsEmpty() && !_disabled;
        _button.interactable = isEnabled;
    }

    // если кнопка энейблится, но при этом она пустая, или в режиме брони, то ничего не делать

    public void SetDisabled(bool value) {
        bool dontEnable = IsEmpty() || _isArmorMode;
        if (!value && dontEnable) {
            return;
        }
        _disabled = value;
        _button.interactable = !value;
        Color color = new(1f, 1f, 1f, value ? 0.3f : 1f);
        _booster.color = color;
        _cursorManager.Disabled = value;
    }
}

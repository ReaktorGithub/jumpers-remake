using UnityEngine;
using UnityEngine.UI;

public class BoosterButton : MonoBehaviour
{
    private SpriteRenderer _booster;
    private GameObject _selected;
    private Button _button;
    private bool _disabled = false;
    private EBoosters _boosterType = EBoosters.None;
    private CursorManager _cursorManager;

    private void Awake() {
        UpdateLinks();
    }

    public EBoosters BoosterType {
        get { return _boosterType; }
        set {
            OnChangeBooster(value);
        }
    }

    public void UpdateLinks() {
        _button = GetComponent<Button>();
        _selected = transform.Find("booster-selected").gameObject;
        _cursorManager = GetComponent<CursorManager>();
        _booster = transform.Find("booster").GetComponent<SpriteRenderer>();
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

    public bool IsAttackOnly() {
        return _boosterType == EBoosters.Vampire || _boosterType == EBoosters.Slimer;
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
            case EBoosters.Vampire: {
                _booster.sprite = BoostersControl.Instance.VampireSprite;
                break;
            }
            case EBoosters.Boombaster: {
                _booster.sprite = BoostersControl.Instance.BoombasterSprite;
                break;
            }
            default: {
                _booster.sprite = null;
                break;
            }
        }

        bool isEnabled = !IsEmpty() && !_disabled;
        _button.interactable = isEnabled;
        _cursorManager.Disabled = !isEnabled;
    }

    // если кнопка энейблится, но при этом она пустая, или в режиме брони, то ничего не делать

    public void SetDisabled(bool value) {
        bool isArmorMode = false;
        transform.TryGetComponent(out BoosterButtonActivate activate);
        if (activate != null) {
            isArmorMode = activate.IsArmorMode;
        }

        bool dontEnable = IsEmpty() || isArmorMode;
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

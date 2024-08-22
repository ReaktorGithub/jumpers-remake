using UnityEngine;
using UnityEngine.UI;

public class BoosterButton : MonoBehaviour
{
    private SpriteRenderer _booster;
    private GameObject _selected;
    private Button _button;
    private bool _disabled = false;
    private EBoosters _boosterType = EBoosters.None;

    private void Awake() {
        _button = GetComponent<Button>();
        _booster = transform.Find("booster").GetComponent<SpriteRenderer>();
        _selected = transform.Find("booster-selected").gameObject;
    }

    public EBoosters BoosterType {
        get { return _boosterType; }
        set {
            OnChangeBooster(value);
        }
    }

    public void OnSelect() {
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
            default: {
                _booster.sprite = null;
                break;
            }
        }

        bool isEnabled = !IsEmpty() && !_disabled;
        _button.interactable = isEnabled;
    }

    // если кнопка энейблится, но при этом она пустая, то ничего не делать

    public void SetDisabled(bool value) {
        if (!value && IsEmpty()) {
            return;
        }
        _disabled = value;
        _button.interactable = !value;
        Color color = new(1f, 1f, 1f, value ? 0.2f : 1f);
        _booster.color = color;
    }
}

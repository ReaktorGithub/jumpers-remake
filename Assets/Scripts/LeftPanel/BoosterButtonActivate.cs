using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoosterButton))]

public class BoosterButtonActivate : MonoBehaviour
{
    private GameObject _armorOverlay;
    private Image _armorOverlayImage;
    private bool _isArmorMode = false;
    private int _armor = 0;
    private int _armorIron = 0;
    private BoosterButton _boosterButton;

    private void Awake() {
        _armorOverlay = transform.Find("armor").gameObject;
        _armorOverlayImage = _armorOverlay.GetComponent<Image>();
        _boosterButton = transform.GetComponent<BoosterButton>();
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
        if (_boosterButton.IsShield()) {
            PlayerControl player = PlayersControl.Instance.GetMe();
            if (player != null) {
                // мы должны запомнить, на какой кнопке игрок активировал щит
                player.Boosters.SelectedShieldButton = _boosterButton;
                BoostersControl.Instance.ExecuteShield(player, _boosterButton.BoosterType == EBoosters.ShieldIron);
            }
            return;
        }

        if (_boosterButton.IsAttackOnly()) {
            BoostersControl.Instance.ShowAttackOnlyWarning();
            return;
        }

        _boosterButton.SetSelected(true);
        BoostersControl.Instance.DisableAllButtons(_boosterButton);
        EffectsControl.Instance.DisableAllButtons(true);
        BoostersControl.Instance.ActivateBooster(_boosterButton.BoosterType);
    }
}

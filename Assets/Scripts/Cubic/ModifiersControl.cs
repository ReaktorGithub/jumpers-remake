using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModifiersControl : MonoBehaviour
{
    [SerializeField] private GameObject _modifierMagnet, _modifierLightning, _modifierStuck;
    private TextMeshProUGUI _stuckText;

    private void Awake() {
        HideModifierMagnet();
        ShowModifierLightning(false);
        _stuckText = _modifierStuck.transform.Find("text").GetComponent<TextMeshProUGUI>();
        HideModifierStuck();
    }

    public void ShowModifierMagnet(bool isSuper) {
        Image image = _modifierMagnet.GetComponent<Image>();

        if (isSuper) {
            image.sprite = BoostersControl.Instance.MagnetSuperSprite;
        } else {
            image.sprite = BoostersControl.Instance.MagnetSprite;
        }

        _modifierMagnet.SetActive(true);
    }

    public void HideModifierMagnet() {
        _modifierMagnet.SetActive(false);
    }

    public void ShowModifierLightning(bool value) {
        _modifierLightning.SetActive(value);
    }

    public void HideModifierStuck() {
        _modifierStuck.SetActive(false);
    }

    public void ShowModifierStuck(int count) {
        if (count == 0) {
            HideModifierStuck();
            return;
        }

        _stuckText.text = "- " + count;
        _modifierStuck.SetActive(true);
    }
}

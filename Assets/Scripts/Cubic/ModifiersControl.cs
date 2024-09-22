using UnityEngine;
using UnityEngine.UI;

public class ModifiersControl : MonoBehaviour
{
    [SerializeField] private GameObject _modifierMagnet, _modifierLightning;

    private void Awake() {
        HideModifierMagnet();
        ShowModifierLightning(false);
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
}

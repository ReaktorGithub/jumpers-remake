using UnityEngine;
using UnityEngine.UI;

public class ModifiersControl : MonoBehaviour
{
    [SerializeField] private GameObject _modifierMagnet;

    private void Awake() {
        HideModifierMagnet();
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
}

using UnityEngine;
using UnityEngine.UI;

public class ModifiersControl : MonoBehaviour
{
    [SerializeField] private GameObject modifierMagnet;

    private void Awake() {
        HideModifierMagnet();
    }

    public void ShowModifierMagnet(bool isSuper) {
        Image image = modifierMagnet.GetComponent<Image>();

        if (isSuper) {
            image.sprite = BoostersControl.Instance.MagnetSuperSprite;
        } else {
            image.sprite = BoostersControl.Instance.MagnetSprite;
        }

        modifierMagnet.SetActive(true);
    }

    public void HideModifierMagnet() {
        modifierMagnet.SetActive(false);
    }
}

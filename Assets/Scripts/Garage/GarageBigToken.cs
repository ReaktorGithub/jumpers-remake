using UnityEngine;
using UnityEngine.UI;

public class GarageBigToken : MonoBehaviour
{
    [SerializeField] private GameObject _symbolObject;

    public void SetToken(GarageShopToken token, Sprite symbolSprite) {
        transform.GetComponent<Image>().sprite = token.TokenSprite;
        _symbolObject.GetComponent<Image>().sprite = symbolSprite;
    }

    public void SetSqueezeAnimation(bool value) {
        Animator animator = transform.GetComponent<Animator>();
        Debug.Log(animator);
        animator.SetBool("squeeze", value);
    }
}

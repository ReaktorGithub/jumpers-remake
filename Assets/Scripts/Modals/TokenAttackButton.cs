using System;
using UnityEngine;
using UnityEngine.UI;

public class TokenAttackButton : MonoBehaviour
{
    private GameObject _selected, _hover;
    private Image _tokenImage;
    private PlayerControl _player;

    private void Awake() {
        _tokenImage = transform.Find("TokenImage").gameObject.GetComponent<Image>();
        _selected = transform.Find("SelectedImage").gameObject;
        _hover = transform.Find("HoverImage").gameObject;
        _selected.SetActive(false);
        _hover.SetActive(false);
    }

    public void SetTokenImage(Sprite sprite) {
        _tokenImage.sprite = sprite;
    }

    public void OnHoverIn() {
        _hover.SetActive(true);
    }

    public void OnHoverOut() {
        _hover.SetActive(false);
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public void BindPlayer(PlayerControl player) {
        _player = player;
    }

    public PlayerControl Player {
        get { return _player; }
        private set {}
    }
}

using UnityEngine;
using UnityEngine.UI;

public class TokenAttackButton : MonoBehaviour
{
    private GameObject _selected, _hover, _shield;
    private Image _tokenImage, _shieldImage;
    private PlayerControl _player;
    private Button _button;
    private bool _disabled = false;

    private void Awake() {
        _tokenImage = transform.Find("TokenImage").gameObject.GetComponent<Image>();
        _shield = transform.Find("ShieldImage").gameObject;
        _shieldImage = _shield.GetComponent<Image>();
        _selected = transform.Find("SelectedImage").gameObject;
        _hover = transform.Find("HoverImage").gameObject;
        _selected.SetActive(false);
        _hover.SetActive(false);
        _button = GetComponent<Button>();
    }

    public void SetTokenImage(Sprite sprite) {
        _tokenImage.sprite = sprite;
    }

    public void DisableShieldImage(bool value) {
        _shield.SetActive(!value);
    }

    public void SetShieldImage(Sprite sprite) {
        _shieldImage.sprite = sprite;
    }

    public void SetDisabled(bool value) {
        _disabled = value;
        if (value) {
            OnHoverOut();
            SetSelected(false);
        }
    }

    public void OnHoverIn() {
        if (!_disabled) {
            _hover.SetActive(true);
        }
    }

    public void OnHoverOut() {
        _hover.SetActive(false);
    }

    public void SetSelected(bool value) {
        if (!_disabled) {
           _selected.SetActive(value); 
        }
    }

    public void BindPlayer(PlayerControl player) {
        _player = player;
    }

    public PlayerControl Player {
        get { return _player; }
        private set {}
    }
}

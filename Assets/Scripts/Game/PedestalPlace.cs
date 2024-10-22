using TMPro;
using UnityEngine;

public class PedestalPlace : MonoBehaviour
{
    [SerializeField] private TextMeshPro _playerName;
    private SpriteRenderer _tokenImage, _tokenSymbol;

    private void Awake() {
        _playerName = transform.Find("PlayerName").GetComponent<TextMeshPro>();
        _tokenImage = GetComponent<SpriteRenderer>();
        _tokenSymbol = transform.Find("TokenSymbol").GetComponent<SpriteRenderer>();
    }

    public void SetPlayer(PlayerControl player) {
        TokenControl token = player.GetTokenControl();

        _playerName.text = player.PlayerName;
        _tokenImage.sprite = token.TokenImage.GetComponent<SpriteRenderer>().sprite;
        _tokenSymbol.sprite = token.GetTokenSymbolSprite();
    }

    public void CleanPedestalVisual() {
        _playerName.text = "";
        _tokenImage.sprite = null;
        _tokenSymbol.sprite = null;
    }
}

using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private GameObject _current, _finish, _tokenImage, _hover, _chamomileActive, _chamomileInactive;
    [SerializeField] private TextMeshProUGUI _placeText, _playerNameText, _powerText, _rubiesText, _coinsText;
    private SpriteRenderer _tokenImageRenderer;

    private void Awake() {
        UpdateLinks();
        SetChamomileActive(false); // todo удалить это после того, как будет готова ромашка
        OnMouseExit();
    }

    public void UpdateLinks() {
        _tokenImageRenderer = _tokenImage.GetComponent<SpriteRenderer>();
    }

    public void OnMouseEnter() {
        _hover.SetActive(true);
    }

    public void OnMouseExit() {
        _hover.SetActive(false);
    }

    private void SetChamomileActive(bool value) {
        _chamomileActive.SetActive(value);
        _chamomileInactive.SetActive(!value);
    }

    public void UpdatePlayerInfoDisplay(PlayerControl player) {
        _current.SetActive(player.IsCurrent());
        _finish.SetActive(player.IsFinished);
        _tokenImageRenderer.sprite = player.GetTokenControl().GetTokenSymbolSprite();
        _tokenImageRenderer.transform.localScale = new Vector3(10f,10f,10f);
        _placeText.text = GetPlaceText(player.MoveOrder);
        _placeText.gameObject.SetActive(!player.IsFinished);
        _playerNameText.text = player.PlayerName;
        _powerText.text = player.Power.ToString();
        _rubiesText.text = player.Rubies.ToString();
        _coinsText.text = player.Coins.ToString();
    }

    private string GetPlaceText(int place) {
        return place switch
        {
            1 => "I",
            2 => "II",
            3 => "III",
            _ => "IV",
        };
    }
}

using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    private TextMeshProUGUI _playerNameText;
    private SpriteRenderer _tokenImage;
    private TextMeshProUGUI _coinText;
    private TextMeshProUGUI _rubyText;
    private GameObject _powerCells, _powerOn, _powerOff, _place, _finish, _hover, _active;
    private SpriteRenderer[] _powerOffImages = new SpriteRenderer[20];
    private SpriteRenderer[] _symbolImages = new SpriteRenderer[4];
    [SerializeField] private int _infoOrder; // todo sort

    private void Awake() {
        _hover = transform.Find("hover_player").gameObject;
        _active = transform.Find("current_player").gameObject;
        _playerNameText = transform.Find("player_name").gameObject.GetComponent<TextMeshProUGUI>();
        _finish = transform.Find("icon_finish").gameObject;
        _place = transform.Find("place").gameObject;
        _tokenImage = transform.Find("token_image").gameObject.GetComponent<SpriteRenderer>();
        _coinText = transform.Find("coin_text").gameObject.GetComponent<TextMeshProUGUI>();
        _rubyText = transform.Find("ruby_text").gameObject.GetComponent<TextMeshProUGUI>();
        _powerCells = transform.Find("PowerCells").gameObject;
        _powerOn = _powerCells.transform.Find("power_cell_on").gameObject;
        _powerOff = _powerCells.transform.Find("power_cell_off").gameObject;
        for (int i = 0; i < 20; i++) {
            string name = "power_cell_" + (i + 1);
            _powerOffImages[i] = _powerCells.transform.Find(name).GetComponent<SpriteRenderer>();
        }
        for (int i = 0; i < 4; i++) {
            string name = "symbol_" + (i + 1);
            _symbolImages[i] = GameObject.Find(name).GetComponent<SpriteRenderer>();
        }
    }

    private void Start() {
        _hover.SetActive(false);
        _powerOn.SetActive(false);
        _powerOff.SetActive(false);
        _active.SetActive(false);
        _finish.SetActive(false);
        foreach (SpriteRenderer sprite in _symbolImages) {
            sprite.gameObject.SetActive(false);
        }
    }

    public void UpdatePlayerInfoDisplay(PlayerControl player) {
        _coinText.text = player.Coins.ToString();
        _rubyText.text = player.Rubies.ToString();
        _finish.SetActive(player.IsFinished);
        _place.SetActive(!player.IsFinished);
        _playerNameText.text = player.PlayerName;
        SpriteRenderer offSprite = _powerOff.GetComponent<SpriteRenderer>();
        SpriteRenderer onSprite = _powerOn.GetComponent<SpriteRenderer>();
        int count = 1;
        foreach (SpriteRenderer sprite in _powerOffImages) {
            if (player.Power >= count) {
                sprite.sprite = onSprite.sprite;
            } else {
                sprite.sprite = offSprite.sprite;
            }
            count++;
        }
        _active.SetActive(player.MoveOrder == MoveControl.Instance.CurrentPlayerIndex);
        _tokenImage.sprite = _symbolImages[player.MoveOrder - 1].sprite;
    }
}

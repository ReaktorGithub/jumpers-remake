using TMPro;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    private bool[] _pedestal = new bool[4];
    private SpriteRenderer _place1;
    private SpriteRenderer _place2;
    private SpriteRenderer _place3;
    private SpriteRenderer _place4;
    private TextMeshPro _name1;
    private TextMeshPro _name2;
    private TextMeshPro _name3;
    private TextMeshPro _name4;

    private void Awake() {
        _place1 = transform.Find("place1").gameObject.GetComponent<SpriteRenderer>();
        _place2 = transform.Find("place2").gameObject.GetComponent<SpriteRenderer>();
        _place3 = transform.Find("place3").gameObject.GetComponent<SpriteRenderer>();
        _place4 = transform.Find("place4").gameObject.GetComponent<SpriteRenderer>();
        _name1 = transform.Find("name1").gameObject.GetComponent<TextMeshPro>();
        _name2 = transform.Find("name2").gameObject.GetComponent<TextMeshPro>();
        _name3 = transform.Find("name3").gameObject.GetComponent<TextMeshPro>();
        _name4 = transform.Find("name4").gameObject.GetComponent<TextMeshPro>();
        _name1.text = "";
        _name2.text = "";
        _name3.text = "";
        _name4.text = "";
        CleanPedestal();
    }

    public int SetPlayerToMaxPlace(PlayerControl player) {
        for (int i = 0; i < _pedestal.Length; i++) {
            if (_pedestal[i] == false) {
                _pedestal[i] = true;
                player.PlaceAfterFinish = i + 1;
                return i + 1;
            }
        }
        return 0;
    }

    public int SetPlayerToMinPlace(PlayerControl player) {
        for (int i = _pedestal.Length - 1; i >= 0; i--) {
            if (_pedestal[i] == false) {
                _pedestal[i] = true;
                player.PlaceAfterFinish = i + 1;
                return i + 1;
            }
        }
        return 0;
    }

    public void SetTokenToPedestal(PlayerControl player, int place) {
        GameObject token = GameObject.Find(player.TokenName);
        Sprite tokenSprite = player.TokenImage;
        switch (place) {
            case 1:
            _place1.sprite = tokenSprite;
            _name1.text = player.PlayerName;
            break;
            case 2:
            _place2.sprite = tokenSprite;
            _name2.text = player.PlayerName;
            break;
            case 3:
            _place3.sprite = tokenSprite;
            _name3.text = player.PlayerName;
            break;
            case 4:
            _place4.sprite = tokenSprite;
            _name4.text = player.PlayerName;
            break;
        }
        token.SetActive(false); 
    }

    public void CleanPedestal() {
        _pedestal[0] = false;
        _pedestal[1] = false;
        _pedestal[2] = false;
        _pedestal[3] = false;
    }
}

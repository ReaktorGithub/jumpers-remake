using UnityEngine;

public class PrepareLevel : MonoBehaviour
{
    private PlayerControl[] _playerControls = new PlayerControl[4];
    private MoveControl _moveControl;
    private Messages _messages;

    private void Awake() {
        _moveControl = GameObject.Find("GameScripts").GetComponent<MoveControl>();
        PlayerControl _playerControl1 = GameObject.Find("player_1").GetComponent<PlayerControl>();
        PlayerControl _playerControl2 = GameObject.Find("player_2").GetComponent<PlayerControl>();
        PlayerControl _playerControl3 = GameObject.Find("player_3").GetComponent<PlayerControl>();
        PlayerControl _playerControl4 = GameObject.Find("player_4").GetComponent<PlayerControl>();
        _playerControls[0] = _playerControl1;
        _playerControls[1] = _playerControl2;
        _playerControls[2] = _playerControl3;
        _playerControls[3] = _playerControl4;
        _playerControl1.TokenName = "token_1";
        _playerControl2.TokenName = "token_2";
        _playerControl3.TokenName = "token_3";
        _playerControl4.TokenName = "token_4";
        _playerControl1.PlayerName = "Игрок A";
        _playerControl2.PlayerName = "Игрок B";
        _playerControl3.PlayerName = "Игрок C";
        _playerControl4.PlayerName = "Игрок D";
        _messages = GameObject.Find("Messages").GetComponent<Messages>();
    }

    private void Start() {
        // порядок важен
        string message = _messages.Wrap("ГОНКА НАЧАЛАСЬ!", UIColors.Yellow);
        _messages.AddMessage(message);
        PreparePlayerMoveOrder();
        PrepareTokenLayerOrder();
        SetAllTokenPlayerNames();
        _moveControl.UpdatePlayerInfo();
        _moveControl.MoveTokensToStart();
        _moveControl.UpdateSqueezeAnimation();
        _moveControl.PrepareNextPlayer();
    }

    public PlayerControl[] PlayerControls {
        get {
            return _playerControls;
        }
        private set {}
    }

    private void PreparePlayerMoveOrder() {
        // должно запускаться только перед первой трассой
        _playerControls[0].MoveOrder = 1;
        _playerControls[1].MoveOrder = 2;
        _playerControls[2].MoveOrder = 3;
        _playerControls[3].MoveOrder = 4;
    }

    /*
        Порядок слоев в начале заезда:
        order 1 = layer 3
        order 2 = layer 0
        order 3 = layer 1
        order 4 = layer 2
    */

    private void PrepareTokenLayerOrder() {
        foreach(PlayerControl player in _playerControls) {
            TokenControl tokenControl = GameObject.Find(player.TokenName).GetComponent<TokenControl>();
            if (player.MoveOrder == 1) {
                tokenControl.SetOrderInLayer(3);
            } else {
                tokenControl.SetOrderInLayer(player.MoveOrder - 2);
            }
        }
    }

    private void SetAllTokenPlayerNames() {
        foreach(PlayerControl player in _playerControls) {
            TokenControl tokenControl = GameObject.Find(player.TokenName).GetComponent<TokenControl>();
            tokenControl.SetPlayerName(player.PlayerName);
        }
    }
}

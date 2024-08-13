using System.Collections;
using UnityEngine;

public class PlayersControl : MonoBehaviour
{
    public static PlayersControl Instance { get; private set; }
    private PlayerControl[] _players = new PlayerControl[4];
    [SerializeField] private float finishDelay = 0.5f;
    [SerializeField] private float loseDelay = 2f;
    [SerializeField] private float redEffectDelay = 1f;
    private Pedestal _pedestal;
    private LevelData _levelData;

    private void Awake() {
        Instance = this;
        PreparePlayersControl();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
        _levelData = GameObject.Find("LevelScripts").GetComponent<LevelData>();
    }

    private void Start() {
        
    }

    public float FinishDelay {
        get { return finishDelay; }
        private set {}
    }

    public float LoseDelay {
        get { return loseDelay; }
        private set {}
    }

    public float RedEffectDelay {
        get { return redEffectDelay; }
        private set {}
    }

    public PlayerControl[] Players {
        get { return _players; }
        private set {}
    }

    private void PreparePlayersControl() {
        PlayerControl _playerControl1 = GameObject.Find("player_1").GetComponent<PlayerControl>();
        PlayerControl _playerControl2 = GameObject.Find("player_2").GetComponent<PlayerControl>();
        PlayerControl _playerControl3 = GameObject.Find("player_3").GetComponent<PlayerControl>();
        PlayerControl _playerControl4 = GameObject.Find("player_4").GetComponent<PlayerControl>();
        _playerControl1.TokenName = "token_1";
        _playerControl2.TokenName = "token_2";
        _playerControl3.TokenName = "token_3";
        _playerControl4.TokenName = "token_4";
        _playerControl1.PlayerName = "Игрок A";
        _playerControl2.PlayerName = "Игрок B";
        _playerControl3.PlayerName = "Игрок C";
        _playerControl4.PlayerName = "Игрок D";
        _playerControl1.MoveOrder = 1;
        _playerControl2.MoveOrder = 2;
        _playerControl3.MoveOrder = 3;
        _playerControl4.MoveOrder = 4;
        _players[0] = _playerControl1;
        _players[1] = _playerControl2;
        _players[2] = _playerControl3;
        _players[3] = _playerControl4;
    }

    public PlayerControl GetPlayer(int index) {
        foreach(PlayerControl player in _players) {
            if (player.MoveOrder == index) {
                return player;
            }
        }
        return null;
    }

    public string GetTokenNameByMoveOrder(int order) {
        foreach(PlayerControl player in _players) {
            if (player.MoveOrder == order) {
                return player.TokenName;
            }
        }
        return null;
    }

    // в игроке сохранить изображение фишки
    // в фишке сохранить имя игрока

    public void BindTokensToPlayers() {
        foreach(PlayerControl player in _players) {
            GameObject token = GameObject.Find(player.TokenName);
            Sprite sprite = token.transform.Find("TokenImage").GetComponent<SpriteRenderer>().sprite;
            player.TokenImage = sprite;

            TokenControl tokenControl = token.GetComponent<TokenControl>();
            tokenControl.SetPlayerName(player.PlayerName);
        }
    }
    
    /*
        Порядок слоев в начале заезда:
        order 1 = layer 3
        order 2 = layer 0
        order 3 = layer 1
        order 4 = layer 2
    */

    public void PrepareTokenLayerOrder() {
        foreach(PlayerControl player in _players) {
            TokenControl tokenControl = player.GetTokenControl();
            if (player.MoveOrder == 1) {
                tokenControl.SetOrderInLayer(3);
            } else {
                tokenControl.SetOrderInLayer(player.MoveOrder - 2);
            }
        }
    }

    /*
        Порядок слоев фишек по умолчанию:
        order 1 - 3 - текущий
        order 2 - 0
        order 3 - 1
        order 4 - 2
        При смене игрока текущий порядок устанавливается на 3, все остальные -1
    */

    public void UpdateTokenLayerOrder(int currentPlayerIndex) {
        foreach(PlayerControl player in _players) {
            string tokenName = GetTokenNameByMoveOrder(player.MoveOrder);
            GameObject token = GameObject.Find(tokenName);
            if (token == null) {
                continue;
            }
            TokenControl tokenControl = token.GetComponent<TokenControl>();
            if (player.MoveOrder == currentPlayerIndex) {
                tokenControl.SetOrderInLayer(3);
            } else {
                tokenControl.SetOrderInLayer(tokenControl.GetOrderInLayer() - 1);
            }
        }
    }

    public void UpdateSqueezeAnimation(int currentPlayerIndex) {
        foreach(PlayerControl player in _players) {
            GameObject token = GameObject.Find(player.TokenName);
            if (token == null) {
                continue;
            }
            TokenControl tokenControl = token.GetComponent<TokenControl>();
            if (player.MoveOrder == currentPlayerIndex) {
                tokenControl.StartSqueeze();
            } else {
                tokenControl.StopSqueeze();
            }
        }
    }

    public void MoveAllTokensToPedestal(int currentPlayerIndex, float delay) {
        foreach (PlayerControl player in _players) {
            if (!player.IsFinished) {
                player.IsFinished = true;
                int place = _pedestal.SetPlayerToMinPlace(player);
                string name = "PlayerInfo" + player.MoveOrder;
                PlayerInfo info = GameObject.Find(name).GetComponent<PlayerInfo>();
                info.UpdatePlayerInfoDisplay(player, currentPlayerIndex);

                TokenControl tokenControl = player.GetTokenControl();
                IEnumerator coroutine = tokenControl.MoveToPedestalDefer(delay, () => {
                    _pedestal.SetTokenToPedestal(player, place);
                });
                StartCoroutine(coroutine);
            }
        }
    }

    public void UpdatePlayersInfo(int currentPlayerIndex) {
        foreach(PlayerControl player in _players) {
            string name = "PlayerInfo" + player.MoveOrder;
            PlayerInfo info = GameObject.Find(name).GetComponent<PlayerInfo>();
            info.UpdatePlayerInfoDisplay(player, currentPlayerIndex);
        }
    }

    public void UpdateTokensCurrentCell(GameObject newCurrentCell) {
        foreach(PlayerControl player in _players) {
            player.GetTokenControl().CurrentCell = newCurrentCell;
        }
    }

    // ресурсы

    public void GiveEffectsBeforeRace(int currentPlayerIndex) {
        foreach(PlayerControl player in _players) {
            player.EffectsGreen = _levelData.EffectsGreen;
            player.EffectsYellow = _levelData.EffectsYellow;
            player.EffectsBlack = _levelData.EffectsBlack;
            player.EffectsRed = _levelData.EffectsRed;
            player.EffectsStar = _levelData.EffectsStar;
        }
        PlayerControl currentPlayer = GetPlayer(currentPlayerIndex);
        EffectsControl.Instance.UpdateQuantityText(currentPlayer);
        EffectsControl.Instance.UpdateEffectEmptiness(currentPlayer);
    }

    public void GiveResourcesAfterRace() {
        foreach(PlayerControl player in _players) {
            int coinsEarned = _levelData.PrizeCoins[player.PlaceAfterFinish - 1];
            int mallowsEarned = _levelData.PrizeMallows[player.PlaceAfterFinish - 1];
            int rubiesEarned = _levelData.PrizeRubies[player.PlaceAfterFinish - 1];
            player.AddCoins(coinsEarned);
            player.AddMallows(mallowsEarned);
            player.AddRubies(rubiesEarned);
        }
    }
}

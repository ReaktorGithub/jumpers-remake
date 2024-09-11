using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayersControl : MonoBehaviour
{
    public static PlayersControl Instance { get; private set; }
    private PlayerControl[] _players = new PlayerControl[4];
    [SerializeField] private float _finishDelay = 0.5f;
    [SerializeField] private float _loseDelay = 2f;
    [SerializeField] private float _redEffectDelay = 1f;
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
        get { return _finishDelay; }
        private set {}
    }

    public float LoseDelay {
        get { return _loseDelay; }
        private set {}
    }

    public float RedEffectDelay {
        get { return _redEffectDelay; }
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
        _playerControl1.TokenObject = GameObject.Find("token_1");
        _playerControl2.TokenObject = GameObject.Find("token_2");
        _playerControl3.TokenObject = GameObject.Find("token_3");
        _playerControl4.TokenObject = GameObject.Find("token_4");
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

    public PlayerControl GetMe() {
        foreach(PlayerControl player in _players) {
            if (player.IsMe()) {
                return player;
            }
        }
        return null;
    }

    public GameObject GetTokenByMoveOrder(int order) {
        foreach(PlayerControl player in _players) {
            if (player.MoveOrder == order) {
                return player.TokenObject;
            }
        }
        return null;
    }

    // в игроке сохранить изображение фишки и ссылку на фишку
    // в фишке сохранить ссылку на игрока

    public void BindTokensToPlayers() {
        foreach(PlayerControl player in _players) {
            Sprite sprite = Utils.FindChildByName(player.TokenObject, "TokenImage").GetComponent<SpriteRenderer>().sprite;
            player.TokenImage = sprite;

            TokenControl tokenControl = player.TokenObject.GetComponent<TokenControl>();
            tokenControl.SetPlayerName(player.PlayerName);
            tokenControl.PlayerControl = player;
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

    public void UpdateTokenLayerOrder() {
        foreach(PlayerControl player in _players) {
            GameObject token = GetTokenByMoveOrder(player.MoveOrder);
            if (token == null) {
                continue;
            }
            TokenControl tokenControl = token.GetComponent<TokenControl>();
            if (player.MoveOrder == MoveControl.Instance.CurrentPlayerIndex) {
                tokenControl.SetOrderInLayer(3);
            } else {
                tokenControl.SetOrderInLayer(tokenControl.GetOrderInLayer() - 1);
            }
        }
    }

    public void UpdateSqueezeAnimation() {
        foreach(PlayerControl player in _players) {
            if (player.TokenObject == null) {
                continue;
            }
            TokenControl tokenControl = player.TokenObject.GetComponent<TokenControl>();
            if (player.MoveOrder == MoveControl.Instance.CurrentPlayerIndex) {
                tokenControl.StartSqueeze();
            } else {
                tokenControl.StopSqueeze();
            }
        }
    }

    public void MoveAllTokensToPedestal(float delay) {
        foreach (PlayerControl player in _players) {
            if (!player.IsFinished) {
                player.IsFinished = true;
                int place = _pedestal.SetPlayerToMinPlace(player);
                string name = "PlayerInfo" + player.MoveOrder;
                PlayerInfo info = GameObject.Find(name).GetComponent<PlayerInfo>();
                info.UpdatePlayerInfoDisplay(player);

                TokenControl tokenControl = player.GetTokenControl();
                IEnumerator coroutine = tokenControl.MoveToPedestalDefer(delay, () => {
                    _pedestal.SetTokenToPedestal(player, place);
                });
                StartCoroutine(coroutine);
            }
        }
    }

    public void UpdatePlayersInfo() {
        foreach(PlayerControl player in _players) {
            string name = "PlayerInfo" + player.MoveOrder;
            PlayerInfo info = GameObject.Find(name).GetComponent<PlayerInfo>();
            info.UpdatePlayerInfoDisplay(player);
        }
    }

    public void UpdateTokensCurrentCell(GameObject newCurrentCell) {
        foreach(PlayerControl player in _players) {
            player.GetTokenControl().CurrentCell = newCurrentCell;
        }
    }

    // ресурсы

    public void GiveEffectsBeforeRace() {
        foreach(PlayerControl player in _players) {
            player.EffectsGreen = _levelData.EffectsGreen;
            player.EffectsYellow = _levelData.EffectsYellow;
            player.EffectsBlack = _levelData.EffectsBlack;
            player.EffectsRed = _levelData.EffectsRed;
            player.EffectsStar = _levelData.EffectsStar;
        }
        PlayerControl currentPlayer = GetPlayer(MoveControl.Instance.CurrentPlayerIndex);
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

    public void SpendPlayersArmor() {
        foreach(PlayerControl player in _players) {
            player.SpendArmor();
        }
    }

    // AI

    // Определеяется по кол-ву рубинов, денег и силы (именно в таком порядке)
    // Если 1 место == 2 месту, то выбрать случайного

    public PlayerControl GetMostSuccessfulPlayer(List<PlayerControl> players) {
        if (players.Count == 1) {
            return players[0];
        }
        
        List<PlayerControl> array = new();

        foreach(PlayerControl player in players) {
            array.Add(player);
        }

        array.Sort((a, b) => {
            if (a.Rubies != b.Rubies) {
                return b.Rubies - a.Rubies;
            } else if (a.Coins != b.Coins) {
                return b.Coins - a.Coins;
            } else {
                return b.Power - a.Power;
            }
        });

        if (IsPlayerSuccessEqual(array[0], array[1])) {
            return GetRandomPlayer(array);
        }

        return array[0];
    }

    // Сравнивает успешность двух игроков

    private bool IsPlayerSuccessEqual(PlayerControl player1, PlayerControl player2) {
        return player1.Rubies == player2.Rubies && player1.Coins == player2.Coins && player1.Power == player2.Power;
    }

    private PlayerControl GetRandomPlayer(List<PlayerControl> players) {
        System.Random random = new();
        int index = random.Next(0, players.Count);
        return players[index];
    }
}

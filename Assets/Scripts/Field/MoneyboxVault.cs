using System.Collections.Generic;
using UnityEngine;

public class MoneyboxVault : MonoBehaviour
{
    private List<MoneyboxStep> _stepScripts = new();
    [SerializeField] private int _currentStep = 0;
    [SerializeField] private bool _isOver = false;
    [SerializeField] private PlayerControl _occupiedPlayer;

    private void Awake() {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.CompareTag("MoneyboxStep")) {
                _stepScripts.Add(child.gameObject.GetComponent<MoneyboxStep>());
            }
        }
    }

    private void Start() {
        UpdateSelection();
    }

    public int CurrentStep {
        get { return _currentStep; }
        private set {}
    }

    public bool IsOver {
        get { return _isOver; }
        private set {}
    }

    public PlayerControl OccupiedPlayer {
        get { return _occupiedPlayer; }
        set { _occupiedPlayer = value; }
    }

    public void UpdateSelection() {
        for (int i = 0; i < _stepScripts.Count; i++) {
            MoneyboxStep step = _stepScripts[i];
            step.SetDisabled(i < _currentStep);
            step.ShowSelection(i == _currentStep);
        }
    }

    public void SetNextStep() {
        if (_isOver) {
            return;
        }

        _currentStep++;
        UpdateSelection();

        if (_currentStep > 10) {
            _isOver = true;
        }
    }

    // Power, Coins, Rubies

    public (int, int, int) GetBonus() {
        if (_currentStep < 5) {
            return (0, 80, 0);
        } else if (_currentStep >= 5 && _currentStep < 10) {
            return (1, 50, 0);
        }

        return (0, 0, 1);
    }

    public void PutPlayerToVault(PlayerControl player) {
        _occupiedPlayer = player;
        if (player != null) {
            string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " попадает в " + Utils.Wrap("копилку", UIColors.Green);
            Messages.Instance.AddMessage(message);
        }
    }

    // собрать всех игроков на клетке, исключая текущего
    // свитчить игроков в порядке хода, начиная с текущего
    // назначить первого попавшегося игрока как текущего
    // если таких игроков нет, то _occupiedPlayer = null

    public void ReassignPlayers() {
        List<PlayerControl> pretenders = new();
        List<PlayerControl> allPlayers = _occupiedPlayer.GetCurrentCell().GetCurrentPlayers();

        foreach(PlayerControl player in allPlayers) {
            if (player.MoveOrder != _occupiedPlayer.MoveOrder) {
                pretenders.Add(player);
            }
        }

        PlayerControl newPlayer = PlayersControl.Instance.GetNearestPlayerByMoveOrder(_occupiedPlayer, pretenders);
        PutPlayerToVault(newPlayer);
    }
}

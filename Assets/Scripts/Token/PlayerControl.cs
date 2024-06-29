using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private string playerName;

    [SerializeField] private string tokenName;
    [SerializeField] private int moveOrder;
    private int _placeAfterFinish;
    private bool _isFinished = false;
    private int _movesSkip = 0;
    [SerializeField] private int coins = 0;
    [SerializeField] private int rubies = 0;
    [SerializeField] private int power = 2;

    public int MoveOrder {
        get { return moveOrder; }
        set {
            if (value >= 1 && value <= 4) {
                moveOrder = value;
            }
        }
    }

    public int MovesSkip {
        get { return _movesSkip; }
        private set {}
    }

    public string TokenName {
        get { return tokenName; }
        set { tokenName = value; }
    }

    public int PlaceAfterFinish {
        get { return _placeAfterFinish; }
        set {
            if (value >= 1 && value <= 4) {
                _placeAfterFinish = value;
            }
        }
    }

    public bool IsFinished {
        get { return _isFinished; }
        set { _isFinished = value; }
    }

    public string PlayerName {
        get { return playerName; }
        set {
            if (value.Length < 15) {
                playerName = value;
            } else {
                playerName = value[..14];
            }
        }
    }

    public int Coins {
        get { return coins; }
        private set {}
    }

    public int Rubies {
        get { return rubies; }
        private set {}
    }

    public int Power {
        get { return power; }
        private set {}
    }

    public void SkipMoveIncrease(TokenControl token) {
        _movesSkip++;
        token.UpdateSkips(_movesSkip);
    }

    public void SkipMoveDecrease(TokenControl token) {
        _movesSkip--;
        token.UpdateSkips(_movesSkip);
    }
}

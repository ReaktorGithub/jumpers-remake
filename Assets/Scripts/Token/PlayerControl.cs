using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private string playerName;

    [SerializeField] private string tokenName;
    [SerializeField] private int moveOrder;
    private int _placeAfterFinish;
    private bool _isFinished = false;

    public int MoveOrder {
        get {
            return moveOrder;
        }
        set {
            if (value >= 1 && value <= 4) {
                moveOrder = value;
            }
        }
    }

    public string TokenName {
        get {
            return tokenName;
        }
        set {
            tokenName = value;
        }
    }

    public int PlaceAfterFinish {
        get {
            return _placeAfterFinish;
        }
        set {
            if (value >= 1 && value <= 4) {
                _placeAfterFinish = value;
            }
        }
    }

    public bool IsFinished {
        get {
            return _isFinished;
        }
        set {
            _isFinished = value;
        }
    }

    public string PlayerName {
        get {
            return playerName;
        }
        set {
            if (value.Length < 15) {
                playerName = value;
            } else {
                playerName = value[..14];
            }
        }
    }
}

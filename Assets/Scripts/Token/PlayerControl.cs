using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private string playerName;

    [SerializeField] private string tokenName;
    [SerializeField] private int moveOrder;

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

    public string PlayerName {
        get {
            return playerName;
        }
        set {
            if (playerName.Length < 17) {
                playerName = value;
            } else {
                playerName = value[..16];
            }
        }
    }
}

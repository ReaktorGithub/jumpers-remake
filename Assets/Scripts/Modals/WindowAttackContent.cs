using System.Collections.Generic;
using UnityEngine;

public class WindowAttackContent : MonoBehaviour
{
    private GameObject _optionalSectionTokens;

    private void Awake() {
        _optionalSectionTokens = transform.Find("OptionalSectionTokens").gameObject;
    }

    public void BuildContent(List<PlayerControl> rivals, PlayerControl currentPlayer) {
        Debug.Log("количество соперников " + rivals.Count);
        Debug.Log("текущий игрок " + currentPlayer.PlayerName);
        if (rivals.Count < 2) {
            _optionalSectionTokens.SetActive(false);
        } else {
            _optionalSectionTokens.SetActive(true);
        }
    }
}

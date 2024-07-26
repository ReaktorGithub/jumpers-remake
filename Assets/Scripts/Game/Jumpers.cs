using UnityEngine;

// Класс предназначен для предварительной настройки игры перед запуском уровня

public class Jumpers : MonoBehaviour
{
    private void Start() {
        CleanInstances();
        PlayersControl.Instance.BindTokensToPlayers();
    }

    private void CleanInstances() {
        GameObject.Find("arrow-body").SetActive(false);
    }
}

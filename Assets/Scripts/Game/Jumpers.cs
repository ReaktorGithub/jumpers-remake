using UnityEngine;

// Класс предназначен для предварительной настройки игры перед запуском уровня

public class Jumpers : MonoBehaviour
{
    private void Start() {
        CleanInstances();
        PlayersControl.Instance.BindTokensToPlayers();
    }

    private void CleanInstances() {
        GameObject Instances = GameObject.Find("Instances");
        Instances.transform.Find("arrow-body").gameObject.SetActive(false);
        Instances.transform.Find("magnet").gameObject.SetActive(false);
        Instances.transform.Find("magnet-super").gameObject.SetActive(false);
        Instances.transform.Find("lasso").gameObject.SetActive(false);
        Instances.transform.Find("shield").gameObject.SetActive(false);
        Instances.transform.Find("shield-iron").gameObject.SetActive(false);
    }
}

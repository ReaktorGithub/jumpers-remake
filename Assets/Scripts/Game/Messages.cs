using TMPro;
using UnityEngine;

public class Messages : MonoBehaviour
{
    public static Messages Instance { get; private set; }
    private GameObject _gameMessagePrefab;

    private void Awake() {
        Instance = this;
        _gameMessagePrefab = GameObject.Find("GameMessageText");
        _gameMessagePrefab.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void AddMessage(string message) {
        GameObject newMessageObject = Instantiate(_gameMessagePrefab, transform);
        Message messageControl = newMessageObject.GetComponent<Message>();
        messageControl.Write(message);
    }
}

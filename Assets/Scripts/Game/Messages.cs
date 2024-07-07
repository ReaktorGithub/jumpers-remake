using TMPro;
using UnityEngine;

public class Messages : MonoBehaviour
{
    private GameObject _gameMessagePrefab;

    private void Awake() {
        _gameMessagePrefab = GameObject.Find("GameMessageText");
        _gameMessagePrefab.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void AddMessage(string message) {
        GameObject newMessageObject = Instantiate(_gameMessagePrefab, transform);
        Message messageControl = newMessageObject.GetComponent<Message>();
        messageControl.Write(message);
    }
}

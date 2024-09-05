using TMPro;
using UnityEngine;

public class Messages : MonoBehaviour
{
    public static Messages Instance { get; private set; }
    private GameObject _gameMessagePrefab;
    private int count = 0;
    [SerializeField] private int maxMessages = 3;

    private void Awake() {
        Instance = this;
        _gameMessagePrefab = GameObject.Find("GameMessageText");
        _gameMessagePrefab.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void AddMessage(string message) {
        // если превышен лимит, удалить последнее сообщение
        if (count == maxMessages) {
            Destroy(transform.GetChild(1).gameObject);
        } else {
            count++;
        }

        GameObject newMessageObject = Instantiate(_gameMessagePrefab, transform);
        Message messageControl = newMessageObject.GetComponent<Message>();
        messageControl.Write(message);
    }
}

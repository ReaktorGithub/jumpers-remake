using TMPro;
using UnityEngine;

public class Messages : MonoBehaviour
{
    public static Messages Instance { get; private set; }
    private GameObject _gameMessagePrefab;
    private int _count = 0;
    [SerializeField] private int _maxMessages = 3;

    private void Awake() {
        Instance = this;
        _gameMessagePrefab = GameObject.Find("GameMessageText");
        _gameMessagePrefab.GetComponent<TextMeshProUGUI>().text = "";
    }

    public void AddMessage(string message) {
        // если превышен лимит, удалить последнее сообщение
        if (_count == _maxMessages) {
            Destroy(transform.GetChild(1).gameObject);
        } else {
            _count++;
        }

        GameObject newMessageObject = Instantiate(_gameMessagePrefab, transform);
        Message messageControl = newMessageObject.GetComponent<Message>();
        messageControl.Write(message);
    }
}

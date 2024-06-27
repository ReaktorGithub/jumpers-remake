using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Messages : MonoBehaviour
{
    private GameObject _gameMessagePrefab;

    private int _nextMessageIndex = 1;

    private void Awake() {
        _gameMessagePrefab = GameObject.Find("GameMessageText");
    }

    public void AddMessage(string message)
    {
        // 1. Создаем дубликат префаба и даем ему имя
        GameObject newMessageObject = Instantiate(_gameMessagePrefab, transform);
        newMessageObject.name = "GameMessageText_" + _nextMessageIndex;

        // 2. Вносим новый текст
        Message messageControl = newMessageObject.GetComponent<Message>();
        messageControl.Write(message);
        TextMeshProUGUI messageText = newMessageObject.GetComponent<TextMeshProUGUI>();

        // 3. Вычисляем высоту дубликата
        float messageHeight = messageText.preferredHeight;
        Debug.Log(messageHeight);

        // 4. Находим все предыдущие сообщения
        List<GameObject> previousMessages = new();
        foreach (Transform child in transform) {
            if (child.gameObject.name.StartsWith("GameMessageText") && child.gameObject != newMessageObject) {
                previousMessages.Add(child.gameObject);
            }
        }

        // 5. Сдвигаем предыдущие сообщения вниз
        foreach (GameObject previousMessage in previousMessages) {
            previousMessage.transform.position += Vector3.down * messageHeight;
        }

        // 6. Помещаем новое сообщение в освободившееся пространство
        newMessageObject.transform.localPosition = Vector3.zero;

        _nextMessageIndex++;
    }
}

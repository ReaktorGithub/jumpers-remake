using System;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    private TextMeshProUGUI _message;

    private void Awake() {
        _message = GetComponent<TextMeshProUGUI>();
    }

    // private void Start() {
    //     string text = Wrap("ИГРА НАЧАЛАСЬ!", UIColors.Yellow);
    //     Write(text);
    // }

    public void Write(string text) {
        string time = DateTime.Now.ToString("HH:mm:ss");
        _message.text = Wrap(time, UIColors.Grey) + "   " + text;
        _message.ForceMeshUpdate();
    }

    private string Wrap(string text, string color) {
        return "<color=" + color + ">" + text + "</color>";
    }
}

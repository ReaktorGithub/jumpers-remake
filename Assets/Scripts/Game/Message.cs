using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    private TextMeshProUGUI _message;
    [SerializeField] private float fadeInTime = 0.4f;

    private void Awake() {
        _message = GetComponent<TextMeshProUGUI>();
        _message.color = new Color(1f, 1f, 1f, 0f);
    }

    private void Start() {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn() {
        float alpha = 0f;
        while (alpha < 1) {
            alpha += Time.deltaTime;
            _message.color = new Color(1f, 1f, 1f, alpha / fadeInTime);
            yield return null;
        }
    }

    public void Write(string text) {
        string time = DateTime.Now.ToString("HH:mm:ss");
        _message.text = "<color=" + UIColors.Grey + ">" + time + "</color>" + "   " + text;
    }
}

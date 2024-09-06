using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    private TextMeshProUGUI _message;
    [SerializeField] private float _fadeInTime = 0.4f;

    private void Awake() {
        _message = GetComponent<TextMeshProUGUI>();
        _message.color = new Color(1f, 1f, 1f, 0f);
    }

    private void Start() {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn() {
        float startTime = Time.time;
        while (Time.time - startTime < _fadeInTime) {
            float progress = (Time.time - startTime) / _fadeInTime;
            float alpha = Mathf.Lerp(0f, 1f, progress);
            _message.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
    }

    public void Write(string text) {
        string time = DateTime.Now.ToString("HH:mm:ss");
        _message.text = "<color=" + UIColors.Grey + ">" + time + "</color>" + "   " + text;
    }
}

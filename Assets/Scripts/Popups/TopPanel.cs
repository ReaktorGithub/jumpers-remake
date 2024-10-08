using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TopPanel : MonoBehaviour
{
    private GameObject _topPanel;
    private IEnumerator _coroutine;
    private float _shift;
    private TextMeshProUGUI _text;
    private GameObject _cancelButton;
    private Action _cancelCallback;
    [SerializeField] private float _fadeInTime = 0.4f;
    [SerializeField] private float _fadeOutTime = 0.1f;
    
    private void Awake() {
        _topPanel = GameObject.Find("TopPanel");
        _shift = _topPanel.GetComponent<RectTransform>().rect.height;
        _text = Utils.FindChildByName(transform.gameObject, "text").GetComponent<TextMeshProUGUI>();
        _cancelButton = Utils.FindChildByName(transform.gameObject, "cancel");
    }

    private void Start() {
        _topPanel.SetActive(false);
    }

    public void SetText(string text) {
        _text.text = text;
    }

    public void OpenWindow() {
        if (_topPanel.activeInHierarchy) {
            return;
        }
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _topPanel.SetActive(true);
        _coroutine = FadeInOut(_shift * -1, _fadeInTime);
        StartCoroutine(_coroutine);
    }

    public void CloseWindow() {
        if (!_topPanel.activeInHierarchy) {
            return;
        }
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _coroutine = FadeInOut(_shift, _fadeOutTime, () => {
            _topPanel.SetActive(false);
        });
        StartCoroutine(_coroutine);
    }

    private IEnumerator FadeInOut(float shift, float fadeTime, Action callback = null) {
        float startY = _topPanel.transform.localPosition.y;
        float endY = _topPanel.transform.localPosition.y + shift;
        float startTime = Time.time;
        float velocity = 0f;
        while (Time.time - startTime < fadeTime) {
            float progress = (Time.time - startTime) / fadeTime;
            float y = Mathf.SmoothDamp(startY, endY, ref velocity, 0.1f, Mathf.Infinity, progress); 
            _topPanel.transform.localPosition = new Vector3(_topPanel.transform.localPosition.x, y, _topPanel.transform.localPosition.z);
            yield return null;
        }

        callback?.Invoke();
    }

    public void SetCancelButtonActive(bool value, Action cb = null) {
        _cancelCallback = cb;
        _cancelButton.SetActive(value);
    }

    public void OnCancel() {
        if (_cancelCallback != null) {
            _cancelCallback();
        }
    }
}

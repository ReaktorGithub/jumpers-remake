using System;
using System.Collections;
using UnityEngine;

public class TopPanel : MonoBehaviour
{
    private GameObject _topPanel;
    private IEnumerator _coroutine;
    private float _shift;
    [SerializeField] private float fadeInTime = 0.4f;
    [SerializeField] private float fadeOutTime = 0.1f;
    
    private void Awake() {
        _topPanel = GameObject.Find("TopPanel");
        _shift = _topPanel.GetComponent<RectTransform>().rect.height;
    }

    private void Start() {
        _topPanel.SetActive(false);
    }

    public void OpenWindow() {
        if (_topPanel.activeInHierarchy) {
            return;
        }
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _topPanel.SetActive(true);
        _coroutine = FadeInOut(_shift * -1, fadeInTime);
        StartCoroutine(_coroutine);
    }

    public void CloseWindow() {
        if (!_topPanel.activeInHierarchy) {
            return;
        }
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _coroutine = FadeInOut(_shift, fadeOutTime, () => {
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
}

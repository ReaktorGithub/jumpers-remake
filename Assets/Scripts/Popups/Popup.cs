using System;
using System.Collections;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private float _shift;
    [SerializeField] float shiftBy = 70;
    [SerializeField] private float fadeTime = 0.7f;
    private IEnumerator _coroutine;

    private void Awake() {
        _shift = GetComponent<RectTransform>().rect.width + shiftBy;
        transform.localPosition = new(transform.localPosition.x - _shift, transform.localPosition.y, transform.localPosition.z);
    }

    private void Start() {
        transform.gameObject.SetActive(false);
    }

    public bool OpenWindow() {
        if (transform.gameObject.activeInHierarchy) {
            return false;
        }
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        transform.gameObject.SetActive(true);
        _coroutine = FadeInOut(_shift);
        StartCoroutine(_coroutine);
        return true;
    }

    public bool CloseWindow(Action callback = null) {
        if (!transform.gameObject.activeInHierarchy) {
            return false;
        }
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _coroutine = FadeInOut(_shift * -1, () => {
            transform.gameObject.SetActive(false);
            callback?.Invoke();
        });
        StartCoroutine(_coroutine);
        return true;
    }

    private IEnumerator FadeInOut(float shift, Action callback = null) {
        float startX = transform.localPosition.x;
        float endX = transform.localPosition.x + shift;
        float startTime = Time.time;
        float velocity = 0f;
        // Time.time - startTime < fadeTime
        while (fadeTime - (Time.time - startTime) > 0.1f) {
            float progress = (Time.time - startTime) / fadeTime;
            float x = Mathf.SmoothDamp(startX, endX, ref velocity, 0.1f, Mathf.Infinity, progress); 
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
            yield return null;
        }

        callback?.Invoke();
    }
}

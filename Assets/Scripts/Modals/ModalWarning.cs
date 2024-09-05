using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ModalWarning : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _windowControl;
    private TextMeshProUGUI _headingText, _bodyText;
    private IEnumerator _coroutine;
    private Action _callback;
    [SerializeField] private float waitAfterClose = 0.4f;

    private void Awake() {
        _modal = GameObject.Find("ModalWarning");
        _windowControl = _modal.transform.Find("WindowWarning").GetComponent<ModalWindow>();
        _headingText = Utils.FindChildByName(_modal, "HeadingText").GetComponent<TextMeshProUGUI>();
        _bodyText = Utils.FindChildByName(_modal, "BodyText").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        _modal.SetActive(false);
    }

    public void OpenWindow() {
        if (!_modal.activeInHierarchy) {
            _modal.SetActive(true);
            _coroutine = _windowControl.FadeIn();
            StartCoroutine(_coroutine);
        }
    }

    public void CloseWindow() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _modal.SetActive(false);
        _windowControl.ResetScale();
        StartCoroutine(CallbackDefer());
    }

    private IEnumerator CallbackDefer() {
        yield return new WaitForSeconds(waitAfterClose);
        _callback?.Invoke();
    }

    // вызов сеттеров обязателен перед открытием окна, даже если они пустые

    public void SetHeadingText(string text) {
        _headingText.text = text;
    }

    public void SetBodyText(string text) {
        _bodyText.text = text;
    }

    public void SetCallback(Action cb = null) {
        _callback = cb;
    }
}

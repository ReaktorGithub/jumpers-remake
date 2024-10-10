using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalWarning : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private TextMeshProUGUI _headingText, _bodyText;
    private Action _callback;
    private Image _backgroundImage;
    [SerializeField] private float _waitAfterClose = 0.4f;

    private void Awake() {
        _modal = GameObject.Find("ModalWarning").GetComponent<Modal>();
        _backgroundImage = _modal.transform.Find("Modal").GetComponent<Image>();
    }

    public void OpenModal(bool withBackground = false) {
        _backgroundImage.enabled = withBackground;
        _modal.OpenModal();
    }

    public void CloseModal() {
        _modal.CloseModal();
        StartCoroutine(CallbackDefer());
    }

    private IEnumerator CallbackDefer() {
        yield return new WaitForSeconds(_waitAfterClose);
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

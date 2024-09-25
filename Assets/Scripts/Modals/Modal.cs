using System.Collections;
using UnityEngine;

public class Modal : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _contentScript;
    private IEnumerator _coroutine;

    private void Awake() {
        _modal = transform.Find("Modal").gameObject;
        _contentScript = _modal.transform.Find("Content").GetComponent<ModalWindow>();
    }

    private void Start() {
        _modal.SetActive(false);
    }

    public void OpenModal() {
        if (!_modal.activeInHierarchy) {
            _modal.SetActive(true);
            _coroutine = _contentScript.FadeIn();
            StartCoroutine(_coroutine);
        }
    }

    public void CloseModal() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _modal.SetActive(false);
        _contentScript.ResetScale();
    }
}

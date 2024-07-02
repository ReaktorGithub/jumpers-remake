using System;
using System.Collections;
using UnityEngine;

public class WindowAttack : MonoBehaviour
{
    private GameObject _attack;
    private float _shift;
    [SerializeField] private float fadeTime = 0.5f;
    private MoveControl _moveControl;

    private void Awake() {
        _attack = GameObject.Find("WindowAttack");
        _shift = _attack.GetComponent<RectTransform>().rect.width + 70;
        _attack.transform.localPosition = new(_attack.transform.localPosition.x - _shift, _attack.transform.localPosition.y, _attack.transform.localPosition.z);
        _moveControl = GameObject.Find("GameScripts").GetComponent<MoveControl>();
    }

    private void Start() {
        _attack.SetActive(false);
    }

    public void OpenWindow() {
        if (!_attack.activeInHierarchy) {
            _attack.SetActive(true);
            StartCoroutine(FadeInOut(_shift));
        }
    }

    public void CloseWindow() {
        StartCoroutine(FadeInOut(_shift * -1, () => {
            _attack.SetActive(false);
            StartCoroutine(_moveControl.EndMoveDefer());
        }));
    }

    private IEnumerator FadeInOut(float shift, Action callback = null) {
        float startX = _attack.transform.localPosition.x;
        float endX = _attack.transform.localPosition.x + shift;
        float startTime = Time.time;
        float velocity = 0f;
        while (Time.time - startTime < fadeTime) {
            float progress = (Time.time - startTime) / fadeTime;
            float x = Mathf.SmoothDamp(startX, endX, ref velocity, 0.1f, Mathf.Infinity, progress); 
            _attack.transform.localPosition = new Vector3(x, _attack.transform.localPosition.y, _attack.transform.localPosition.z);
            yield return null;
        }

        callback?.Invoke();
    }
}

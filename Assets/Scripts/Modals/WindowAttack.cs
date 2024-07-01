using System.Collections;
using UnityEngine;

public class WindowAttack : MonoBehaviour
{
    private GameObject _attack;
    private float _shift;
    [SerializeField] private float fadeTime = 0.5f;

    private void Awake() {
        _attack = GameObject.Find("WindowAttack");
        _shift = _attack.GetComponent<RectTransform>().rect.width + 70;
        _attack.transform.localPosition = new(_attack.transform.localPosition.x - _shift, _attack.transform.localPosition.y, _attack.transform.localPosition.z);
    }

    private void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.W)) {
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn() {
        float startX = _attack.transform.localPosition.x;
        float endX = _attack.transform.localPosition.x + _shift;
        float startTime = Time.time;
        float velocity = 0f;
        while (Time.time - startTime < fadeTime) {
            float progress = (Time.time - startTime) / fadeTime;
            float x = Mathf.SmoothDamp(startX, endX, ref velocity, 0.1f, Mathf.Infinity, progress); 
            _attack.transform.localPosition = new Vector3(x, _attack.transform.localPosition.y, _attack.transform.localPosition.z);
            yield return null;
        }
    }
}

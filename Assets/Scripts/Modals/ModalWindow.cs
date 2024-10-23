using System.Collections;
using UnityEngine;

public class ModalWindow : MonoBehaviour
{
    [SerializeField] private float _fadeInTime = 0.3f;

    private void Awake() {
        ResetScale();
    }

    public IEnumerator FadeIn() {
        ResetScale();
        float startTime = Time.time;
        float velocity = 0f;
        while (Time.time - startTime < _fadeInTime) {
            float progress = (Time.time - startTime) / _fadeInTime;
            float size = Mathf.SmoothDamp(0f, 1f, ref velocity, 0.1f, Mathf.Infinity, progress); 
            transform.localScale = new Vector3(size, size, 1f);
            yield return null;
        }
    }

    public void ResetScale() {
        transform.localScale = new Vector3(0f, 0f, 1f);
    }
}

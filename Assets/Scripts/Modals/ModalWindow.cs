using System.Collections;
using UnityEngine;

public class ModalWindow : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 1f;

    private void Awake() {
        ResetScale();
    }

    public IEnumerator FadeIn() {
        ResetScale();
        float startTime = Time.time;
        float velocity = 0f;
        while (Time.time - startTime < fadeInTime) {
            float progress = (Time.time - startTime) / fadeInTime;
            float size = Mathf.SmoothDamp(0f, 1f, ref velocity, 0.1f, Mathf.Infinity, progress); 
            transform.localScale = new Vector3(size, size, 1f);
            yield return null;
        }
    }

    public void ResetScale() {
        transform.localScale = new Vector3(0f, 0f, 1f);
    }
}

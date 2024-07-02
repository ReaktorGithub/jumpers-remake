using System.Collections;
using UnityEngine;

public class ModalWindow : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 1f;

    private void Awake() {
        transform.localScale = new Vector3(0f, 0f, 1f);
    }

    public IEnumerator FadeIn() {
        transform.localScale = new Vector3(0f, 0f, 1f);
        float startTime = Time.time;
        float velocity = 0f;
        while (Time.time - startTime < fadeInTime) {
            float progress = (Time.time - startTime) / fadeInTime;
            float size = Mathf.SmoothDamp(0f, 1f, ref velocity, 0.1f, Mathf.Infinity, progress); 
            transform.localScale = new Vector3(size, size, 1f);
            yield return null;
        }
    }
}

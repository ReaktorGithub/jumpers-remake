using System.Collections;
using UnityEngine;

public class ModalWindow : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 1f;

    private void Awake() {
        transform.localScale = new Vector3(0f, 0f, 1f);
    }

    private void Start() {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn() {
        float startTime = Time.time;
        while (Time.time - startTime < fadeInTime) {
            float progress = (Time.time - startTime) / fadeInTime;
            float size = Mathf.Lerp(0f, 1f, progress);
            transform.localScale = new Vector3(size, size, 1f);
            yield return null;
        }
    }
}

using UnityEngine;

public class DevOnly : MonoBehaviour
{
    private void Awake() {
        transform.gameObject.SetActive(Application.isEditor);
    }
}

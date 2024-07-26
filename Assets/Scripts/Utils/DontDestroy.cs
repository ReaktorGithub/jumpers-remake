using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private string objectId;

    private void Awake() {
        objectId = name + transform.position.ToString() + transform.eulerAngles.ToString();
    }

    private void Start() {
        DontDestroy[] objects = FindObjectsOfType<DontDestroy>();
        for (int i = 0; i < objects.Length; i++) {
            if (objects[i] != this) {
                if (objects[i].objectId == objectId) {
                    Destroy(gameObject);
                }
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}

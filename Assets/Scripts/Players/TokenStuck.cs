using UnityEngine;

public class TokenStuck : MonoBehaviour
{
    private GameObject _stuck1, _stuck2, _stuck3, _stuck4;

    private void Awake() {
        _stuck1 = transform.Find("stuck1").gameObject;
        _stuck2 = transform.Find("stuck2").gameObject;
        _stuck3 = transform.Find("stuck3").gameObject;
        _stuck4 = transform.Find("stuck4").gameObject;
        UpdateStuckVisual(0);
    }

    public void UpdateStuckVisual(int count) {
        _stuck1.SetActive(count > 0);
        _stuck2.SetActive(count > 1);
        _stuck3.SetActive(count > 2);
        _stuck4.SetActive(count > 3);
    }
}

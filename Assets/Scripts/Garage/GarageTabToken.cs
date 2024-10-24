using UnityEngine;

public class GarageTabToken : MonoBehaviour
{
    [SerializeField] private GameObject _ownedTokenButtonSample, _ownedListObject;

    private void Start() {
        _ownedTokenButtonSample.SetActive(false);
    }

    public void BuildContent() {
        BuildOwnedList();
    }

    private void BuildOwnedList() {
        Transform[] children = _ownedListObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.CompareTag("OwnedTokenButton")) {
                Destroy(child.gameObject);
            }
        }
    }
}

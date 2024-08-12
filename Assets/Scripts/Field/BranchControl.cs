using System.Collections.Generic;
using UnityEngine;

public class BranchControl : MonoBehaviour
{
    private List<GameObject> _branchButtonsList = new();
    [SerializeField] private bool isReverse = false;

    private void Awake() {
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            if (child.name == "BranchButton") {
                _branchButtonsList.Add(child.gameObject);
            }
        }
    }

    private void Start() {
        HideAllBranches();
    }

    public List<GameObject> BranchButtonsList {
        get { return _branchButtonsList; }
        private set {}
    }

    public bool IsReverse {
        get { return isReverse; }
        private set {}
    }

    public void HideAllBranches() {
        foreach (GameObject branch in _branchButtonsList) {
            branch.GetComponent<BranchButton>().StopPulse();
            branch.SetActive(false);
        }
    }

    public void ShowAllBranches() {
        foreach (GameObject branch in _branchButtonsList) {
            branch.SetActive(true);
            branch.GetComponent<BranchButton>().StartPulse();
        }
    }
}

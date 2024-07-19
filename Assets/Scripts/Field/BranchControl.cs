using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchControl : MonoBehaviour
{
    private List<GameObject> _branchButtonsList = new();

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

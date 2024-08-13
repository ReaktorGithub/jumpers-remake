using UnityEngine;

[RequireComponent(typeof(CellControl))]
public class BranchCell : MonoBehaviour
{
    [SerializeField] private GameObject branchObject;

    public GameObject BranchObject {
        get { return branchObject; }
        private set {}
    }

    public bool IsReverse() {
        return branchObject.GetComponent<BranchControl>().IsReverse;
    }
}

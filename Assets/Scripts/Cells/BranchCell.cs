using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellControl))]
public class BranchCell : MonoBehaviour
{
    [SerializeField] private GameObject _branchObject;
    private BranchControl _branchControl;

    private void Awake() {
        _branchControl = _branchObject.GetComponent<BranchControl>();
    }

    public GameObject BranchObject {
        get { return _branchObject; }
        private set {}
    }

    public bool IsReverse() {
        return _branchControl.IsReverse;
    }

    public List<GameObject> GetAllNextCells() {
        return _branchControl.GetAllNextCells();
    }
}

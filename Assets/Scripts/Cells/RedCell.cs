using UnityEngine;

[RequireComponent(typeof(CellControl))]
public class RedCell : MonoBehaviour
{
    [SerializeField] private GameObject _penaltyCell;

    private void Start() {
        (GameObject cell, int _) = CellsControl.Instance.FindNearestCell(transform.gameObject, false, IsStartOrCheckpointPredicate);
        _penaltyCell = cell;
    }

    private bool IsStartOrCheckpointPredicate(CellControl cell) {
        return cell.CellType == ECellTypes.Start || cell.CellType == ECellTypes.Checkpoint;
    }

    public GameObject PenaltyCell {
        get { return _penaltyCell; }
        set { _penaltyCell = value; }
    }
}

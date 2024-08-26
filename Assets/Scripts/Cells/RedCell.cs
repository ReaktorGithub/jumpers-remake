using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellControl))]
public class RedCell : MonoBehaviour
{
    [SerializeField] private GameObject _penaltyCell;

    private void Start() {
        List<ECellTypes> findCells = new() { ECellTypes.Start, ECellTypes.Checkpoint };
        _penaltyCell = CellsControl.Instance.FindNearestCell(transform.gameObject, false, findCells);
    }

    public GameObject PenaltyCell {
        get { return _penaltyCell; }
        set { _penaltyCell = value; }
    }
}

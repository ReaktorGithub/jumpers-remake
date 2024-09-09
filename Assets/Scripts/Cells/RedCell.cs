using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CellControl))]
public class RedCell : MonoBehaviour
{
    [SerializeField] private GameObject _penaltyCell;

    private void Start() {
        List<ECellTypes> findCells = new() { ECellTypes.Start, ECellTypes.Checkpoint };
        (GameObject cell, int _) = CellsControl.Instance.FindNearestCell(transform.gameObject, false, findCells);
        _penaltyCell = cell;
    }

    public GameObject PenaltyCell {
        get { return _penaltyCell; }
        set { _penaltyCell = value; }
    }
}

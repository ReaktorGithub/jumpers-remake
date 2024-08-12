using UnityEngine;

[RequireComponent(typeof(CellControl))]
public class RedCell : MonoBehaviour
{
    [SerializeField] private GameObject _penaltyCell;

    private void Start() {
        _penaltyCell = CellsControl.Instance.FindNearestCheckpoint(transform.gameObject);
    }

    public GameObject PenaltyCell {
        get { return _penaltyCell; }
        set {}
    }
}

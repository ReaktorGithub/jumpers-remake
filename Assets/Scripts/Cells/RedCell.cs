using UnityEngine;

[RequireComponent(typeof(CellControl))]
public class RedCell : MonoBehaviour
{
    [SerializeField] private string penaltyCell;

    public string PenaltyCell {
        get { return penaltyCell; }
        set {}
    }
}

using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(CellControl))]
public class ArrowCell : MonoBehaviour
{
    [SerializeField] private GameObject arrowSpline;
    [SerializeField] private string arrowToCell;

    public string ArrowToCell {
        get { return arrowToCell; }
        private set {}
    }

    public SplineContainer ArrowSpline {
        get { return arrowSpline.GetComponent<SplineContainer>(); }
        private set {}
    }
}

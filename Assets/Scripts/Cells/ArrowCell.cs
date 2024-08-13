using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(CellControl))]
public class ArrowCell : MonoBehaviour
{
    [SerializeField] private GameObject arrowSpline;
    [SerializeField] private GameObject arrowToCell;

    public GameObject ArrowToCell {
        get { return arrowToCell; }
        private set {}
    }

    public SplineContainer ArrowSpline {
        get { return arrowSpline.GetComponent<SplineContainer>(); }
        private set {}
    }
}

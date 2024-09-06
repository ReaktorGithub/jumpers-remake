using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(CellControl))]
public class ArrowCell : MonoBehaviour
{
    [SerializeField] private GameObject _arrowSpline;
    [SerializeField] private GameObject _arrowToCell;

    public GameObject ArrowToCell {
        get { return _arrowToCell; }
        private set {}
    }

    public SplineContainer ArrowSpline {
        get { return _arrowSpline.GetComponent<SplineContainer>(); }
        private set {}
    }
}

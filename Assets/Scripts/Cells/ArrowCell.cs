using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(CellControl))]
public class ArrowCell : MonoBehaviour
{
    [SerializeField] private GameObject _arrowSpline;
    [SerializeField] private GameObject _arrowToCell;
    [SerializeField] private bool _isForward = true;

    public GameObject ArrowToCell {
        get { return _arrowToCell; }
        private set {}
    }

    public bool IsForward {
        get { return _isForward; }
        private set {}
    }

    public SplineContainer ArrowSpline {
        get { return _arrowSpline.GetComponent<SplineContainer>(); }
        private set {}
    }
}

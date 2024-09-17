using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(BranchButton))]

public class BranchButtonHedge : MonoBehaviour
{
    [SerializeField] private int _taxCost = 0; // если больше 0, то появится диалоговое окно с ежом
    [SerializeField] private GameObject _nextArrowSpline;
    private ModalHedgehogArrow _modal;
    private BranchControl _branchControl;

    private void Awake() {
        _modal = GameObject.Find("GameScripts").GetComponent<ModalHedgehogArrow>();
        _branchControl = transform.parent.GetComponent<BranchControl>();
    }

    public int TaxCost {
        get { return _taxCost; }
        private set {}
    }

    public void ExecuteHedgehogChoice(GameObject nextCell) {
        if (_taxCost > 0) {
            _branchControl.SetDisabledAllButtons(true);
            _modal.BuildContent(MoveControl.Instance.CurrentPlayer, _taxCost);
            _modal.OpenWindow();
            return;
        }

        SplineContainer spline = _nextArrowSpline.GetComponent<SplineContainer>();
        MoveControl.Instance.SwitchBranchHedgehog(nextCell, spline);
    }
}

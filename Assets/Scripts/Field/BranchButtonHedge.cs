using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(BranchButton))]

public class BranchButtonHedge : MonoBehaviour
{
    [SerializeField] private int _taxCost = 0; // если больше 0, то появится диалоговое окно с ежом
    [SerializeField] private GameObject _nextArrowSpline;
    private ModalHedgehogArrow _modal;
    private BranchControl _branchControl;
    private BranchButton _branchButton;

    private void Awake() {
        _modal = GameObject.Find("GameScripts").GetComponent<ModalHedgehogArrow>();
        _branchControl = transform.parent.GetComponent<BranchControl>();
        _branchButton = transform.GetComponent<BranchButton>();
    }

    public int TaxCost {
        get { return _taxCost; }
        private set {}
    }

    public void InitiateHedgehogChoice() {
        if (_taxCost > 0) {
            _branchControl.SetDisabledAllButtons(true);
            _modal.BuildContent(MoveControl.Instance.CurrentPlayer, _branchControl, this);
            _modal.OpenWindow();
            return;
        }

        ExecuteHedgehogChoice();
    }

    public void ExecuteHedgehogChoice() {
        _branchButton.ExecuteHedgehogChoice(_nextArrowSpline.GetComponent<SplineContainer>());
    }
}

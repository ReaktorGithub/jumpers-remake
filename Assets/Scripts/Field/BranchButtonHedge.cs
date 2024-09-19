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
    private BranchHedgehog _branchHedgehog;
    private HedgehogsControl _hedgehogsControl;

    private void Awake() {
        _modal = GameObject.Find("GameScripts").GetComponent<ModalHedgehogArrow>();
        _branchControl = transform.parent.GetComponent<BranchControl>();
        _branchButton = transform.GetComponent<BranchButton>();
        _branchHedgehog = _branchControl.transform.GetComponent<BranchHedgehog>();
        _hedgehogsControl = GameObject.Find("LevelScripts").GetComponent<HedgehogsControl>();
    }

    public int TaxCost {
        get { return _taxCost; }
        private set {}
    }

    public BranchControl BranchControl {
        get { return _branchControl; }
        private set {}
    }

    public BranchHedgehog BranchHedgehog {
        get { return _branchHedgehog; }
        private set {}
    }

    public void InitiateHedgehogChoice() {
        if (_taxCost > 0) {
            _branchControl.SetDisabledAllButtons(true);
            _modal.BuildContent(MoveControl.Instance.CurrentPlayer, this);
            _modal.OpenWindow();
            return;
        }

        ExecuteHedgehogChoice();
    }

    public void ExecuteHedgehogChoice() {
        int rest = _hedgehogsControl.MaxFeedCount - _branchHedgehog.FeedCount;
        string name = MoveControl.Instance.CurrentPlayer.PlayerName;
        string restText = "";
        switch(rest) {
            case 2:
            case 3:
            case 4: {
                restText = ". Осталось покормить: <b>" + rest + "</b> раза";
                break;
            }
            case 0: {
                restText = ". Теперь развилка будет перекрыта";
                break;
            }
            default: {
                restText = ". Осталось покормить: <b>" + rest + "</b> раз";
                break;
            }
        }
        string message = Utils.Wrap(name, UIColors.Yellow) + " кормит " + Utils.Wrap("ежа", UIColors.DarkGreen) + restText;
        Messages.Instance.AddMessage(message);

        _branchButton.ExecuteHedgehogChoice(_nextArrowSpline.GetComponent<SplineContainer>());
    }
}

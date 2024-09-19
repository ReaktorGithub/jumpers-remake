using UnityEngine;

public class HedgehogAnswerButton : MonoBehaviour
{
    private GameObject _hover;
    private bool _disabled = false;
    [SerializeField] private bool _isFightAction;
    private ModalHedgehogFinish _modalControl;
    private CursorManager _cursorManager;

    private void Awake() {
        _hover = transform.Find("Hover").gameObject;
        _modalControl = GameObject.Find("GameScripts").GetComponent<ModalHedgehogFinish>();
        _cursorManager = GetComponent<CursorManager>();
    }

    private void Start() {
        _hover.SetActive(false);
    }

    public void OnMouseEnter() {
        if (!_disabled) {
            _hover.SetActive(true);
        }
    }

    public void OnMouseExit() {
        _hover.SetActive(false);
    }

    public void OnClick() {
        if (_isFightAction) {
            if (!_disabled) {
                _modalControl.CloseWindow();
                _modalControl.OnFight();
            }
        } else {
            _modalControl.CloseWindow();
            _modalControl.OnPay();
        }
    }

    public bool Disabled {
        get { return _disabled; }
        set { 
            _disabled = value;
            _hover.SetActive(!value);
            _cursorManager.Disabled = value;
        }
    }
}

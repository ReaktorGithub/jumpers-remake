using UnityEngine;

public class ReplaceAnswerButton : MonoBehaviour
{
    private GameObject _hover;
    private bool _disabled = false;
    [SerializeField] private bool _isReplaceAction;
    private ModalReplaceEffect _modalControl;
    private CursorManager _cursorManager;

    private void Awake() {
        _hover = transform.Find("Hover").gameObject;
        _modalControl = GameObject.Find("GameScripts").GetComponent<ModalReplaceEffect>();
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
        if (_isReplaceAction) {
            if (!_disabled) {
                _modalControl.CloseWindow();
                EffectsControl.Instance.ActivateSelectionMode(true);
            }
        } else {
            MoveControl.Instance.CheckCellEffects();
            _modalControl.CloseWindow();
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

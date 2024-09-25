using UnityEngine;
using UnityEngine.UI;

public class BigAnswerButton : MonoBehaviour
{
    private GameObject _hover;
    private bool _disabled = false;
    private CursorManager _cursorManager;
    private Button _button;

    private void Awake() {
        _hover = transform.Find("Hover").gameObject;
        _cursorManager = GetComponent<CursorManager>();
        _button = GetComponent<Button>();
    }

    private void OnEnable() {
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

    public bool Disabled {
        get { return _disabled; }
        set { 
            _disabled = value;
            _hover.SetActive(!value);
            _cursorManager.Disabled = value;
            _button.interactable = !value;
        }
    }
}

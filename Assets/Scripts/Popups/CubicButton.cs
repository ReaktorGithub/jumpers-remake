using UnityEngine;

public class CubicButton : MonoBehaviour
{
    private GameObject _selected, _hover;
    private PopupMagnet _popupMagnet;
    private PopupVacuum _popupVacuum;
    [SerializeField] private bool isVacuumMode = false;
    
    [SerializeField] private int _score = 1;

    private void Awake() {
        _selected = transform.Find("SelectedImage").gameObject;
        _hover = transform.Find("HoverImage").gameObject;
        _selected.SetActive(false);
        _hover.SetActive(false);
        _popupMagnet = GameObject.Find("GameScripts").GetComponent<PopupMagnet>();
        _popupVacuum = GameObject.Find("GameScripts").GetComponent<PopupVacuum>();
    }

    public int Score {
        get { return _score; }
        private set {}
    }

    public void OnHoverIn() {
        _hover.SetActive(true);
    }

    public void OnHoverOut() {
        _hover.SetActive(false);
    }

    public void SetSelected(bool value) {
        _selected.SetActive(value);
    }

    public void OnClick() {
        if (isVacuumMode) {
            _popupVacuum.OnCubicButtonClick(_score);
        } else {
            _popupMagnet.OnCubicButtonClick(_score);
        }
    }
}

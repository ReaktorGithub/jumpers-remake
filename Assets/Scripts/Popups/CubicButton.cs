using UnityEngine;

public class CubicButton : MonoBehaviour
{
    private GameObject _selected, _hover;
    private PopupMagnet _popupMagnet;
    
    [SerializeField] private int score = 1;

    private void Awake() {
        _selected = transform.Find("SelectedImage").gameObject;
        _hover = transform.Find("HoverImage").gameObject;
        _selected.SetActive(false);
        _hover.SetActive(false);
        _popupMagnet = GameObject.Find("GameScripts").GetComponent<PopupMagnet>();
    }

    public int Score {
        get { return score; }
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
        _popupMagnet.SelectedScore = score;
        _popupMagnet.UpdateButonsSelection();
    }
}

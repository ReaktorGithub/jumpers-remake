using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorDefault;
    [SerializeField] private Texture2D cursorHover;
    private Vector2 _hotspot = new Vector2();
    private bool _disabled;

    public bool Disabled {
        get { return _disabled; }
        set {
            _disabled = value;
            bool check = Utils.RaycastPointer(transform.gameObject);
            if (check) {
                if (value) {
                    OnMouseExit();
                } else {
                    OnMouseEnter();
                }
            }
        }
    }

    public void OnMouseEnter() {
        if (!_disabled) {
            Cursor.SetCursor(cursorHover, _hotspot, CursorMode.Auto);
        }
    }

    public void OnMouseExit() {
        Cursor.SetCursor(cursorDefault, _hotspot, CursorMode.Auto);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

public class CursorEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CursorManager _cursorManager;

    private void Awake() {
        _cursorManager = GetComponent<CursorManager>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        _cursorManager.OnMouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData) {
        _cursorManager.OnMouseExit();
    }
}

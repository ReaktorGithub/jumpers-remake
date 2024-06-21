using System;
using System.Collections;
using UnityEngine;

public class TokenControl : MonoBehaviour
{
    [SerializeField] private float moveTime = 2f;
    private float _currentTime = 0f;
    private IEnumerator _coroutine;
    private string _currentCell;
    private SpriteRenderer _imageRenderer;

    private void Awake() {
        _currentCell = "start";
        _imageRenderer = transform.Find("TokenImage").gameObject.GetComponent<SpriteRenderer>();
    }

    public string CurrentCell {
        get {
            return _currentCell;
        }
        private set {}
    }

    public void ClearCoroutine() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _currentTime = 0f;
    }

    public void SetToNextCell(Action callback) {
        ClearCoroutine();
        GameObject currentObj = GameObject.Find(_currentCell);
        if (!currentObj) {
            Debug.Log("Current cell not found");
            return;
        }
        CellControl cell = currentObj.GetComponent<CellControl>();
        if (cell.NextCell == "") {
            Debug.Log("Next cell not found");
            return;
        }
        GameObject nextObj = GameObject.Find(cell.NextCell);
        _currentCell = cell.NextCell;
        _coroutine = MoveTo(nextObj.transform.position, moveTime, callback);
        StartCoroutine(_coroutine);
    }

    public IEnumerator MoveTo(Vector3 position, float moveTime, Action callback) {
        while (Vector3.Distance(transform.localPosition, position) > 0.1) {
            _currentTime += Time.fixedDeltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, position, _currentTime / moveTime);
            yield return null;
        }
        transform.localPosition = position;
        callback();
    }

    public void SetOrderInLayer(int order) {
        _imageRenderer.sortingOrder = order;
    }

    public int GetOrderInLayer() {
        return _imageRenderer.sortingOrder;
    }
}

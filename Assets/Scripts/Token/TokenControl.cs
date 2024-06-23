using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using Cinemachine;

public class TokenControl : MonoBehaviour
{
    [SerializeField] private float moveTime = 2f;
    [SerializeField] private float pedestalMoveTime = 4f;
    private float _currentTime = 0f;
    private IEnumerator _coroutine;
    private string _currentCell;
    private GameObject _tokenImage;
    private SortingGroup _sortingGroup;
    private TextMeshPro _playerName;
    [SerializeField] private float squeezeTime = 0.8f;
    [SerializeField] private float squeezeMaxValue = 5.2f;
    [SerializeField] private float squeezeMinValue = 4.6f;
    [SerializeField] private float squeezeDefaultValue = 4.6f;
    private IEnumerator _squeezeCoroutine;
    private bool _isSqueeze = false;
    private Camera _camera;

    private void Awake() {
        _currentCell = "start";
        _tokenImage = transform.Find("TokenImage").gameObject;
        _playerName = transform.Find("PlayerName").gameObject.GetComponent<TextMeshPro>();
        _sortingGroup = transform.gameObject.GetComponent<SortingGroup>();
        _camera = Camera.main;
    }

    public string CurrentCell {
        get {
            return _currentCell;
        }
        private set {}
    }

    public GameObject TokenImage {
        get {
            return _tokenImage;
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

    public IEnumerator MoveToPedestalDefer(float delay, Action callback) {
        yield return new WaitForSeconds(delay);
        StopSqueeze();
        ClearCoroutine();
        _coroutine = MoveToPedestal(callback);
        StartCoroutine(_coroutine);
    }

    public IEnumerator MoveToPedestal(Action callback) {
        Vector3 position = _camera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0));
        Vector3 goalScale = new(0f, 0f, 0f);
        while (Vector3.Distance(transform.localPosition, position) > 20) {
            _currentTime += Time.fixedDeltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, position, _currentTime / pedestalMoveTime);
            transform.localScale = Vector3.Lerp(transform.localScale, goalScale, _currentTime / pedestalMoveTime);
            yield return null;
        }
        transform.localPosition = position;
        transform.localScale = goalScale;
        callback();
    }

    public void SetOrderInLayer(int order) {
        _sortingGroup.sortingOrder = order;
    }

    public int GetOrderInLayer() {
        return _sortingGroup.sortingOrder;
    }

    // Squeeze animation

    public void StartSqueeze() {
        if (!_isSqueeze) {
            _isSqueeze = true;
            _squeezeCoroutine = Squeeze();
            StartCoroutine(_squeezeCoroutine);
        }
    }

    public void StopSqueeze() {
        if (_squeezeCoroutine != null) {
            StopCoroutine(_squeezeCoroutine);
            _isSqueeze = false;
            _tokenImage.transform.localScale = new Vector3(
                squeezeDefaultValue,
                squeezeDefaultValue,
                _tokenImage.transform.localScale.z);
        }
    }

    private IEnumerator Squeeze() {
        float testCurrent = 0f;
        while (true) {
            while (_tokenImage.transform.localScale.y > squeezeMinValue) {
                testCurrent += Time.fixedDeltaTime;
                _tokenImage.transform.localScale = new Vector3(
                    squeezeMaxValue,
                    _tokenImage.transform.localScale.y - (testCurrent / squeezeTime),
                    _tokenImage.transform.localScale.z);
                yield return null;
            }
            testCurrent = 0;
            while (_tokenImage.transform.localScale.y < squeezeMaxValue) {
                testCurrent += Time.fixedDeltaTime;
                _tokenImage.transform.localScale = new Vector3(
                    squeezeMaxValue,
                    _tokenImage.transform.localScale.y + (testCurrent / squeezeTime),
                    _tokenImage.transform.localScale.z);
                yield return null;
            }
            testCurrent = 0;
        }
    }

    // Отображаемое имя игрока

    public void SetPlayerName(string name) {
        _playerName.text = name;
    }
}

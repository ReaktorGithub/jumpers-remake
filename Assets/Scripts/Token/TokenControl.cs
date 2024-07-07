using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class TokenControl : MonoBehaviour
{
    [SerializeField] private float moveTime = 2f;
    [SerializeField] private float pedestalMoveTime = 4f;
    [SerializeField] private float squeezeTime = 0.8f;
    [SerializeField] private float squeezeMaxValue = 5.2f;
    [SerializeField] private float squeezeMinValue = 4.6f;
    [SerializeField] private float squeezeDefaultValue = 4.6f;
    [SerializeField] private float tokenScale = 0.7f;
    private IEnumerator _coroutine, _squeezeCoroutine;
    private string _currentCell = "start";
    private GameObject _tokenImage, _playerName, _playerNameBg, _skip1, _skip2, _skip3;
    private SortingGroup _sortingGroup;
    private bool _isSqueeze = false;
    private Vector3 _pedestalPosition;

    private void Awake() {
        _tokenImage = transform.Find("TokenImage").gameObject;
        _playerName = transform.Find("PlayerName").gameObject;
        _playerNameBg = transform.Find("token-text-bg").gameObject;
        _skip1 = transform.Find("skip1").gameObject;
        _skip2 = transform.Find("skip2").gameObject;
        _skip3 = transform.Find("skip3").gameObject;
        UpdateSkips(0);
        _sortingGroup = GetComponent<SortingGroup>();
        _pedestalPosition = GameObject.Find("Pedestal").transform.position;
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
    }

    public void SetToNextCell(Action callback = null) {
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

    public IEnumerator MoveTo(Vector3 position, float moveTime, Action callback = null) {
        while (Vector3.Distance(transform.localPosition, position) > 0.1) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, position, moveTime * Time.deltaTime);
            yield return null;
        }
        transform.localPosition = position;
        callback?.Invoke();
    }

    // Пьедестал

    public IEnumerator MoveToPedestalDefer(float delay, Action callback = null) {
        _playerName.SetActive(false);
        _playerNameBg.SetActive(false);
        yield return new WaitForSeconds(delay);
        StopSqueeze();
        ClearCoroutine();
        _coroutine = MoveToPedestal(callback);
        StartCoroutine(_coroutine);
    }

    public IEnumerator MoveToPedestal(Action callback = null) {
        Vector3 goalScale = new(0f, 0f, 0f);
        while (Vector3.Distance(transform.localPosition, _pedestalPosition) > 4) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _pedestalPosition, pedestalMoveTime * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, goalScale, pedestalMoveTime * Time.deltaTime);
            yield return null;
        }
        callback?.Invoke();
    }

    public void ResetView() {
        _playerName.SetActive(true);
        _playerNameBg.SetActive(true);
        transform.localScale = new Vector3(tokenScale, tokenScale, tokenScale);
    }

    // Слои

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
        bool isIn = true;
        while (true) {
            while (isIn && _tokenImage.transform.localScale.y > squeezeMinValue) {
                _tokenImage.transform.localScale = new Vector3(
                    squeezeMaxValue,
                    _tokenImage.transform.localScale.y - (squeezeTime * Time.deltaTime),
                    _tokenImage.transform.localScale.z);
                yield return null;
            }
            isIn = false;
            while (!isIn && _tokenImage.transform.localScale.y < squeezeMaxValue) {
                _tokenImage.transform.localScale = new Vector3(
                    squeezeMaxValue,
                    _tokenImage.transform.localScale.y + (squeezeTime * Time.deltaTime),
                    _tokenImage.transform.localScale.z);
                yield return null;
            }
            isIn = true;
        }
    }

    // Отображаемое имя игрока

    public void SetPlayerName(string name) {
        _playerName.GetComponent<TextMeshPro>().text = name;
    }

    // Булавки

    public void UpdateSkips(int skips) {
        _skip1.SetActive(skips == 1);
        _skip2.SetActive(skips == 2);
        _skip3.SetActive(skips > 2);
    }
}

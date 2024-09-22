using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.Splines;
using System.Collections.Generic;

public class TokenControl : MonoBehaviour
{
    private IEnumerator _coroutine, _squeezeCoroutine;
    [SerializeField] private GameObject _currentCell, _playerName, _indicators, _aiThinking, _indicatorsList, _indicatorBg;
    private GameObject _tokenImage, _skip1, _skip2, _skip3, _armor, _armorIron, _squeezable;
    private PlayerControl _playerControl;
    private SortingGroup _sortingGroup;
    private bool _isSqueeze = false;
    private GameObject _pedestal;
    private SplineAnimate _splineAnimate;
    private int _indicatorsCount = 0;

    private void Awake() {
        _squeezable = transform.Find("Squeezable").gameObject;
        _tokenImage = _squeezable.transform.Find("TokenImage").gameObject;
        _skip1 = _squeezable.transform.Find("skip1").gameObject;
        _skip2 = _squeezable.transform.Find("skip2").gameObject;
        _skip3 = _squeezable.transform.Find("skip3").gameObject;
        _armor = _squeezable.transform.Find("armor").gameObject;
        _armorIron = _squeezable.transform.Find("armor-iron").gameObject;
        UpdateSkips(0);
        UpdateShield(EBoosters.None);
        ShowAi(false);
        _sortingGroup = GetComponent<SortingGroup>();
        _pedestal = GameObject.Find("Pedestal");
        _splineAnimate = GetComponent<SplineAnimate>();
        RemoveAllIndicators();
    }

    public GameObject CurrentCell {
        get { return _currentCell; }
        set { _currentCell = value; }
    }

    public GameObject TokenImage {
        get { return _tokenImage; }
        private set {}
    }

    public PlayerControl PlayerControl {
        get { return _playerControl; }
        set { _playerControl = value; }
    }

    public Sprite GetArmorSprite() {
        return _armor.GetComponent<SpriteRenderer>().sprite;
    }

    public Sprite GetArmorIronSprite() {
        return _armorIron.GetComponent<SpriteRenderer>().sprite;
    }

    public void ClearCoroutine() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
    }

    public CellControl GetCurrentCellControl() {
        return _currentCell.GetComponent<CellControl>();
    }

    public void DisableIndicators(bool value) {
        _indicators.SetActive(!value);
    }

    public void SetToNextCell(float moveTime, Action callback = null) {
        ClearCoroutine();
        if (_currentCell == null) {
            Debug.Log("Current cell not found");
            return;
        }
        CellControl cell = _currentCell.GetComponent<CellControl>();
        GameObject nextCell = MoveControl.Instance.CurrentPlayer.IsReverseMove ? cell.PreviousCell : cell.NextCell;

        if (nextCell == null) {
            Debug.Log("Next cell not specified");
            return;
        }
        _currentCell = nextCell;
        _coroutine = Utils.MoveTo(transform.gameObject, nextCell.transform.position, moveTime, callback);
        StartCoroutine(_coroutine);
    }

    public void SetToSpecifiedCell(GameObject nextCell, float moveTime, Action callback = null) {
        ClearCoroutine();
        _currentCell = nextCell;
        _coroutine = Utils.MoveTo(transform.gameObject, nextCell.transform.position, moveTime, callback);
        StartCoroutine(_coroutine);
    }

    // Пьедестал

    public IEnumerator MoveToPedestalDefer(float delay, Action callback = null) {
        DisableIndicators(true);
        yield return new WaitForSeconds(delay);
        StopSqueeze();
        ClearCoroutine();
        _coroutine = MoveToPedestal(callback);
        StartCoroutine(_coroutine);
    }

    public IEnumerator MoveToPedestal(Action callback = null) {
        Vector3 goalScale = new(0f, 0f, 0f);
        Vector3 pedestalPosition = _pedestal.transform.position;
        float pedestalMoveTime = TokensControl.Instance.PedestalMoveTime;

        while (Vector3.Distance(transform.localPosition, pedestalPosition) > 4) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, pedestalPosition, pedestalMoveTime * Time.deltaTime);
            transform.localScale = Vector3.Lerp(transform.localScale, goalScale, pedestalMoveTime * Time.deltaTime);
            yield return null;
        }

        callback?.Invoke();
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
            _squeezable.transform.localScale = new Vector3(
                TokensControl.Instance.SqueezeDefaultValue,
                TokensControl.Instance.SqueezeDefaultValue,
                _squeezable.transform.localScale.z);
        }
    }

    private IEnumerator Squeeze() {
        bool isIn = true;
        while (true) {
            while (isIn && _squeezable.transform.localScale.y > TokensControl.Instance.SqueezeMinValue) {
                _squeezable.transform.localScale = new Vector3(
                    TokensControl.Instance.SqueezeMaxValue,
                    _squeezable.transform.localScale.y - (TokensControl.Instance.SqueezeTime * Time.deltaTime),
                    _squeezable.transform.localScale.z);
                yield return null;
            }
            isIn = false;
            while (!isIn && _squeezable.transform.localScale.y < TokensControl.Instance.SqueezeMaxValue) {
                _squeezable.transform.localScale = new Vector3(
                    TokensControl.Instance.SqueezeMaxValue,
                    _squeezable.transform.localScale.y + (TokensControl.Instance.SqueezeTime * Time.deltaTime),
                    _squeezable.transform.localScale.z);
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

    // Разные иконки

    public void ShowAi(bool value) {
        _aiThinking.SetActive(value);
    }

    // Броня

    public void UpdateShield(EBoosters booster) {
        switch(booster) {
            case EBoosters.Shield: {
                _armor.SetActive(true);
                _armorIron.SetActive(false);
                break;
            }
            case EBoosters.ShieldIron: {
                _armor.SetActive(false);
                _armorIron.SetActive(true);
                break;
            }
            default: {
                _armor.SetActive(false);
                _armorIron.SetActive(false);
                break;
            }
        }
    }

    // перемещения по стрелкам

    public void ExecuteArrowMove(SplineContainer spline, GameObject nextCell = null) {
        DisableIndicators(true);
        _splineAnimate.Container = spline;
        _splineAnimate.Restart(false);
        _splineAnimate.Play();
        StartCoroutine(ConfirmNewArrowPositionDefer(nextCell));
    }

    public IEnumerator ConfirmNewArrowPositionDefer(GameObject nextCell = null) {
        yield return new WaitForSeconds(TokensControl.Instance.ArrowMovingTime);
        ConfirmNewArrowPosition(nextCell);
    }

    public void ConfirmNewArrowPosition(GameObject nextCell = null) {
        _splineAnimate.StopAllCoroutines();
        _splineAnimate.Container = null;
        if (nextCell == null) {
            // обычная стрелка
            CellControl currentCellControl = GetCurrentCellControl();
            currentCellControl.RemoveToken(transform.gameObject);
            _currentCell = currentCellControl.transform.GetComponent<ArrowCell>().ArrowToCell;
        } else {
            // hedgehog
            _currentCell = nextCell;
            CellControl currentCellControl = GetCurrentCellControl();
            currentCellControl.RemoveToken(transform.gameObject);
        }
        transform.SetLocalPositionAndRotation(_currentCell.transform.localPosition, Quaternion.Euler(new Vector3(0,0,0)));
        DisableIndicators(false);
        MoveControl.Instance.ConfirmNewPosition();
    }

    // Индикаторы

    private List<TokenIndicator> GetAllIndicators() {
        List<TokenIndicator> result = new();

        Transform[] children = _indicatorsList.GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            if (child.CompareTag("TokenIndicator")) {
                TokenIndicator control = child.gameObject.GetComponent<TokenIndicator>();
                result.Add(control);
            }
        }

        return result;
    }

    public void RemoveIndicator(ETokenIndicators type) {
        List<TokenIndicator> indicators = GetAllIndicators();

        foreach (TokenIndicator indicator in indicators) {
            if (indicator.Type == type) {
                _indicatorsCount--;
                Destroy(indicator.gameObject);
            }
        }

        // если индикаторов не осталось, то убрать фон
        _indicatorBg.SetActive(_indicatorsCount != 0);
    }

    public void RemoveAllIndicators() {
        List<TokenIndicator> indicators = GetAllIndicators();

        foreach (TokenIndicator indicator in indicators) {
            Destroy(indicator.gameObject);
        }

        _indicatorsCount = 0;
        _indicatorBg.SetActive(false);
    }

    public void AddIndicator(ETokenIndicators type, string text = "") {
        GameObject clone = Instantiate(TokensControl.Instance.TokenIndicatorSample);
        clone.transform.SetParent(_indicatorsList.transform);
        TokenIndicator indicator = clone.GetComponent<TokenIndicator>();
        indicator.UpdateLinks();
        indicator.Type = type;
        indicator.SetText(TokensControl.Instance.IndicatorWidthSmall, TokensControl.Instance.IndicatorWidthDefault, text);

        switch(type) {
            case ETokenIndicators.Lightning: {
                indicator.SetSprite(TokensControl.Instance.IndicatorLightningSprite);
                indicator.SetTextColor(new Color32(49,255,34,255));
                break;
            }
            default: {
                indicator.SetSprite(null);
                break;
            }
        }

        clone.SetActive(true);
        clone.transform.localScale = new Vector3(1f,1f,1f);
        _indicatorBg.SetActive(true);
        _indicatorsCount++;
    }

    public void UpdateIndicator(ETokenIndicators type, string newText) {
        List<TokenIndicator> indicators = GetAllIndicators();

        foreach (TokenIndicator indicator in indicators) {
            if (indicator.Type == type) {
                indicator.SetText(TokensControl.Instance.IndicatorWidthSmall, TokensControl.Instance.IndicatorWidthDefault, newText);
            }
        }
    }
}

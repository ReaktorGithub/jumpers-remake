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
    [SerializeField] private GameObject _currentCell, _playerName, _indicators, _aiThinking, _indicatorsList, _indicatorBg, _bonusEventsList, _stuck;
    [SerializeField] private float _bonushFlashTime = 3f;
    [SerializeField] private float _nextBonusTime = 1.5f;
    private GameObject _tokenImage, _skip1, _skip2, _skip3, _armor, _armorIron, _squeezable, _rotate;
    private PlayerControl _playerControl;
    private SortingGroup _sortingGroup;
    private bool _isSqueeze = false;
    private GameObject _pedestal;
    private SplineAnimate _splineAnimate;
    private int _indicatorsCount = 0;
    private List<(string, Color32)> _bonusQueue = new();
    private bool _isProcessingQueue = false;
    private TokenStuck _stuckScript;
    private Animator _teleportAnimator;
    private SpriteRenderer _oreol;

    private void Awake() {
        _rotate = transform.Find("Rotate").gameObject;
        _squeezable = _rotate.transform.Find("Squeezable").gameObject;
        _tokenImage = _squeezable.transform.Find("TokenImage").gameObject;
        _skip1 = _squeezable.transform.Find("skip1").gameObject;
        _skip2 = _squeezable.transform.Find("skip2").gameObject;
        _skip3 = _squeezable.transform.Find("skip3").gameObject;
        _armor = _squeezable.transform.Find("armor").gameObject;
        _armorIron = _squeezable.transform.Find("armor-iron").gameObject;
        _sortingGroup = GetComponent<SortingGroup>();
        _pedestal = GameObject.Find("Pedestal");
        _splineAnimate = GetComponent<SplineAnimate>();
        _stuckScript = _stuck.GetComponent<TokenStuck>();
        _teleportAnimator = _rotate.GetComponent<Animator>();
        _oreol = transform.Find("Indicators").transform.Find("Oreol").GetComponent<SpriteRenderer>();
    }

    private void Start() {
        UpdateSkips(0);
        UpdateShield(EBoosters.None);
        ShowAi(false);
        RemoveAllIndicators();
        SetOreol(false);
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

    public void SetToNextCell(bool isReverseMode, float moveTime, Action callback = null) {
        ClearCoroutine();

        if (_currentCell == null) {
            Debug.Log("Current cell not found");
            return;
        }

        CellControl cell = _currentCell.GetComponent<CellControl>();
        GameObject nextCell = isReverseMode ? cell.PreviousCell : cell.NextCell;

        if (nextCell == null) {
            Debug.Log("Next cell not specified");
            callback?.Invoke();
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

    public void UpdateStuckVisual(int count) {
        _stuckScript.UpdateStuckVisual(count);
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
        GameObject image = clone.transform.Find("Image").gameObject;
        clone.transform.SetParent(_indicatorsList.transform);
        TokenIndicator indicator = clone.GetComponent<TokenIndicator>();
        indicator.UpdateLinks();
        indicator.Type = type;
        indicator.SetText(TokensControl.Instance.IndicatorWidthSmall, TokensControl.Instance.IndicatorWidthDefault, text);

        switch(type) {
            case ETokenIndicators.Lightning: {
                indicator.SetSprite(TokensControl.Instance.IndicatorLightningSprite);
                indicator.SetTextColor(new Color32(49,255,34,255));
                image.transform.localScale = new Vector3(0.75f,0.75f,0.75f);
                break;
            }
            case ETokenIndicators.Flash: {
                indicator.SetSprite(TokensControl.Instance.IndicatorFlashSprite);
                indicator.SetTextColor(new Color32(173,208,255,255));
                image.transform.localScale = new Vector3(3f,3f,3f);
                break;
            }
            case ETokenIndicators.Blot: {
                indicator.SetSprite(TokensControl.Instance.IndicatorBlotSprite);
                indicator.SetTextColor(new Color32(180,180,180,180));
                image.transform.localScale = new Vector3(3f,3f,3f);
                break;
            }
            default: {
                indicator.SetSprite(null);
                break;
            }
        }

        clone.SetActive(true);
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

    // Событие появления бонуса

    public void AddBonusEventToQueue(string message, Color32 color) {
        if (!transform.gameObject.activeInHierarchy || !_indicators.activeInHierarchy || message == "0") {
            return;
        }

        _bonusQueue.Add((message, color));

        if (!_isProcessingQueue) {
            StartCoroutine(StartBonusQueue());
        }
    }

    private IEnumerator StartBonusQueue() {
        _isProcessingQueue = true;

        while (_bonusQueue.Count > 0) {
            FlashBonusEvent(_bonusQueue[0].Item1, _bonusQueue[0].Item2);
            _bonusQueue.RemoveAt(0);
            yield return new WaitForSeconds(_nextBonusTime);
        }

        _isProcessingQueue = false; // Завершаем обработку очереди
    }

    private void FlashBonusEvent(string message, Color32 color) {
        GameObject clone = Instantiate(TokensControl.Instance.BonusEventSample);
        clone.transform.SetParent(_bonusEventsList.transform);
        clone.transform.localScale = new Vector3(1f,1f,1f);

        TextMeshPro text = clone.transform.Find("Text (TMP)").GetComponent<TextMeshPro>();
        text.text = message;
        text.color = color;

        Animator animator = clone.GetComponent<Animator>();
        clone.SetActive(true);
        animator.SetBool("isFlash", true);
        
        StartCoroutine(DestroyBonusEventDefer(clone));
    }

    private IEnumerator DestroyBonusEventDefer(GameObject bonusObj) {
        yield return new WaitForSeconds(_bonushFlashTime);
        Animator animator = bonusObj.GetComponent<Animator>();
        animator.SetBool("isFlash", false);
        Destroy(bonusObj);
    }

    public void StartTeleportInAnimation() {
        _teleportAnimator.SetInteger("teleport", 1);
    }

    public void StartTeleportOutAnimation() {
        _teleportAnimator.SetInteger("teleport", 2);
        StartCoroutine(StopTeleportAnimation());
    }

    private IEnumerator StopTeleportAnimation() {
        yield return new WaitForSeconds(TokensControl.Instance.TeleportAnimationTime);
        _teleportAnimator.SetInteger("teleport", 0);
    }

    public void SetOreol(bool isOn, int level = 1) {
        float scale = level > 1 ? TokensControl.Instance.OreolScale2 : TokensControl.Instance.OreolScale1;
        _oreol.transform.localScale = new Vector3(
            scale,
            scale,
            _oreol.transform.localScale.z
        );
        _oreol.gameObject.SetActive(isOn);
    }
}

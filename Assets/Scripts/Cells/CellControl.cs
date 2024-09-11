using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellControl : MonoBehaviour
{
    // todo добавить _ перед переделыванием уровня
    [SerializeField] private GameObject nextCell;
    [SerializeField] private GameObject previousCell;
    [SerializeField] private ECellTypes cellType = ECellTypes.None;
    [SerializeField] private EControllableEffects effect = EControllableEffects.None;
    private GameObject _container, _glow;
    private SpriteRenderer _spriteRenderer, _glowSpriteRenderer;
    private float[] _cellScale = new float[2];
    private List<GameObject> _currentTokens = new();
    private bool _isEffectPlacementMode, _isLassoMode = false;
    private TextMeshPro _text;
    private bool _isChanging = false;
    private IEnumerator _changingCoroutine;
    private Sprite _oldSprite, _newSprite;
    private Color _oldTextColor, _newTextColor;
    private IEnumerator _coroutine;
    private float _pulseTime = 1.2f;
    private float _pulseMinAlpha = 0.2f;
    private CursorManager _cursorManager;
    [SerializeField] private int _aiScore = 0; // чем выше число, тем больше эта клетка нравится ai

    private void Awake() {
        _container = transform.Find("container").gameObject;
        _spriteRenderer = _container.transform.Find("cell").gameObject.GetComponent<SpriteRenderer>();
        _glow = _container.transform.Find("glow").gameObject;
        _glowSpriteRenderer = _glow.GetComponent<SpriteRenderer>();
        _glow.SetActive(false);
        _cellScale[0] = 1;
        _cellScale[1] = 1.15f;
        Transform number = _container.transform.Find("number");
        if (number != null) {
            _text = number.GetComponent<TextMeshPro>();
        }
        Transform button = transform.Find("button");
        if (button != null) {
            _cursorManager = button.GetComponent<CursorManager>();
        }
    }

    private void Start() {
        SetCursorDisabled(true);
        UpdateAiScore();
    }

    private void SetCursorDisabled(bool value) {
        if (_cursorManager != null) {
            _cursorManager.Disabled = value;
        }
    }

    public GameObject NextCell {
        get { return nextCell; }
        set { nextCell = value; }
    }

    public GameObject PreviousCell {
        get { return previousCell; }
        set { previousCell = value; }
    }

    public ECellTypes CellType {
        get { return cellType; }
        private set {}
    }

    public int AiScore {
        get { return _aiScore; }
        private set {}
    }

    public EControllableEffects Effect {
        get { return effect; }
        set {
            effect = value;
            UpdateAiScore();
        }
    }

    public List<GameObject> CurrentTokens {
        get { return _currentTokens; }
        set { _currentTokens = value; }
    }

    public bool HasToken(GameObject tokenObject) {
        return _currentTokens.Contains(tokenObject);
    }

    public bool IsNoTokens() {
        return _currentTokens.Count == 0;
    }

    public void AddToken(GameObject tokenObject) {
        _currentTokens.Add(tokenObject);
    }

    public void RemoveToken(GameObject tokenObject) {
        if (HasToken(tokenObject)) {
            _currentTokens.Remove(tokenObject);
        }
    }

    public string GetCellText() {
        return _text.text;
    }

    public bool IsSelectionMode() {
        return _isEffectPlacementMode || _isLassoMode;
    }

    public bool IsPenaltyEffect() {
        return effect == EControllableEffects.Black || effect == EControllableEffects.Red;
    }

    // Перераспределить позиции фишек на клетке

    public void AlignTokens(float moveTime, Action callback = null) {
        int done = 0;
        int coroutinesCompleted = 0;

        foreach(GameObject tokenObject in _currentTokens) {
            if (tokenObject == null) {
                coroutinesCompleted++;
                continue;
            }
            TokenControl tokenControl = tokenObject.GetComponent<TokenControl>();
            Vector3 pos;
            switch(_currentTokens.Count) {
                case 2:
                if (done == 0) {
                    pos = new(transform.position.x - 1.2f, transform.position.y - 1.2f, 0);
                } else {
                    pos = new(transform.position.x + 1.2f, transform.position.y + 1.2f, 0);
                }
                done++;
                break;
                case 3:
                if (done == 0) {
                    pos = new(transform.position.x - 1.2f, transform.position.y - 1.5f, 0);
                } else if (done == 1) {
                    pos = new(transform.position.x - 0.4f, transform.position.y + 1f, 0);
                } else {
                    pos = new(transform.position.x + 1.4f, transform.position.y - 0.8f, 0);
                }
                done++;
                break;
                case 4:
                if (done == 0) {
                    pos = new(transform.position.x - 1.3f, transform.position.y - 1.4f, 0);
                } else if (done == 1) {
                    pos = new(transform.position.x - 1f, transform.position.y + 0.7f, 0);
                } else if (done == 2) {
                    pos = new(transform.position.x + 1.4f, transform.position.y + 1.2f, 0);
                } else {
                    pos = new(transform.position.x + 1.3f, transform.position.y - 1.1f, 0);
                }
                done++;
                break;
                default:
                pos = transform.position;
                break;
            }
            tokenControl.ClearCoroutine();
            StartCoroutine(Utils.MoveTo(tokenControl.gameObject, pos, moveTime, () => {
                coroutinesCompleted++;
                if (coroutinesCompleted == _currentTokens.Count) {
                    callback?.Invoke();
                }
            }));
        }
    }

    // выбор клетки игроком

    public void TurnOnEffectPlacementMode() {
        bool newValue = cellType == ECellTypes.None && effect == EControllableEffects.None && IsNoTokens();
        _isEffectPlacementMode = newValue;
        SetCursorDisabled(!newValue);
    }

    public void TurnOffEffectPlacementMode() {
        DownscaleCell();
        _isEffectPlacementMode = false;
        SetCursorDisabled(true);
    }

    public void TurnOnGlow() {
        _glow.SetActive(true);
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _coroutine = Utils.StartPulse(_glowSpriteRenderer, _pulseTime, _pulseMinAlpha);
        StartCoroutine(_coroutine);
    }

    public void TurnOffGlow() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _glow.SetActive(false);
    }

    public void TurnOnLassoMode() {
        _isLassoMode = true;
        SetCursorDisabled(false);
        TurnOnGlow();
    }

    public void TurnOffLassoMode() {
        DownscaleCell();
        _isLassoMode = false;
        SetCursorDisabled(true);
        TurnOffGlow();
    }

    // Флаг force принудительно меняет размер клетки, даже если мы не находимся в режиме выбора

    public void UpscaleCell(bool force = false) {
        if (!force && !IsSelectionMode()) {
            return;
        }
        _spriteRenderer.sortingOrder = 2;
        _glowSpriteRenderer.sortingOrder = 3;
        _container.transform.localScale = new Vector3(
            _cellScale[1],
            _cellScale[1],
            _container.transform.localScale.z
        );
    }

    public void DownscaleCell(bool force = false) {
        if (!force && !IsSelectionMode()) {
            return;
        }
        _spriteRenderer.sortingOrder = 0;
        _glowSpriteRenderer.sortingOrder = 1;
        _container.transform.localScale = new Vector3(
            _cellScale[0],
            _cellScale[0],
            _container.transform.localScale.z
        );
    }

    public void OnClick() {
        if (_isEffectPlacementMode) {
            EffectsControl.Instance.OnConfirmChangeEffect(this);
        } else if (_isLassoMode) {
            BoostersControl.Instance.ExecuteLasso(this);
        }
    }

    // установка нового эффекта на эту клетку

    public void ChangeEffect(EControllableEffects newEffect, Sprite newSprite) {
        Effect = newEffect;

        // Наложение спрайта и изменение цвета текста

        _oldSprite = _spriteRenderer.sprite;
        _newSprite = newSprite;
        _oldTextColor = _text.color;

        switch(newEffect) {
            case EControllableEffects.Yellow:
            case EControllableEffects.Red:
            case EControllableEffects.Green: {
                Color newCol;
                if (ColorUtility.TryParseHtmlString(UIColors.CellGrey, out newCol)) {
                    _newTextColor = newCol;
                }
                break;
            }
            default: {
                Color newCol;
                if (ColorUtility.TryParseHtmlString(UIColors.CellDefault, out newCol)) {
                    _newTextColor = newCol;
                }
                break;
            }
        }

        // Назначение скриптов

        TryGetComponent(out RedCell redCellComponent);

        if (newEffect == EControllableEffects.Red) {
            if (redCellComponent == null) {
                transform.gameObject.AddComponent<RedCell>();
            }
        } else {
            if (redCellComponent != null) {
                Destroy(redCellComponent);
            }
        }

        // визуальное отображение без анимации

        SetNewEffect();
    }

    // анимация нанесения нового эффекта

    private void SetNewEffect() {
        _spriteRenderer.sprite = _newSprite;
        _text.color = _newTextColor;
    }

    private void SetOldEffect() {
        _spriteRenderer.sprite = _oldSprite;
        _text.color = _oldTextColor;
    }

    public void StartChanging() {
        if (!_isChanging) {
            _isChanging = true;
            _changingCoroutine = Changing();
            StartCoroutine(_changingCoroutine);
            StartCoroutine(ChangingAnimationScheduler());
        }
    }

    public void StopChanging() {
        if (_changingCoroutine != null) {
            StopCoroutine(_changingCoroutine);
            _isChanging = false;
            SetNewEffect();
            _newSprite = null;
            _oldSprite = null;
        }
    }

    private IEnumerator Changing() {
        while (true) {
            SetNewEffect();
            yield return new WaitForSeconds(CellsControl.Instance.ChangingEffectTime);
            SetOldEffect();
            yield return new WaitForSeconds(CellsControl.Instance.ChangingEffectTime);
        }
    }

    private IEnumerator ChangingAnimationScheduler() {
        yield return new WaitForSeconds(CellsControl.Instance.ChangingEffectDuration);
        StopChanging();
    }

    // разное

    public bool IsNegativeEffect() {
        return Manual.Instance.GetEffectCharacter(effect) == EResourceCharacters.Negative;
    }

    private void UpdateAiScore() {
        if (CellType == ECellTypes.Finish) {
            _aiScore = 1000;
            return;
        }

        if (effect == EControllableEffects.Red || effect == EControllableEffects.Black) {
            _aiScore = -20;
            return;
        }

        int points = 0;

        if (CellType == ECellTypes.Lightning || CellType == ECellTypes.Moneybox || CellType == ECellTypes.Surprise || effect == EControllableEffects.Green) points += 8;
        if (effect == EControllableEffects.Star) points += 12;
        if (effect == EControllableEffects.Yellow) points -= 8;

        bool isArrow = transform.TryGetComponent(out ArrowCell arrow);
        if (isArrow) {
            int add = arrow.IsForward ? 6 : -6;
            points += add;
        }

        _aiScore = points;
    }
}

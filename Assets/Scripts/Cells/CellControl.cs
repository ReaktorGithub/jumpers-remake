using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellControl : MonoBehaviour
{
    // todo добавить _ перед переделыванием уровня
    [SerializeField] private GameObject nextCell, previousCell;
    [SerializeField] private ECellTypes cellType = ECellTypes.None;
    [SerializeField] private EControllableEffects effect = EControllableEffects.None;
    [SerializeField] private int _effectLevel = 1;
    private int _oldEffectLevel = 1;
    [SerializeField] private int _coinBonusValue = 0;
    [SerializeField] private bool _enableReverse = false;
    // Если _enableReverse = true, то клетка находится в ответвлении со стеной. Нахождение фишки на этой клетке никак не влияет на её направление.
    // Если _enableReverse = false, то клетка обычная. После остановки фишки у игрока будет принудительно сменено направление на "вперед"
    private GameObject _container, _glow, _coinBonusObject, _brick, _boombaster;
    private SpriteRenderer _spriteRenderer, _glowSpriteRenderer, _grindSpriteRenderer;
    private float[] _cellScale = new float[2];
    [SerializeField] private List<GameObject> _currentTokens = new();
    private bool _isEffectPlacementMode, _isLassoMode = false;
    private TextMeshPro _text, _coinBonusText, _boombasterText;
    private bool _isChanging = false;
    private IEnumerator _changingCoroutine;
    private Sprite _oldSprite, _newSprite;
    private Color _oldTextColor, _newTextColor;
    private IEnumerator _coroutine;
    private float _pulseTime = 1.2f;
    private float _pulseMinAlpha = 0.2f;
    private CursorManager _cursorManager;
    [SerializeField] private int _aiScore = 0; // чем выше число, тем больше эта клетка нравится ai
    private bool _isBoombaster = false;
    private int _boombasterTimer = 9;

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
        _coinBonusObject = _container.transform.Find("CoinBonus").gameObject;
        _coinBonusText = _coinBonusObject.transform.Find("CoinBonusText").GetComponent<TextMeshPro>();
        _grindSpriteRenderer = _container.transform.Find("grind").GetComponent<SpriteRenderer>();
        _brick = _container.transform.Find("brick").gameObject;
        _boombaster = _container.transform.Find("Boombaster").gameObject;
        _boombasterText = _boombaster.transform.Find("timer").GetComponent<TextMeshPro>();
    }

    private void Start() {
        SetCursorDisabled(true);
        UpdateAiScore();
        UpdateCoinBonusView();
        UpdateGrindVisual(_effectLevel);
        UpdateBoombasterVisual();
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

    public bool IsBoombaster {
        get { return _isBoombaster; }
        set {_isBoombaster = value; }
    }

    public int BoombasterTimer {
        get { return _boombasterTimer; }
        set {
            _boombasterTimer = value;
            if (value == -1 && IsBoombaster) {
                ExecuteBoombasterExplosion();
            } else {
                UpdateBoombasterVisual();
            }
        }
    }

    public bool EnableReverse {
        get { return _enableReverse; }
        set { _enableReverse = value; }
    }

    public int CoinBonusValue {
        get { return _coinBonusValue; }
        set {
            _coinBonusValue = value;
            UpdateCoinBonusView();
        }
    }

    public EControllableEffects Effect {
        get { return effect; }
        set {
            effect = value;
            UpdateAiScore();
        }
    }

    public int EffectLevel {
        get { return _effectLevel; }
        private set {}
    }

    public List<GameObject> CurrentTokens {
        get { return _currentTokens; }
        set { _currentTokens = value; }
    }

    public List<PlayerControl> GetCurrentPlayers() {
        List<PlayerControl> result = new();

        foreach(GameObject obj in _currentTokens) {
            TokenControl token = obj.GetComponent<TokenControl>();
            result.Add(token.PlayerControl);
        }

        return result;
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

    public bool IsWallEffect() {
        return EffectLevel == 3 && IsPenaltyEffect();
    }

    public void UpdateGrindVisual(int level) {
        switch(level) {
            case 2: {
                _grindSpriteRenderer.sprite = CellsControl.Instance.Grind2Sprite;
                break;
            }
            case 3: {
                _grindSpriteRenderer.sprite = CellsControl.Instance.Grind3Sprite;
                break;
            }
            default: {
                _grindSpriteRenderer.sprite = null;
                break;
            }
        }

        bool isWall = level == 3 && IsPenaltyEffect();
        _brick.SetActive(isWall);
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

    public void ChangeEffect(EControllableEffects newEffect, Sprite newSprite, int level) {
        _oldEffectLevel = _effectLevel;
        _effectLevel = level;
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
        UpdateGrindVisual(_effectLevel);
    }

    private void SetOldEffect() {
        _spriteRenderer.sprite = _oldSprite;
        _text.color = _oldTextColor;
        UpdateGrindVisual(_oldEffectLevel);
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

        if (_coinBonusValue > 90) {
            _aiScore = 20;
            return;
        } else if (_coinBonusValue > 30) {
            points += 5;
        } else if (_coinBonusValue < -90) {
            _aiScore = -20;
            return;
        } else if (_coinBonusValue < -30) {
            points -= 5;
        }

        bool isArrow = transform.TryGetComponent(out ArrowCell arrow);
        if (isArrow) {
            int add = arrow.IsForward ? 6 : -6;
            points += add;
        }

        _aiScore = points;
    }

    private void UpdateCoinBonusView() {
        if (_coinBonusValue == 0) {
            _coinBonusObject.SetActive(false);
            return;
        }

        (string, Color32) values = Utils.GetTextWithSymbolAndColor(_coinBonusValue);
        _coinBonusText.text = values.Item1;
        _coinBonusText.color = values.Item2;
        _coinBonusObject.SetActive(true);
    }

    // Если произошел взрыв, то возвращает true

    public bool TickBoombaster() {
        BoombasterTimer--;
        return BoombasterTimer == -1;
    }

    private void UpdateBoombasterVisual() {
        _boombasterText.text = _boombasterTimer.ToString();
        _boombaster.SetActive(_isBoombaster);
    }

    private void ExecuteBoombasterExplosion() {
        IsBoombaster = false;
        UpdateBoombasterVisual();
        CellsControl.Instance.ExecuteBoombasterExplosion(this);
    }
}

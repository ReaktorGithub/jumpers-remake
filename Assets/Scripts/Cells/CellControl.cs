using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellControl : MonoBehaviour
{
    [SerializeField] private GameObject nextCell;
    [SerializeField] private GameObject previousCell;
    [SerializeField] private ECellTypes cellType = ECellTypes.None;
    [SerializeField] private EControllableEffects effect = EControllableEffects.None;
    private GameObject _container;
    private SpriteRenderer _spriteRenderer;
    private float[] cellScale = new float[2];
    private List<string> _currentTokens = new();
    private bool _isEffectPlacementMode = false;
    private TextMeshPro _text;
    private bool _isChanging = false;
    private IEnumerator _changingCoroutine;
    private Sprite _oldSprite, _newSprite;
    private Color _oldTextColor, _newTextColor;

    private void Awake() {
        _container = transform.Find("container").gameObject;
        _spriteRenderer = _container.transform.Find("cell").gameObject.GetComponent<SpriteRenderer>();
        cellScale[0] = 1;
        cellScale[1] = 1.15f;
        Transform number = _container.transform.Find("number");
        if (number != null) {
            _text = number.GetComponent<TextMeshPro>();
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

    public EControllableEffects Effect {
        get { return effect; }
        set {}
    }

    public List<string> CurrentTokens {
        get { return _currentTokens; }
        set { _currentTokens = value; }
    }

    public bool HasToken(string tokenName) {
        return _currentTokens.Contains(tokenName);
    }

    public bool IsNoTokens() {
        return _currentTokens.Count == 0;
    }

    public void AddToken(string tokenName) {
        _currentTokens.Add(tokenName);
    }

    public void RemoveToken(string tokenName) {
        if (HasToken(tokenName)) {
            _currentTokens.Remove(tokenName);
        }
    }

    // Перераспределить позиции фишек на клетке

    public void AlignTokens(float moveTime, Action callback = null) {
        int done = 0;
        int coroutinesCompleted = 0;

        foreach(string tokenName in _currentTokens) {
            if (tokenName == null) {
                coroutinesCompleted++;
                continue;
            }
            TokenControl tokenControl = GameObject.Find(tokenName).GetComponent<TokenControl>();
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

    // вывести в дебаг клетки, где есть фишки

    public void ShowTokensAtCells() {
        GameObject[] allCells = GameObject.FindGameObjectsWithTag("cell");
        foreach(GameObject obj in allCells) {
            CellControl cell = obj.GetComponent<CellControl>();
            if (cell._currentTokens.Count == 0) {
                continue;
            }
            string message = "Tokens at " + cell.name + ": ";
            foreach(string token in cell._currentTokens) {
                message = message + token + " ";
            }
            Debug.Log(message);
        }
    }

    // выбор клетки игроком

    public void TurnOnEffectPlacementMode() {
        _isEffectPlacementMode = cellType == ECellTypes.None && effect == EControllableEffects.None && IsNoTokens();
    }

    public void TurnOffEffectPlacementMode() {
        DownscaleCell();
        _isEffectPlacementMode = false;
    }

    public void UpscaleCell() {
        if (!_isEffectPlacementMode) {
            return;
        }
        _spriteRenderer.sortingOrder = 1;
        _container.transform.localScale = new Vector3(
            cellScale[1],
            cellScale[1],
            _container.transform.localScale.z
        );
    }

    public void DownscaleCell() {
        if (!_isEffectPlacementMode) {
            return;
        }
        _spriteRenderer.sortingOrder = 0;
        _container.transform.localScale = new Vector3(
            cellScale[0],
            cellScale[0],
            _container.transform.localScale.z
        );
    }

    public void OnClick() {
        if (!_isEffectPlacementMode) {
            return;
        }
        EffectsControl.Instance.OnConfirmChangeEffect(this);
    }

    // установка нового эффекта на эту клетку

    public void ChangeEffect(EControllableEffects newEffect, Sprite newSprite) {
        effect = newEffect;

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
}

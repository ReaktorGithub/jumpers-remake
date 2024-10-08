using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubicControl : MonoBehaviour
{
    public static CubicControl Instance { get; private set; }
    private Button _cubicButton;
    private CursorManager _cursorManager;
    private Animator _anim;
    private IEnumerator _coroutine, _coroutinePulse;
    private TextMeshProUGUI _statusText;
    [SerializeField] private float _rotateTime = 1.4f;
    [SerializeField] private float _holdTime = 1f;
    [SerializeField] private float _pulseTime = 0.5f;
    [SerializeField] private float _pulseMinAlpha = 0.2f;
    [SerializeField] private float _showFinalScoreDelay = 0.2f;
    private int _finalScore;
    private GameObject _border, _borderSelect, _finalScoreDisplay;
    private TextMeshProUGUI _finalScoreDisplayText;
    private PopupMagnet _popupMagnet;
    private ModifiersControl _modifiersControl;

    private void Awake() {
        Instance = this;
        _anim = transform.Find("CubicImage").GetComponent<Animator>();
        _cubicButton = transform.Find("CubicButton").GetComponent<Button>();
        _statusText = transform.Find("StatusText").GetComponent<TextMeshProUGUI>();
        _statusText.text = "";
        _border = transform.Find("cubic_border").gameObject;
        _borderSelect = transform.Find("cubic_border_select").gameObject;
        _popupMagnet = GameObject.Find("GameScripts").GetComponent<PopupMagnet>();
        _cursorManager = _cubicButton.GetComponent<CursorManager>();
        _modifiersControl = GameObject.Find("Modifiers").GetComponent<ModifiersControl>();
        _finalScoreDisplay = GameObject.Find("FinalScore");
        _finalScoreDisplayText = _finalScoreDisplay.transform.Find("score").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        _border.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Space) && _cubicButton.interactable) {
            Throw();
        }
    }

    public ModifiersControl ModifiersControl {
        get { return _modifiersControl; }
        private set {}
    }

    public void OnCubicClick(int specifiedScore = 0) {
        Throw(specifiedScore);
    }

    // specifiedScore - указать на выброс определенного числа, без рандома
    // finalScore - сохранение числа перемещений фишки после всех вычислений

    public void Throw(int specifiedScore = 0, bool isMagnet = false) {
        SetCubicInteractable(false);
        EffectsControl.Instance.DisableAllButtons(true);
        BoostersControl.Instance.DisableAllButtons();
        _anim.SetInteger("score", 0);
        _anim.SetBool("isRotate", true);
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _coroutine = SetScoreDefer(specifiedScore, isMagnet);
        StartCoroutine(_coroutine);
    }

    private IEnumerator SetScoreDefer(int specifiedScore = 0, bool isMagnet = false) {
        yield return new WaitForSeconds(_rotateTime);
        if (isMagnet) {
            bool isSuccess = specifiedScore == _popupMagnet.SelectedScore;
            string text1 = isSuccess ? Utils.Wrap("Магнит сработал! ", UIColors.Green) : Utils.Wrap("Магнит не сработал! ", UIColors.Red);
            string text2 = "Загадано: <b>" + _popupMagnet.SelectedScore + "</b> Выпало: <b>" + specifiedScore + "</b>";
            Messages.Instance.AddMessage(text1 + text2);
        }
        SetScore(specifiedScore);
    }

    private void SetScore(int specifiedScore = 0) {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;

        int score;

        if (specifiedScore != 0) {
            score = specifiedScore;
        } else {
            int max = player.GetCubicMaxScore() + 1;
            System.Random random = new();
            score = random.Next(1, max);
        }

        int scoreToDisplay = score;
        score -= player.StuckAttached;

        int multiplier = 1;
        if (player.Effects.IsLightning && !player.Boosters.IsBlot()) {
            multiplier = 2;
        }

        _finalScore = score * multiplier;
        _anim.SetInteger("score", Math.Abs(scoreToDisplay));
        _anim.SetBool("isRotate", false);
        _finalScoreDisplayText.text = _finalScore.ToString();
        StartCoroutine(ShowFinalScoreDisplayDefer());
        _coroutine = MakeMoveDefer();
        StartCoroutine(_coroutine);
    }

    private IEnumerator MakeMoveDefer() {
        yield return new WaitForSeconds(_holdTime);
        MoveControl.Instance.MakeMove(_finalScore);
    }

    public void SetCubicInteractable(bool value) {
        _cubicButton.interactable = value;
        _cursorManager.Disabled = !value;
        _border.SetActive(!value);
        _borderSelect.SetActive(value);
        SpriteRenderer borderSelectSprite = _borderSelect.GetComponent<SpriteRenderer>();
        if (value) {
            _finalScoreDisplay.SetActive(false);
            _coroutinePulse = Utils.StartPulse(borderSelectSprite, _pulseTime, _pulseMinAlpha);
            StartCoroutine(_coroutinePulse);
        } else if (_coroutinePulse != null) {
            StopCoroutine(_coroutinePulse);
            SpriteRenderer sprite = borderSelectSprite;
            sprite.color = new Color(1f, 1f, 1f, 1f);
        }

        string message;

        if (value) {
            message = Utils.Wrap("ваш ход!", UIColors.Green);
        } else {
            message = "";
        }

        WriteStatus(message);
    }

    public void WriteStatus(string text) {
        _statusText.text = text;
    }

    private IEnumerator ShowFinalScoreDisplayDefer() {
        yield return new WaitForSeconds(_showFinalScoreDelay);

        Color32 color = new(255,255,255,255);
        if (_finalScore > 0) {
            color = new(51,179,107,255);
        } else if (_finalScore < 0) {
            color = new(255,68,68,255);
        }

        Color32 textColor = _finalScore == 0 ? new(0,0,0,255) : new(255,255,255,255);
        _finalScoreDisplay.GetComponent<Image>().color = color;
        _finalScoreDisplayText.color = textColor;
        
        _finalScoreDisplay.SetActive(true);
    }

    public void HideFinalScoreDisplay() {
        _finalScoreDisplay.SetActive(false);
    }
}

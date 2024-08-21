using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubicControl : MonoBehaviour
{
    private Button _cubicButton;
    private Animator _anim;
    private IEnumerator _coroutine, _coroutinePulse;
    private TextMeshProUGUI _statusText;
    [SerializeField] private float rotateTime = 1.4f;
    [SerializeField] private float holdTime = 1f;
    [SerializeField] private float pulseTime = 0.5f;
    [SerializeField] private float pulseMinAlpha = 0.2f;
    private int finalScore;
    private GameObject _border, _borderSelect;
    private PopupMagnet _popupMagnet;

    private void Awake() {
        _anim = transform.Find("CubicImage").GetComponent<Animator>();
        _cubicButton = transform.Find("CubicButton").GetComponent<Button>();
        _statusText = transform.Find("StatusText").GetComponent<TextMeshProUGUI>();
        _statusText.text = "";
        _border = transform.Find("cubic_border").gameObject;
        _borderSelect = transform.Find("cubic_border_select").gameObject;
        _popupMagnet = GameObject.Find("GameScripts").GetComponent<PopupMagnet>();
    }

    private void Start() {
        _border.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Space) && _cubicButton.interactable) {
            Throw();
        }
    }

    private void SetScore(int specifiedScore = 0) {
        int score;
        if (specifiedScore != 0) {
            score = specifiedScore;
        } else {
            int max = MoveControl.Instance.CurrentPlayer.CubicMaxScore + 1;
            System.Random random = new();
            score = random.Next(1, max);
        }
        finalScore = score;
        _anim.SetInteger("score", finalScore);
        _anim.SetBool("isRotate", false);
        _coroutine = MakeMoveDefer();
        StartCoroutine(_coroutine);
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
        _statusText.text = "";
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _anim.SetInteger("score", 0);
        _anim.SetBool("isRotate", true);
        _coroutine = SetScoreDefer(specifiedScore, isMagnet);
        StartCoroutine(_coroutine);
    }

    private IEnumerator SetScoreDefer(int specifiedScore = 0, bool isMagnet = false) {
        yield return new WaitForSeconds(rotateTime);
        if (isMagnet) {
            bool isSuccess = specifiedScore == _popupMagnet.SelectedScore;
            string text1 = isSuccess ? Utils.Wrap("Магнит сработал! ", UIColors.Green) : Utils.Wrap("Магнит не сработал! ", UIColors.Red);
            string text2 = "Загадано: <b>" + _popupMagnet.SelectedScore + "</b> Выпало: <b>" + specifiedScore + "</b>";
            Messages.Instance.AddMessage(text1 + text2);
        }
        SetScore(specifiedScore);
    }

    private IEnumerator MakeMoveDefer() {
        yield return new WaitForSeconds(holdTime);
        MoveControl.Instance.MakeMove(finalScore);
    }

    public void SetCubicInteractable(bool value) {
        _cubicButton.interactable = value;
        _border.SetActive(!value);
        _borderSelect.SetActive(value);
        SpriteRenderer borderSelectSprite = _borderSelect.GetComponent<SpriteRenderer>();
        if (value) {
            _coroutinePulse = Utils.StartPulse(borderSelectSprite, pulseTime, pulseMinAlpha);
            StartCoroutine(_coroutinePulse);
        } else if (_coroutinePulse != null) {
            StopCoroutine(_coroutinePulse);
            SpriteRenderer sprite = borderSelectSprite;
            sprite.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void WriteStatus(string text) {
        _statusText.text = text;
    }
}

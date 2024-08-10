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
    [SerializeField] private int overrideScore = 0;
    [SerializeField] private float rotateTime = 1.4f;
    [SerializeField] private float holdTime = 1f;
    [SerializeField] private float pulseTime = 0.5f;
    [SerializeField] private float pulseMinAlpha = 0.2f;
    private int finalScore;
    private GameObject _border, _borderSelect;


    private void Awake() {
        _anim = transform.Find("CubicImage").GetComponent<Animator>();
        _cubicButton = transform.Find("CubicButton").GetComponent<Button>();
        _statusText = transform.Find("StatusText").GetComponent<TextMeshProUGUI>();
        _statusText.text = "";
        _border = transform.Find("cubic_border").gameObject;
        _borderSelect = transform.Find("cubic_border_select").gameObject;
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
        } else if (overrideScore != 0) {
            score = overrideScore;
        } else {
            System.Random random = new();
            score = random.Next(1,7);
        }
        finalScore = score;
        _anim.SetInteger("score", finalScore);
        _anim.SetBool("isRotate", false);
        _coroutine = MakeMoveDefer();
        StartCoroutine(_coroutine);
    }

    // specifiedScore - указать на выброс определенного числа, без рандома
    // overrideScore - это число, которое указывается в редакторе Unity
    // specifiedScore имеет приоритет над overrideScore
    // finalScore - сохранение числа перемещений фишки после всех вычислений

    public void Throw(int specifiedScore = 0) {
        SetCubicInteractable(false);
        EffectsControl.Instance.SetDisabledEffectButtons(true);
        _statusText.text = "";
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _anim.SetInteger("score", 0);
        _anim.SetBool("isRotate", true);
        _coroutine = SetScoreDefer(specifiedScore);
        StartCoroutine(_coroutine);
    }

    private IEnumerator SetScoreDefer(int specifiedScore = 0) {
        yield return new WaitForSeconds(rotateTime);
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

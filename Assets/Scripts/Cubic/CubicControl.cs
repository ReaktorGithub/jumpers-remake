using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubicControl : MonoBehaviour
{
    private Button _cubicButton;
    private Animator _anim;
    private MoveControl _moveControl;
    private IEnumerator _coroutine, _coroutinePulse;
    private TextMeshProUGUI _statusText;
    [SerializeField] private int overrideScore = 0;
    [SerializeField] private float rotateTime = 1.4f;
    [SerializeField] private float holdTime = 1f;
    [SerializeField] private float pulseTime = 0.5f;
    private int finalScore;
    private GameObject _border, _borderSelect;


    private void Awake() {
        _anim = transform.Find("CubicImage").GetComponent<Animator>();
        _cubicButton = transform.Find("CubicButton").GetComponent<Button>();
        _moveControl = GameObject.Find("GameScripts").GetComponent<MoveControl>();
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

    private void SetScore() {
        int score;
        if (overrideScore != 0) {
            score = overrideScore;
        } else {
            System.Random random = new();
            score = random.Next(1,7);
        }
        finalScore = score;
        _anim.SetInteger("score", finalScore);
        _anim.SetBool("isRotate", false);
        Debug.Log("finalScore: " + finalScore);
        _coroutine = MakeMoveDefer();
        StartCoroutine(_coroutine);
    }

    public void Throw() {
        SetCubicInteractable(false);
        _statusText.text = "";
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _anim.SetInteger("score", 0);
        _anim.SetBool("isRotate", true);
        _coroutine = SetScoreDefer();
        StartCoroutine(_coroutine);
    }

    private IEnumerator SetScoreDefer() {
        yield return new WaitForSeconds(rotateTime);
        SetScore();
    }

    private IEnumerator MakeMoveDefer() {
        yield return new WaitForSeconds(holdTime);
        _moveControl.MakeMove(finalScore);
    }

    public void SetCubicInteractable(bool value) {
        _cubicButton.interactable = value;
        _border.SetActive(!value);
        _borderSelect.SetActive(value);
        if (value) {
            _coroutinePulse = StartPulse();
            StartCoroutine(_coroutinePulse);
        } else if (_coroutinePulse != null) {
            StopCoroutine(_coroutinePulse);
            SpriteRenderer sprite = _borderSelect.GetComponent<SpriteRenderer>();
            sprite.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void WriteStatus(string text) {
        _statusText.text = text;
    }

    private IEnumerator StartPulse() {
        float alpha = 0.2f;
        bool isIn = true;
        SpriteRenderer sprite = _borderSelect.GetComponent<SpriteRenderer>();
        
        while (true) {
            while (isIn && alpha < 1f) {
                alpha += Time.deltaTime * pulseTime;
                sprite.color = new Color(1f, 1f, 1f, alpha);
                yield return null;
            }
            isIn = false;
            while (!isIn && alpha > 0.2f) {
                alpha -= Time.deltaTime * pulseTime;
                sprite.color = new Color(1f, 1f, 1f, alpha / pulseTime);
                yield return null;
            }
            isIn = true;
        }
    }
}

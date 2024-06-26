using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CubicControl : MonoBehaviour
{
    private Button _cubicButton;
    private Animator _anim;
    private MoveControl _moveControl;
    private IEnumerator _coroutine;
    [SerializeField] private int overrideScore = 0;
    [SerializeField] private float rotateTime = 1.4f;
    [SerializeField] private float holdTime = 1f;
    private int finalScore;

    private void Awake() {
        _anim = GetComponent<Animator>();
        _cubicButton = GameObject.Find("CubicButton").GetComponent<Button>();
        _moveControl = GameObject.Find("GameScripts").GetComponent<MoveControl>();
    }

    private void FixedUpdate() {
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
    }
}

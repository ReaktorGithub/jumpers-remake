using System.Collections;
using TMPro;
using UnityEngine;

public class ModalHedgehogFinish : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _windowControl;
    private IEnumerator _coroutine;
    [SerializeField] private GameObject _lowPower, _payButton;
    [SerializeField] private TextMeshProUGUI _fightCostText, _payCostText;
    [SerializeField] private int _fightCost = 3;
    [SerializeField] private float _clickDelay = 0.5f;
    private LevelData _levelData;
    private FinishCell _finishCell;
    private HedgehogAnswerButton _payButtonScript;
    private PlayerControl _currentPlayer;

    private void Awake() {
        _modal = GameObject.Find("ModalHedgehogFinish");
        _windowControl = _modal.transform.Find("WindowHedgehogFinish").GetComponent<ModalWindow>();
        _levelData = GameObject.Find("LevelScripts").GetComponent<LevelData>();
        _payButtonScript = _payButton.GetComponent<HedgehogAnswerButton>();
    }

    private void Start() {
        _lowPower.SetActive(false);
        _modal.SetActive(false);
    }

    public void OpenWindow() {
        if (!_modal.activeInHierarchy) {
            _modal.SetActive(true);
            _coroutine = _windowControl.FadeIn();
            StartCoroutine(_coroutine);
        }
    }

    public void CloseWindow() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _modal.SetActive(false);
        _windowControl.ResetScale();
    }

    public void BuildContent(PlayerControl currentPlayer, FinishCell finishCell) {
        _currentPlayer = currentPlayer;
        _finishCell = finishCell;
        _fightCostText.text = _fightCost.ToString();
        bool isLowPower = currentPlayer.Power < _fightCost;
        _lowPower.SetActive(isLowPower);
        _payButtonScript.Disabled = isLowPower;
        _payCostText.text = _levelData.PrizeCoins[3].ToString();
    }

    public void OnPay() {
        StartCoroutine(OnPayDefer());
    }

    public void OnFight() {
        StartCoroutine(OnFightDefer());
    }

    private IEnumerator OnPayDefer() {
        yield return new WaitForSeconds(_clickDelay);
        int cost = _levelData.PrizeCoins[3];
        _finishCell.AddCoinsCollected(cost);
        _currentPlayer.Effects.ExecuteHedgehogFinishPay(cost);
    }

    private IEnumerator OnFightDefer() {
        yield return new WaitForSeconds(_clickDelay);
        _currentPlayer.Effects.ExecuteHedgehogFinishFight();
        _finishCell.BoostersCollected.Clear();
        _finishCell.CoinsCollected = 0;
        _finishCell.IsHedgehog = false;
    }
}

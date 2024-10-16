using System.Collections;
using TMPro;
using UnityEngine;

public class ModalHedgehogFinish : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _lowPower, _payButton;
    [SerializeField] private TextMeshProUGUI _fightCostText, _payCostText;
    [SerializeField] private int _fightCost = 3;
    [SerializeField] private float _clickDelay = 0.5f;
    private LevelData _levelData;
    private BigAnswerButton _payButtonScript;

    private void Awake() {
        _modal = GameObject.Find("ModalHedgehogFinish").GetComponent<Modal>();
        _levelData = GameObject.Find("LevelScripts").GetComponent<LevelData>();
        _payButtonScript = _payButton.GetComponent<BigAnswerButton>();
    }

    private void Start() {
        _lowPower.SetActive(false);
    }

    public void BuildContent() {
        _fightCostText.text = _fightCost.ToString();
        bool isLowPower = MoveControl.Instance.CurrentPlayer.Power < _fightCost;
        _lowPower.SetActive(isLowPower);
        _payButtonScript.Disabled = isLowPower;
        _payCostText.text = _levelData.PrizeCoins[3].ToString();
    }

    public void OnPayClick() {
        _modal.CloseModal();
        StartCoroutine(OnPayDefer());
    }

    public void OnFightClick() {
        _modal.CloseModal();
        StartCoroutine(OnFightDefer());
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    private IEnumerator OnPayDefer() {
        yield return new WaitForSeconds(_clickDelay);
        int cost = _levelData.PrizeCoins[3];
        MoveControl.Instance.CurrentPlayer.Effects.ExecuteHedgehogFinishPay(cost);
    }

    private IEnumerator OnFightDefer() {
        yield return new WaitForSeconds(_clickDelay);
        MoveControl.Instance.CurrentPlayer.Effects.ExecuteHedgehogFinishFight();
    }
}

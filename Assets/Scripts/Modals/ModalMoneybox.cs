using System.Collections;
using TMPro;
using UnityEngine;

public class ModalMoneybox : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _powerRow, _coinsRow, _rubiesRow;
    [SerializeField] private TextMeshProUGUI _coinsBonusText;
    private PlayerControl _currentPlayer;
    private MoneyboxVault _moneyboxVault;
    [SerializeField] private float _clockDelay = 0.5f;

    private void Awake() {
        _modal = GameObject.Find("ModalMoneybox").GetComponent<Modal>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void BuildContent(PlayerControl currentPlayer) {
        _currentPlayer = currentPlayer;
        _moneyboxVault = _currentPlayer.GetCurrentCell().GetComponent<MoneyboxCell>().MoneyboxVault;
        (int, int, int) bonus = _moneyboxVault.GetBonus();

        _coinsBonusText.text = bonus.Item2.ToString();
        _powerRow.SetActive(bonus.Item1 > 0);
        _coinsRow.SetActive(bonus.Item2 > 0);
        _rubiesRow.SetActive(bonus.Item3 > 0);
    }

    public void OnLeaveMoneybox() {
        _modal.CloseModal();
        StartCoroutine(OnLeaveMoneyboxDefer());
    }

    public void OnSkipMove() {
        _modal.CloseModal();
        StartCoroutine(OnSkipMoveDefer());
    }

    private IEnumerator OnLeaveMoneyboxDefer() {
        yield return new WaitForSeconds(_clockDelay);
        _currentPlayer.Effects.LeaveMoneybox(_moneyboxVault);
    }

    private IEnumerator OnSkipMoveDefer() {
        yield return new WaitForSeconds(_clockDelay);
        _currentPlayer.Effects.ExecuteMoneybox(_moneyboxVault);
    }
}

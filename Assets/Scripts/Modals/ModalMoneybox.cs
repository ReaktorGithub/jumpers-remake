using System.Collections;
using TMPro;
using UnityEngine;

public class ModalMoneybox : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _windowControl;
    private IEnumerator _coroutine;
    [SerializeField] private GameObject _powerRow, _coinsRow, _rubiesRow;
    [SerializeField] private TextMeshProUGUI _coinsBonusText;
    private PlayerControl _currentPlayer;
    private MoneyboxVault _moneyboxVault;
    [SerializeField] private float _clockDelay = 0.5f;

    private void Awake() {
        _modal = GameObject.Find("ModalMoneybox");
        _windowControl = _modal.transform.Find("WindowMoneybox").GetComponent<ModalWindow>();
    }

    private void Start() {
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
        StartCoroutine(OnLeaveMoneyboxDefer());
    }

    public void OnSkipMove() {
        StartCoroutine(OnSkipMoveDefer());
    }

    private IEnumerator OnLeaveMoneyboxDefer() {
        yield return new WaitForSeconds(_clockDelay);
        _currentPlayer.LeaveMoneybox(_moneyboxVault);
    }

    private IEnumerator OnSkipMoveDefer() {
        yield return new WaitForSeconds(_clockDelay);
        _currentPlayer.ExecuteMoneybox(_moneyboxVault);
    }
}

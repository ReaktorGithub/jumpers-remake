using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalLastChance : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private TextMeshProUGUI _coinsPaidText, _powerNeedText;
    [SerializeField] private GameObject _paidButtonObject, _loseButtonObject, _warning;
    [SerializeField] private int _powerPrice = 200;
    private Button _paidButton, _loseButton;
    private int _powerNeed = 0;
    private Action _callback; // метод, который надо запустить после нажатия на "заплатить"
    [SerializeField] private float _activateButtonsDelay = 1.5f;

    private void Awake() {
        _modal = GameObject.Find("ModalLastChance").GetComponent<Modal>();
        _paidButton = _paidButtonObject.GetComponent<Button>();
        _loseButton = _loseButtonObject.GetComponent<Button>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void CloseModal() {
        _modal.CloseModal();
    }

    public void BuildContent(PlayerControl player, Action callback = null) {
        _callback = callback;
        _powerNeed = Math.Abs(player.Power);
        int invoice = _powerPrice * _powerNeed;
        bool isEnough = player.Coins >= invoice;
        _warning.SetActive(!isEnough);
        _powerNeedText.text = _powerNeed.ToString();
        _coinsPaidText.text = invoice.ToString();

        SetButtonInteractable(_paidButton, false);
        SetButtonInteractable(_loseButton, false);
        StartCoroutine(ScheduleButtonsActivate(player));
    }

    private void SetButtonInteractable(Button button, bool value) {
        button.interactable = value;
        button.GetComponent<CursorManager>().Disabled = !value;
    }

    private IEnumerator ScheduleButtonsActivate(PlayerControl player) {
        yield return new WaitForSeconds(_activateButtonsDelay);
        SetButtonInteractable(_loseButton, true);
        SetButtonInteractable(_paidButton, player.Coins >= _powerPrice * _powerNeed);
    }

    public void OnAdmitLose() {
        MoveControl.Instance.CurrentPlayer.ConfirmLose();
    }

    public void OnPayCoins() {
        StartCoroutine(MoveControl.Instance.CurrentPlayer.ExecuteLastChanceDefer(_powerPrice, _callback));
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalLastChance : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private TextMeshProUGUI _coinsTotalText, _coinsPaidText;
    [SerializeField] private GameObject _counterObject, _paidButtonObject, _loseButtonObject;
    [SerializeField] private int _powerCost = 200;
    private Counter _counter;
    private Button _paidButton, _loseButton;
    [SerializeField] private float _activateButtonsDelay = 1.5f;

    private void Awake() {
        _modal = GameObject.Find("ModalLastChance").GetComponent<Modal>();
        _paidButton = _paidButtonObject.GetComponent<Button>();
        _loseButton = _loseButtonObject.GetComponent<Button>();
        _counter = _counterObject.GetComponent<Counter>();
    }

    public void OpenModal() {
        BuildContent();
        _modal.OpenModal();
    }

    public void CloseModal() {
        _modal.CloseModal();
    }

    public void BuildContent() {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        Sprite sprite = Manual.Instance.Power.Sprite;
        int maxCount = Math.Abs(player.Power);
        _counter.Init(sprite, 0, 0, maxCount);
        UpdateContent();
        SetButtonInteractable(_paidButton, false);
        SetButtonInteractable(_loseButton, false);
        StartCoroutine(ScheduleButtonsActivate());
    }

    public void OnIncreaseButtonClick() {
        _counter.OnIncrease();
        UpdateContent();
    }

    public void OnDiscreaseButtonClick() {
        _counter.OnDiscrease();
        UpdateContent();
    }

    private void UpdateContent() {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        int power = _counter.Count;
        int paid = _powerCost * power;
        int total = player.Coins - paid;

        _coinsPaidText.text = paid.ToString();
        _coinsTotalText.text = total.ToString();

        SetButtonInteractable(_paidButton, power > 0);
    }

    private void SetButtonInteractable(Button button, bool value) {
        button.interactable = value;
        button.GetComponent<CursorManager>().Disabled = !value;
    }

    private IEnumerator ScheduleButtonsActivate() {
        yield return new WaitForSeconds(_activateButtonsDelay);
        SetButtonInteractable(_loseButton, true);
        SetButtonInteractable(_paidButton, _counter.Count > 0);
    }
}

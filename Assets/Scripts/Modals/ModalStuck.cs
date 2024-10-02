using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalStuck : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private TextMeshProUGUI _coinsTotalText, _rubiesTotalText;
    [SerializeField] private GameObject _counterObject, _warningObject, _rubiesInfoRow, _rubiesTotalRow, _confirmButtonObject;
    [SerializeField] private int _coinsCost = 300, _rubiesCost = 1;
    private Button _confirmButton;
    private Sprite _counterSprite;
    private Counter _counter;
    private int _stuckCount = 0;
    private int _coinsTotal = 0;
    private int _rubiesTotal = 0;
    private PlayerControl _player;

    private void Awake() {
        GameObject instances = GameObject.Find("Instances");
        _counterSprite = instances.transform.Find("stuck-icon").GetComponent<SpriteRenderer>().sprite;
        _counter = _counterObject.GetComponent<Counter>();
        _confirmButton = _confirmButtonObject.GetComponent<Button>();
        _modal = GameObject.Find("ModalStuck").GetComponent<Modal>();
    }

    public void OpenModal() {
        BuildContent(PlayersControl.Instance.GetMe());
        _modal.OpenModal();
    }

    public void ConfirmRemoveStuck() {
        _modal.CloseModal();
        _player.ExecuteRemoveStuck(_stuckCount, _coinsTotal, _rubiesTotal);
    }

    private void BuildContent(PlayerControl player) {
        _player = player;
        _stuckCount = 0;
        _counter.Init(_counterSprite, _stuckCount, 0, _player.StuckAttached);
        UpdateContent();
    }

    public void OnIncreaseButtonClick() {
        int newValue = _counter.OnIncrease();
        _stuckCount = newValue;
        UpdateContent();
    }

    public void OnDiscreaseButtonClick() {
        int newValue = _counter.OnDiscrease();
        _stuckCount = newValue;
        UpdateContent();
    }

    private void UpdateContent() {
        int newCoinsTotal = _coinsCost * _stuckCount;
        int newRubiesTotal = _rubiesCost * _rubiesTotal;
        bool lackCoins = _player.Coins < newCoinsTotal;

        if (lackCoins) {
            int coinPortions = (int)Math.Floor((double)_player.Coins / (double)_coinsCost);
            newCoinsTotal = coinPortions * _coinsCost;
            newRubiesTotal = (_stuckCount - coinPortions) * _rubiesCost;
        }

        bool lackResources = lackCoins && _player.Rubies < newRubiesTotal;

        _rubiesInfoRow.SetActive(lackCoins);
        _rubiesTotalRow.SetActive(lackCoins);
        _warningObject.SetActive(lackResources);
        _coinsTotal = newCoinsTotal;
        _rubiesTotal = newRubiesTotal;
        _coinsTotalText.text = "<b>" + newCoinsTotal + "</b>";
        _rubiesTotalText.text = "<b>" + _rubiesTotal + "</b>";
        SetConfirmButtonInteractable(_stuckCount > 0 && !lackResources);
    }

    private void SetConfirmButtonInteractable(bool value) {
        _confirmButton.interactable = value;
        _confirmButton.GetComponent<CursorManager>().Disabled = !value;
    }
}

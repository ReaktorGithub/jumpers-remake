using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalSurprise : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _stopButtonObject, _confirmButtonObject, _rollObject, _resultEffectObject, _resultCoinsObject, _resultGiftObject, _effectIcon, _warningBox, _giftIcon;
    [SerializeField] private TextMeshProUGUI _effectName, _effectDesription, _giftName, _giftDescription, _coinsCount;
    [SerializeField] private float _delayBeforeClick = 1.5f;
    private Button _stopButton;

    private void Awake() {
        _stopButton = _stopButtonObject.GetComponent<Button>();
        _modal = GameObject.Find("ModalSurprise").GetComponent<Modal>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void OnStopRolling() {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;

        _confirmButtonObject.SetActive(true);
        _stopButtonObject.SetActive(false);
        _rollObject.SetActive(false);
        _warningBox.SetActive(false);

        ESurprise type = SurpriseGenerator.GenerateSurpriseType();

        switch(type) {
            case ESurprise.InventoryEffect: {
                EControllableEffects effect = SurpriseGenerator.GenerateSurpriseInventoryEffect();
                _resultGiftObject.SetActive(true);
                int level = player.Grind.GetEffectLevel(effect);
                ManualContent manual = Manual.Instance.GetEffectManual(effect);
                _giftIcon.GetComponent<Image>().sprite = manual.Sprite;
                _giftName.text = manual.GetEntityNameWithLevel(level);
                _giftDescription.text = manual.GetShortDescription(level);
                break;
            }
            case ESurprise.Booster: {
                EBoosters booster = SurpriseGenerator.GenerateSurpriseBooster();
                _resultGiftObject.SetActive(true);
                int level = player.Grind.GetBoosterLevel(booster);
                ManualContent manual = Manual.Instance.GetBoosterManual(booster);
                _giftIcon.GetComponent<Image>().sprite = manual.Sprite;
                _giftName.text = manual.GetEntityNameWithLevel(level);
                _giftDescription.text = manual.GetShortDescription(level);
                break;
            }
            case ESurprise.Bonus: {
                int coins = SurpriseGenerator.GenerateSurpriseCoins(false);
                _resultCoinsObject.SetActive(true);
                (string, Color32) values = Utils.GetTextWithSymbolAndColor(coins);
                _coinsCount.text = values.Item1;
                _coinsCount.color = values.Item2;
                break;
            }
            case ESurprise.Penalty: {
                int coins = SurpriseGenerator.GenerateSurpriseCoins(true);
                _resultCoinsObject.SetActive(true);
                (string, Color32) values = Utils.GetTextWithSymbolAndColor(coins);
                _coinsCount.text = values.Item1;
                _coinsCount.color = values.Item2;
                break;
            }
            case ESurprise.Mallow: {
                _resultGiftObject.SetActive(true);
                ManualContent manual = Manual.Instance.Mallow;
                _giftIcon.GetComponent<Image>().sprite = manual.Sprite;
                _giftName.text = manual.GetEntityName();
                _giftDescription.text = manual.GetShortDescription(1);
                break;
            }
            default: {
                _resultEffectObject.SetActive(true);
                ManualContent manual = Manual.Instance.GetEffectManualBySurprise(type);
                _effectIcon.GetComponent<Image>().sprite = manual.Sprite;
                int level = 0;
                if (type != ESurprise.Teleport && type != ESurprise.Lightning) {
                    level = SurpriseGenerator.GenerateEffectLevel();
                }
                _effectName.text = manual.GetEntityNameWithLevel(level);
                _effectDesription.text = manual.GetShortDescription(level);
                break;
            }
        }
    }

    public void OnConfirmSurprise() {
        _modal.CloseModal();
    }

    public void BuildContent() {
        _confirmButtonObject.SetActive(false);
        _stopButtonObject.SetActive(true);
        _resultEffectObject.SetActive(false);
        _resultCoinsObject.SetActive(false);
        _resultGiftObject.SetActive(false);
        _warningBox.SetActive(true);
        SetStopButtonInteractable(false);
        StartCoroutine(ActivateStopButtonSheduler());
    }

    private void SetStopButtonInteractable(bool value) {
        _stopButton.interactable = value;
        _stopButton.GetComponent<CursorManager>().Disabled = !value;
    }

    private IEnumerator ActivateStopButtonSheduler() {
        yield return new WaitForSeconds(_delayBeforeClick);
        SetStopButtonInteractable(true);
    }
}

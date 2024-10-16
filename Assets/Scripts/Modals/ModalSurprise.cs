using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalSurprise : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _stopButtonObject, _confirmButtonObject, _rollObject, _resultEffectObject, _resultCoinsObject, _resultGiftObject, _warningBox, _giftIcon;
    [SerializeField] private TextMeshProUGUI _effectDesription, _giftName, _giftDescription, _coinsCount;
    [SerializeField] private float _delayBeforeClick = 1.5f;
    private EffectDisplayer _effectDisplayer;
    private Button _stopButton;
    private Action _onConfirm;

    private void Awake() {
        _stopButton = _stopButtonObject.GetComponent<Button>();
        _modal = GameObject.Find("ModalSurprise").GetComponent<Modal>();
        _effectDisplayer = _resultEffectObject.transform.Find("EffectDisplayer").GetComponent<EffectDisplayer>();
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

        (
            ESurprise surpriseType,
            EControllableEffects surpriseEffect,
            EBoosters surpriseBooster,
            int surpriseCoinsBonus,
            int surpriseCoinsPenalty,
            int surpriseLevel
        ) = SurpriseGenerator.GenerateSurprise();

        switch(surpriseType) {
            case ESurprise.InventoryEffect: {
                _resultGiftObject.SetActive(true);
                int level = player.Grind.GetEffectLevel(surpriseEffect);
                ManualContent manual = Manual.Instance.GetEffectManual(surpriseEffect);
                _giftIcon.GetComponent<Image>().sprite = manual.Sprite;
                _giftName.text = manual.GetEntityNameWithLevel(level);
                _giftDescription.text = manual.GetShortDescription(level);
                break;
            }
            case ESurprise.Booster: {
                _resultGiftObject.SetActive(true);
                int level = player.Grind.GetBoosterLevel(surpriseBooster);
                ManualContent manual = Manual.Instance.GetBoosterManual(surpriseBooster);
                _giftIcon.GetComponent<Image>().sprite = manual.Sprite;
                _giftName.text = manual.GetEntityNameWithLevel(level);
                _giftDescription.text = manual.GetShortDescription(level);
                break;
            }
            case ESurprise.Bonus: {
                _resultCoinsObject.SetActive(true);
                (string, Color32) values = Utils.GetTextWithSymbolAndColor(surpriseCoinsBonus);
                _coinsCount.text = values.Item1;
                _coinsCount.color = values.Item2;
                break;
            }
            case ESurprise.Penalty: {
                _resultCoinsObject.SetActive(true);
                (string, Color32) values = Utils.GetTextWithSymbolAndColor(surpriseCoinsPenalty);
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
                ManualContent manual = Manual.Instance.GetEffectManualBySurprise(surpriseType);
                int level = 0;
                if (surpriseType != ESurprise.Teleport && surpriseType != ESurprise.Lightning) {
                    level = surpriseLevel;
                }
                string text = manual.GetEntityNameWithLevel(level);
                _effectDisplayer.BuildContent(text, manual.Sprite, level);
                _effectDesription.text = manual.GetShortDescription(level);
                break;
            }
        }

        _onConfirm = () => {
            player.Effects.ProcessGeneratedSurprise(
                surpriseType,
                surpriseEffect,
                surpriseBooster,
                surpriseCoinsBonus,
                surpriseCoinsPenalty,
                surpriseLevel
            );
        };
    }

    public void OnConfirmSurprise() {
        _modal.CloseModal();
        _onConfirm?.Invoke();
    }

    public void BuildContent() {
        _rollObject.SetActive(true);
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

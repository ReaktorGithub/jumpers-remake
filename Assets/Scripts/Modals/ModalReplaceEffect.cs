using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalReplaceEffect : MonoBehaviour
{
    private GameObject _modal;
    private ModalWindow _windowControl;
    private IEnumerator _coroutine;
    [SerializeField] private GameObject _iconExecute, _iconCostEffect, _iconResource, _lowPower, _answerButton, _shieldInfo;
    [SerializeField] private TextMeshProUGUI _descriptionText, _effectCostName, _resourceCost, _resourceCostText, _effectName;
    private TextMeshProUGUI _lowPowerText;
    private ReplaceAnswerButton _answerButtonScript;

    private void Awake() {
        _modal = GameObject.Find("ModalReplaceEffect");
        _windowControl = _modal.transform.Find("WindowReplaceEffect").GetComponent<ModalWindow>();
        _lowPowerText = _lowPower.GetComponent<TextMeshProUGUI>();
        _answerButtonScript = _answerButton.GetComponent<ReplaceAnswerButton>();
    }

    private void Start() {
        _lowPower.SetActive(false);
        _modal.SetActive(false);

        // EffectsControl.Instance.SelectedEffect = EControllableEffects.Black;

        // if (Manual.Instance.GetEffectCharacter(EffectsControl.Instance.SelectedEffect) == EResourceCharacters.Negative) {
        //     BuildContent(MoveControl.Instance.CurrentPlayer);
        //     OpenWindow();
        // }
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

    private void SetNewEffectIcon(Sprite sprite) {
        _iconExecute.GetComponent<Image>().sprite = sprite;
        _iconCostEffect.GetComponent<Image>().sprite = sprite;
    }

    public void BuildContent(PlayerControl currentPlayer) {
        EControllableEffects effect = EffectsControl.Instance.SelectedEffect;

        // todo уровень эффекта должен вычисляться из PlayerControl
        int effectLevel = 1;
        
        // иконка

        ManualContent manual = Manual.Instance.GetEffectManual(effect);
        SetNewEffectIcon(manual.Sprite);

        // имя эффекта

        _effectName.text = manual.GetEntityNameWithLevel(effectLevel);

        // описание

        _descriptionText.text = manual.GetShortDescription(effectLevel);

        // название эффекта

        _effectCostName.text = manual.GetEntityName(true);

        // ресурс, который тратится для совершения действия

        int cost = manual.GetCost(effectLevel);
        _resourceCost.text = cost.ToString();
        EResourceTypes resourceType = manual.CostResourceType;
        ManualContent resourceManual = Manual.Instance.Power;
        bool isNotEnough;

        if (resourceType == EResourceTypes.Power) {
            // ресурс - это сила
            _lowPowerText.text = "Мало силы";
            isNotEnough = cost > currentPlayer.Power;
        } else {
            // ресурс - это монеты
            resourceManual = Manual.Instance.Coins;
            _lowPowerText.text = "Мало монет";
            isNotEnough = cost > currentPlayer.Coins;
        }

        _lowPower.SetActive(isNotEnough);
        _answerButtonScript.Disabled = isNotEnough;
        _iconResource.GetComponent<Image>().sprite = resourceManual.Sprite;
        _resourceCostText.text = resourceManual.GetEntityName(true);

        // Информация о наличии щита
        bool isPowerDangerEffect = effect == EControllableEffects.Black || effect == EControllableEffects.Red;
        _shieldInfo.SetActive(isPowerDangerEffect && currentPlayer.Armor > 0 && currentPlayer.IsIronArmor);
    }
}

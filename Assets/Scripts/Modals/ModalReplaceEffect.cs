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
    [SerializeField] private GameObject iconExecute, iconCostEffect, iconResource, lowPower, answerButton, shieldInfo;
    [SerializeField] private TextMeshProUGUI descriptionText, effectCostName, resourceCost, resourceCostText, effectName;
    private TextMeshProUGUI _lowPowerText;
    private ReplaceAnswerButton answerButtonScript;

    private void Awake() {
        _modal = GameObject.Find("ModalReplaceEffect");
        _windowControl = _modal.transform.Find("WindowReplaceEffect").GetComponent<ModalWindow>();
        _lowPowerText = lowPower.GetComponent<TextMeshProUGUI>();
        answerButtonScript = answerButton.GetComponent<ReplaceAnswerButton>();
    }

    private void Start() {
        lowPower.SetActive(false);
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
        iconExecute.GetComponent<Image>().sprite = sprite;
        iconCostEffect.GetComponent<Image>().sprite = sprite;
    }

    public void BuildContent(PlayerControl currentPlayer) {
        EControllableEffects effect = EffectsControl.Instance.SelectedEffect;

        // todo уровень эффекта должен вычисляться из PlayerControl
        int effectLevel = 1;
        
        // иконка

        ManualContent manual = Manual.Instance.GetEffectManual(effect);
        SetNewEffectIcon(manual.Sprite);

        // имя эффекта

        effectName.text = manual.GetEntityNameWithLevel(effectLevel);

        // описание

        List<string> textList = manual.GetShortDescription(effectLevel);
        int count = 1;
        string result = "";
        foreach(string text in textList) {
            string myText = textList.Count == count ? text : text + "<br>";
            result += myText;
            count++;
        }
        descriptionText.text = result;

        // название эффекта

        effectCostName.text = manual.GetEntityName(true);

        // ресурс, который тратится для совершения действия

        int cost = manual.GetCostToReplaceEffect(effectLevel);
        resourceCost.text = cost.ToString();
        EResourceTypes resourceType = manual.ReplaceEffectResourceType;
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

        lowPower.SetActive(isNotEnough);
        answerButtonScript.Disabled = isNotEnough;
        iconResource.GetComponent<Image>().sprite = resourceManual.Sprite;
        resourceCostText.text = resourceManual.GetEntityName(true);

        // Информация о наличии щита
        bool isPowerDangerEffect = effect == EControllableEffects.Black || effect == EControllableEffects.Red;
        shieldInfo.SetActive(isPowerDangerEffect && currentPlayer.Armor > 0 && currentPlayer.IsIronArmor);
    }
}

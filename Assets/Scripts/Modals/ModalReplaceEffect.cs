using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalReplaceEffect : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _iconExecute, _iconCostEffect, _iconResource, _lowPower, _replaceButton, _shieldInfo;
    [SerializeField] private TextMeshProUGUI _descriptionText, _effectCostName, _resourceCost, _resourceCostText, _effectName;
    private TextMeshProUGUI _lowPowerText;
    private BigAnswerButton _replaceButtonScript;

    private void Awake() {
        _modal = GameObject.Find("ModalReplaceEffect").GetComponent<Modal>();
        _lowPowerText = _lowPower.GetComponent<TextMeshProUGUI>();
        _replaceButtonScript = _replaceButton.GetComponent<BigAnswerButton>();
    }

    private void Start() {
        _lowPower.SetActive(false);
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    private void SetNewEffectIcon(Sprite sprite) {
        _iconExecute.GetComponent<Image>().sprite = sprite;
        _iconCostEffect.GetComponent<Image>().sprite = sprite;
    }

    public void BuildContent(PlayerControl currentPlayer) {
        EControllableEffects effect = EffectsControl.Instance.SelectedEffect;
        
        CellControl cell = currentPlayer.GetCurrentCell();
        int effectLevel = cell.EffectLevel;
        
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
        _replaceButtonScript.Disabled = isNotEnough;
        _iconResource.GetComponent<Image>().sprite = resourceManual.Sprite;
        _resourceCostText.text = resourceManual.GetEntityName(true);

        // Информация о наличии щита
        bool isPowerDangerEffect = effect == EControllableEffects.Black || effect == EControllableEffects.Red;
        _shieldInfo.SetActive(isPowerDangerEffect && currentPlayer.Boosters.Armor > 0 && currentPlayer.Boosters.IsIronArmor);
    }

    public void OnConfirmClick() {
        _modal.CloseModal();
        MoveControl.Instance.CheckCellEffects();
    }

    public void OnReplaceClick() {
        _modal.CloseModal();
        EffectsControl.Instance.ActivateSelectionMode(true);
    }
}

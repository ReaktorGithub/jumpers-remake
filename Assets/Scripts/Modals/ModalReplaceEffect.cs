using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModalReplaceEffect : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _effectDisplayerObject, _iconCostEffect, _iconResource, _lowPower, _replaceButton, _shieldInfo, _shieldIcon;
    [SerializeField] private TextMeshProUGUI _descriptionText, _effectCostName, _resourceCost, _resourceCostText;
    private EffectDisplayer _effectDisplayer;
    private TextMeshProUGUI _lowPowerText;
    private BigAnswerButton _replaceButtonScript;
    private Sprite _shieldSprite, _starSprite;
    private Image _shieldImage;

    private void Awake() {
        GameObject Instances = GameObject.Find("Instances");
        _modal = GameObject.Find("ModalReplaceEffect").GetComponent<Modal>();
        _lowPowerText = _lowPower.GetComponent<TextMeshProUGUI>();
        _replaceButtonScript = _replaceButton.GetComponent<BigAnswerButton>();
        _shieldSprite = Instances.transform.Find("shield-iron").GetComponent<SpriteRenderer>().sprite;
        _starSprite = Instances.transform.Find("star-icon").GetComponent<SpriteRenderer>().sprite;
        _shieldImage = _shieldIcon.GetComponent<Image>();
        _effectDisplayer = _effectDisplayerObject.GetComponent<EffectDisplayer>();
    }

    private void Start() {
        _lowPower.SetActive(false);
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    private void SetNewEffectIcon(Sprite sprite) {
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

        string text = manual.GetEntityNameWithLevel(effectLevel);
        _effectDisplayer.BuildContent(text, manual.Sprite, effectLevel);

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

        // Информация о защите
        bool showStar = effect == EControllableEffects.Black && currentPlayer.IsLuckyStar;
        bool isPowerDangerEffect = effect == EControllableEffects.Black || effect == EControllableEffects.Red;
        bool showShield = isPowerDangerEffect && currentPlayer.Boosters.Armor > 0 && currentPlayer.Boosters.IsIronArmor;
        if (showStar) {
            _shieldImage.sprite = _starSprite;
        } else if (showShield) {
            _shieldImage.sprite = _shieldSprite;
        }
        _shieldInfo.SetActive(showStar || showShield);
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

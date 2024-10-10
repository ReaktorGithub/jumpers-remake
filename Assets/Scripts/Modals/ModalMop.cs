using UnityEngine;
using UnityEngine.UI;

public class ModalMop : MonoBehaviour
{
    private Modal _modal;
    [SerializeField] private GameObject _confirmButtonObject, _optionButtonEffectObject, _optionButtonTrapObject, _optionButtonBoombasterObject;
    private Button _optionButtonEffect, _optionButtonTrap, _optionButtonBoombaster;
    private TokenAttackButton _optionButtonEffectScript, _optionButtonTrapScript, _optionButtonBoombasterScript;
    private Button _confirmButton;
    private EMopOptions _selectedOption = EMopOptions.None;
    private CellControl _selectedCell;

    private void Awake() {
        // GameObject instances = GameObject.Find("Instances");
        _confirmButton = _confirmButtonObject.GetComponent<Button>();
        _modal = GameObject.Find("ModalMop").GetComponent<Modal>();
        _optionButtonEffectScript = _optionButtonEffectObject.GetComponent<TokenAttackButton>();
        _optionButtonTrapScript = _optionButtonTrapObject.GetComponent<TokenAttackButton>();
        _optionButtonBoombasterScript = _optionButtonBoombasterObject.GetComponent<TokenAttackButton>();
        _optionButtonEffect = _optionButtonEffectObject.GetComponent<Button>();
        _optionButtonTrap = _optionButtonTrapObject.GetComponent<Button>();
        _optionButtonBoombaster = _optionButtonBoombasterObject.GetComponent<Button>();
    }

    public void OpenModal() {
        _modal.OpenModal();
    }

    public void ConfirmMop() {
        _modal.CloseModal();
        MoveControl.Instance.CurrentPlayer.Boosters.ExecuteMop(_selectedCell, _selectedOption);
    }

    public void CancelMop() {
        _modal.CloseModal();
        BoostersControl.Instance.ExitMopMode();
        BoostersControl.Instance.ExitMopModePhase2();
    }

    public void BuildContent(CellControl selectedCell) {
        _selectedCell = selectedCell;
        _selectedOption = EMopOptions.None;
        _confirmButton.interactable = false;
        _optionButtonEffectScript.SetSelected(false);
        _optionButtonTrapScript.SetSelected(false);
        _optionButtonBoombasterScript.SetSelected(false);

        bool showEffect = selectedCell.Effect != EControllableEffects.None;
        bool showTrap = selectedCell.WhosTrap != null;
        bool showBoombaster = selectedCell.IsBoombaster;

        if (showEffect) {
            Sprite sprite = EffectsControl.Instance.GetBrushSprite(selectedCell.Effect);
            _optionButtonEffectScript.SetTokenImage(sprite);
            _optionButtonEffect.onClick.RemoveAllListeners();
            _optionButtonEffect.onClick.AddListener(() => {
                OnOptionClick(EMopOptions.Effect);
            });
        }

        if (showTrap) {
            _optionButtonTrap.onClick.RemoveAllListeners();
            _optionButtonTrap.onClick.AddListener(() => {
                OnOptionClick(EMopOptions.Trap);
            });
        }

        if (showBoombaster) {
            _optionButtonBoombaster.onClick.RemoveAllListeners();
            _optionButtonBoombaster.onClick.AddListener(() => {
                OnOptionClick(EMopOptions.Boombaster);
            });
        }

        _optionButtonEffectObject.SetActive(showEffect);
        _optionButtonTrapObject.SetActive(showTrap);
        _optionButtonBoombasterObject.SetActive(showBoombaster);
    }

    public void OnOptionClick(EMopOptions option) {
        _selectedOption = option;
        _confirmButton.interactable = true;
        UpdateOptionsButtonSelection();
    }

    private void UpdateOptionsButtonSelection() {
        _optionButtonEffectScript.SetSelected(_selectedOption == EMopOptions.Effect);
        _optionButtonTrapScript.SetSelected(_selectedOption == EMopOptions.Trap);
        _optionButtonBoombasterScript.SetSelected(_selectedOption == EMopOptions.Boombaster);
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowAttackContent : MonoBehaviour
{
    private GameObject _optionalSectionTokens;
    private TextMeshProUGUI _tokenName, _attackHeading, _attackDescription, _powerNow, _powerLeft, _warningText;
    private List<TokenAttackButton> _tokenAttackButtons = new();
    private List<AttackTypeButton> _attackTypeButtons = new();
    private PlayerControl _selectedPlayer = null;
    private EAttackTypes _selectedAttackType = EAttackTypes.Usual;
    private Button _buttonAttack, _buttonCancel;
    private int _powerInitial = 0;
    private int _powerNeed = 0;

    private void Awake() {
        _optionalSectionTokens = transform.Find("OptionalSectionTokens").gameObject;
        _tokenName = Utils.FindChildByName(_optionalSectionTokens, "TokenName").GetComponent<TextMeshProUGUI>();
        TokenAttackButton[] allButtons = _optionalSectionTokens.GetComponentsInChildren<TokenAttackButton>();
        foreach (TokenAttackButton button in allButtons) {
            _tokenAttackButtons.Add(button);
        }
        AttackTypeButton[] allAttackButtons = Utils.FindChildByName(transform.gameObject, "AttackList").GetComponentsInChildren<AttackTypeButton>();
        foreach (AttackTypeButton button in allAttackButtons) {
            button.GetComponent<Button>().onClick.AddListener(() => {
                SetSelectedAttackType(button.AttackType);
            });
            _attackTypeButtons.Add(button);
        }
        _attackHeading = Utils.FindChildByName(transform.gameObject, "HeadText").transform.GetComponent<TextMeshProUGUI>();
        _attackDescription = Utils.FindChildByName(transform.gameObject, "BodyText").transform.GetComponent<TextMeshProUGUI>();
        _buttonAttack = Utils.FindChildByName(transform.gameObject, "ButtonOk").GetComponent<Button>();
        _buttonCancel = Utils.FindChildByName(transform.gameObject, "ButtonCancel").GetComponent<Button>();
        _powerNow = Utils.FindChildByName(transform.gameObject, "PowerNow").GetComponent<TextMeshProUGUI>();
        _powerLeft = Utils.FindChildByName(transform.gameObject, "PowerLeft").GetComponent<TextMeshProUGUI>();
        _warningText = Utils.FindChildByName(transform.gameObject, "WarningText").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        UpdateAttackTypeSelection();
        SetButtonsInteractable(true);
    }

    public void BuildContent(List<PlayerControl> rivals, PlayerControl currentPlayer) {
        // раздел с атаками

        foreach(AttackTypeButton button in _attackTypeButtons) {
            if (currentPlayer.AvailableAttackTypes.Contains(button.AttackType)) {
                button.SetAsEnabled();
            } else {
                button.SetAsDisabled();
            }
        }

        // сила

        _powerInitial = currentPlayer.Power;
        UpdatePower();

        // раздел с соперниками

        if (rivals.Count < 2) {
            _optionalSectionTokens.SetActive(false);
            _selectedPlayer = rivals[0];
        } else {
            _optionalSectionTokens.SetActive(true);
            _tokenName.text = "";
            int index = 0;
            foreach(TokenAttackButton button in _tokenAttackButtons) {
                if (index < rivals.Count) {
                    Sprite sprite = rivals[index].GetTokenSprite();
                    button.SetTokenImage(sprite);
                    button.BindPlayer(rivals[index]);
                    Button buttonComponent = button.gameObject.GetComponent<Button>();
                    buttonComponent.onClick.AddListener(() => {
                        SetSelectedPlayer(button.Player);
                    });
                    button.gameObject.SetActive(true);
                } else {
                    button.gameObject.SetActive(false);
                }
                index++;
            }
        }

        // кнопка атаки

        UpdateAttackButtonStatus();
    }

    public void SetSelectedPlayer(PlayerControl player) {
        if (_selectedPlayer == player) {
            _selectedPlayer = null;
            _tokenName.text = "";
            UpdatePlayerSelection();
            return;
        }

        _selectedPlayer = player;
        _tokenName.text = player.PlayerName;
        UpdatePlayerSelection();
    }

    private void UpdatePlayerSelection() {
        foreach(TokenAttackButton button in _tokenAttackButtons) {
            button.SetSelected(button.Player == _selectedPlayer);
        }
        UpdateAttackButtonStatus();
    }

    public void SetSelectedAttackType(EAttackTypes type) {
        _selectedAttackType = type;
        UpdateAttackTypeSelection();
    }

    private void UpdateAttackTypeSelection() {
        string[] aboutText = GetSelectedAttackDescription();
        _attackHeading.text = aboutText[0];
        _attackDescription.text = aboutText[1];
        foreach(AttackTypeButton button in _attackTypeButtons) {
            button.SetSelected(button.AttackType == _selectedAttackType);
        }
        UpdatePower();
    }

    private string[] GetSelectedAttackDescription() {
        string[] result = new string[2];

        switch (_selectedAttackType) {
            case EAttackTypes.Usual:
            result[0] = "Обычная атака";
            result[1] = "Атакуйте соперника ценой 1 силы и ходите еще раз.<br><br>Соперник пропустит ход.";
            break;
            case EAttackTypes.MagicKick:
            result[0] = "Волшебный пинок (ур.1)";
            result[1] = "Атакуйте соперника ценой 1 силы.<br><br>Он откатится на 2 клетки назад.";
            break;
            case EAttackTypes.Vampyre:
            result[0] = "Вампирские клыки";
            result[1] = "Соперник пропустит ход, вы ходите ещё раз.<br><br>Ваша сила увеличится на 1, а у соперника уменьшится на 1.";
            break;
            case EAttackTypes.Knockout:
            result[0] = "Нокаут (ур.1)";
            result[1] = "Выбросьте соперника с трассы ценой 5 силы и ходите еще раз.<br><br>Жертва займет последнее возможное место.";
            break;
            default:
            result[0] = "";
            result[1] = "";
            break;
        }

        return result;
    }

    public void ResetContent() {
        SetSelectedAttackType(EAttackTypes.Usual);
        _selectedPlayer = null;
        _tokenName.text = "";
        foreach(TokenAttackButton button in _tokenAttackButtons) {
            button.SetSelected(false);
        }
    }

    public void SetButtonsInteractable(bool value) {
        if (!value) {
            foreach(AttackTypeButton button in _attackTypeButtons) {
                button.SetAsDisabled();
            }
        }
        foreach(TokenAttackButton tokenButton in _tokenAttackButtons) {
            tokenButton.GetComponent<Button>().interactable = value;
        }
        _buttonAttack.interactable = value;
        _buttonCancel.interactable = value;
    }

    private void UpdatePower() {
        int powerNeed = 0;

        switch (_selectedAttackType) {
            case EAttackTypes.Usual:
            case EAttackTypes.MagicKick:
            powerNeed =+ 1;
            break;
            case EAttackTypes.Vampyre:
            powerNeed =- 1;
            break;
            case EAttackTypes.Knockout:
            powerNeed += 5;
            break;
        }

        int powerLeft = _powerInitial - powerNeed;
        _powerNeed = powerNeed;
        string powerLeftString = powerLeft > 0 ? powerLeft.ToString() : Utils.Wrap(powerLeft.ToString(), UIColors.Red);

        _powerNow.text = "Сил в наличии: <b>" + _powerInitial + "</b>";
        _powerLeft.text = "Сил останется: <b>" + powerLeftString + "</b>";

        UpdateAttackButtonStatus();
    }

    private void UpdateAttackButtonStatus() {
        if (_selectedPlayer == null) {
            _buttonAttack.interactable = false;
            _warningText.text = "Выберите соперника";
        } else if (_powerInitial - _powerNeed < 0) {
            _buttonAttack.interactable = false;
            _warningText.text = "Мало сил";
        } else {
            _buttonAttack.interactable = true;
            _warningText.text = "";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupAttack : MonoBehaviour
{
    private GameObject _attack, _optionalSectionTokens;
    private Popup _popup;
    private TextMeshProUGUI _tokenName, _attackHeading, _attackDescription, _powerNow, _powerLeft, _warningText;
    private List<TokenAttackButton> _tokenAttackButtons = new();
    private List<AttackTypeButton> _attackTypeButtons = new();
    private PlayerControl _selectedPlayer = null;
    private EAttackTypes _selectedAttackType = EAttackTypes.Usual;
    private Button _buttonAttack, _buttonCancel;
    private int _powerInitial = 0;
    private int _powerNeed = 0;
    [SerializeField] private float _attackDelay = 0.7f;

    private void Awake() {
        _attack = GameObject.Find("PopupAttack");
        _popup = _attack.GetComponent<Popup>();
        _optionalSectionTokens = _attack.transform.Find("OptionalSectionTokens").gameObject;
        _tokenName = Utils.FindChildByName(_optionalSectionTokens, "TokenName").GetComponent<TextMeshProUGUI>();
        TokenAttackButton[] allButtons = _optionalSectionTokens.GetComponentsInChildren<TokenAttackButton>();
        foreach (TokenAttackButton button in allButtons) {
            _tokenAttackButtons.Add(button);
        }
        AttackTypeButton[] allAttackButtons = Utils.FindChildByName(_attack, "AttackList").GetComponentsInChildren<AttackTypeButton>();
        foreach (AttackTypeButton button in allAttackButtons) {
            button.GetComponent<Button>().onClick.AddListener(() => {
                SetSelectedAttackType(button.AttackType);
            });
            _attackTypeButtons.Add(button);
        }
        _attackHeading = Utils.FindChildByName(_attack, "HeadText").transform.GetComponent<TextMeshProUGUI>();
        _attackDescription = Utils.FindChildByName(_attack, "BodyText").transform.GetComponent<TextMeshProUGUI>();
        _buttonAttack = Utils.FindChildByName(_attack, "ButtonOk").GetComponent<Button>();
        _buttonCancel = Utils.FindChildByName(_attack, "ButtonCancel").GetComponent<Button>();
        _powerNow = Utils.FindChildByName(_attack, "PowerNow").GetComponent<TextMeshProUGUI>();
        _powerLeft = Utils.FindChildByName(_attack, "PowerLeft").GetComponent<TextMeshProUGUI>();
        _warningText = Utils.FindChildByName(_attack, "WarningText").GetComponent<TextMeshProUGUI>();
    }

    // перед открытием окна сперва запускать BuildContent!

    public void OnOpenWindow() {
        bool opened = _popup.OpenWindow();
        if (opened) {
            SetButtonsInteractable(true);
            UpdateAttackTypeSelection();
        }
    }

    public void OnCloseWindow() {
        bool closed = _popup.CloseWindow();
        if (closed) {
            SetButtonsInteractable(false);
        }
    }

    public void BuildContent(PlayerControl currentPlayer, List<PlayerControl> rivals) {
        SetSelectedAttackType(EAttackTypes.Usual);

        // раздел с атаками

        if (currentPlayer.BoosterVampire > 0) {
            currentPlayer.AddAvailableAttackType(EAttackTypes.Vampire);
        } else {
            currentPlayer.RemoveAvailableAttackType(EAttackTypes.Vampire);
        }

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
            _selectedPlayer = null;
            _tokenName.text = "";
            int index = 0;
            foreach(TokenAttackButton button in _tokenAttackButtons) {
                if (index < rivals.Count) {
                    Sprite sprite = rivals[index].TokenImage;
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
            case EAttackTypes.Usual: {
                result[0] = Manual.Instance.AttackUsual.GetEntityName();
                result[1] = Manual.Instance.AttackUsual.GetShortDescription(1);
                break;
            }
            case EAttackTypes.MagicKick: {
                result[0] = Manual.Instance.AttackMagicKick.GetEntityNameWithLevel(1); // todo добывать уровень из current player
                result[1] = Manual.Instance.AttackMagicKick.GetShortDescription(1);
                break;
            }
            case EAttackTypes.Vampire: {
                result[0] = Manual.Instance.AttackVampire.GetEntityName();
                result[1] = Manual.Instance.AttackVampire.GetShortDescription(1);
                break;
            }
            case EAttackTypes.Knockout: {
                result[0] = Manual.Instance.AttackMagicKick.GetEntityNameWithLevel(1); // todo добывать уровень из current player
                result[1] = Manual.Instance.AttackMagicKick.GetShortDescription(1);
                break;
            }
            default: {
                result[0] = "";
                result[1] = "";
                break;
            }
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

    private void SetButtonAttackInteractable(bool value) {
        _buttonAttack.interactable = value;
        _buttonAttack.GetComponent<CursorManager>().Disabled = !value;
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
        SetButtonAttackInteractable(value);
        _buttonCancel.interactable = value;
    }

    private void UpdatePower() {
        int powerNeed = 0;

        switch (_selectedAttackType) {
            case EAttackTypes.Usual:
            case EAttackTypes.MagicKick:
            powerNeed =+ 1;
            break;
            case EAttackTypes.Vampire:
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
            SetButtonAttackInteractable(false);
            _warningText.text = "Выберите соперника";
        } else if (_powerInitial - _powerNeed < 0) {
            SetButtonAttackInteractable(false);
            _warningText.text = "Мало сил";
        } else {
            SetButtonAttackInteractable(true);
            _warningText.text = "";
        }
    }

    private IEnumerator ConfirmAttackDefer() {
        yield return new WaitForSeconds(_attackDelay);

        switch(_selectedAttackType) {
            case EAttackTypes.MagicKick: {
                MoveControl.Instance.CurrentPlayer.ExecuteAttackMagicKick(_selectedPlayer);
                break;
            }
            case EAttackTypes.Vampire: {
                MoveControl.Instance.CurrentPlayer.ExecuteAttackVampire(_selectedPlayer);
                break;
            }
            default: {
                MoveControl.Instance.CurrentPlayer.ExecuteAttackUsual(_selectedPlayer);
                break;
            }
        }
    }

    private IEnumerator CancelAttackDefer() {
        yield return new WaitForSeconds(_attackDelay);
        ResetContent();
        string message = Utils.Wrap(MoveControl.Instance.CurrentPlayer.PlayerName, UIColors.Yellow) + " отказался от атаки";
        Messages.Instance.AddMessage(message);
        StartCoroutine(MoveControl.Instance.EndMoveDefer());
    }

    public void ConfirmAttack() {
        StartCoroutine(ConfirmAttackDefer());
    }

    public void CancelAttack() {
        StartCoroutine(CancelAttackDefer());
    }
}

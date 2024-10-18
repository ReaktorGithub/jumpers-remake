using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupAttack : MonoBehaviour
{
    private GameObject _attack, _whosAttackObject;
    private Popup _popup;
    private TextMeshProUGUI _tokenName, _attackHeading, _attackDescription, _powerNow, _powerLeft, _warningText, _removeStuckCost;
    private Sprite _counterSprite;
    private List<TokenAttackButton> _tokenAttackButtons = new();
    private List<AttackTypeButton> _attackTypeButtons = new();
    private PlayerControl _selectedPlayer = null;
    private EAttackTypes _selectedAttackType = EAttackTypes.Usual;
    private Button _buttonAttack, _buttonCancel;
    private int _powerNeed = 0;
    [SerializeField] private float _attackDelay = 0.7f;
    [SerializeField] private GameObject _stuckAddButton, _optionalSectionStuckAdd, _optionalSectionStuckRemove, _removeStuckCostObject, _counterObject;
    [SerializeField] private TextMeshProUGUI _stuckTextNormal, _stuckTextLocked;
    private TokenAttackButton _stuckAddButtonScript;
    private Counter _counter;
    private PlayerControl _player;
    private bool _isAddStuck = false;

    private void Awake() {
        GameObject instances = GameObject.Find("Instances");
        _counterSprite = instances.transform.Find("stuck-icon").GetComponent<SpriteRenderer>().sprite;
        _attack = GameObject.Find("PopupAttack");
        _popup = _attack.GetComponent<Popup>();
        GameObject optionalSectionTokens = _attack.transform.Find("OptionalSectionTokens").gameObject;
        _tokenName = Utils.FindChildByName(optionalSectionTokens, "TokenName").GetComponent<TextMeshProUGUI>();
        TokenAttackButton[] allButtons = optionalSectionTokens.GetComponentsInChildren<TokenAttackButton>();
        foreach (TokenAttackButton button in allButtons) {
            _tokenAttackButtons.Add(button);
        }
        _whosAttackObject = optionalSectionTokens.transform.Find("Subhead1").gameObject;
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
        _stuckAddButtonScript = _stuckAddButton.GetComponent<TokenAttackButton>();
        _removeStuckCost = _removeStuckCostObject.GetComponent<TextMeshProUGUI>();
        _counter = _counterObject.GetComponent<Counter>();
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
        _player = currentPlayer;
        SetSelectedAttackType(EAttackTypes.Usual);

        // раздел с атаками

        if (_player.Boosters.Vampire > 0) {
            _player.AddAvailableAttackType(EAttackTypes.Vampire);
        } else {
            _player.RemoveAvailableAttackType(EAttackTypes.Vampire);
        }

        foreach(AttackTypeButton button in _attackTypeButtons) {
            if (_player.AvailableAttackTypes.Contains(button.AttackType)) {
                button.SetAsEnabled();
            } else {
                button.SetAsDisabled();
            }
        }

        // раздел с соперниками

        bool isSingle = rivals.Count < 2;

        if (isSingle) {
            _selectedPlayer = rivals[0];
            _tokenName.text = rivals[0].PlayerName;
        } else {
            _selectedPlayer = null;
            _tokenName.text = "";
        }

        _whosAttackObject.SetActive(!isSingle);

        for (int i = 0; i <_tokenAttackButtons.Count; i++) {
            TokenAttackButton button = _tokenAttackButtons[i];

            if (i < rivals.Count) {
                PlayerControl rival = rivals[i];
                SetTokenAttackButton(button, rival, isSingle);
                button.gameObject.SetActive(true);
            } else {
                button.gameObject.SetActive(false);
            }
        }

        // Прилипалы
        _counter.Init(_counterSprite, 0, 0, _player.StuckAttached);
        UpdateStuckRemoveText(0);
        UpdateStuckBlocks();

        // сила

        UpdatePower();

        // кнопка атаки

        UpdateAttackButtonStatus();
    }

    private void SetTokenAttackButton(TokenAttackButton button, PlayerControl rival, bool disable = false) {
        button.SetTokenImage(rival.TokenImage);
        button.BindPlayer(rival);
        button.ShowSoapImage(rival.IsAbilitySoap);
        Button buttonComponent = button.gameObject.GetComponent<Button>();
        buttonComponent.onClick.RemoveAllListeners();

        if (rival.Boosters.Armor > 0) {
            TokenControl token = rival.GetTokenControl();
            Sprite sprite = rival.Boosters.IsIronArmor ? token.GetArmorIronSprite() : token.GetArmorSprite();
            button.SetShieldImage(sprite);
            button.DisableShieldImage(false);
            button.SetDisabled(disable || true);
            buttonComponent.onClick.AddListener(() => {
                _player.OpenAttackShieldModal();
            });
        } else {
            button.SetShieldImage(null);
            button.DisableShieldImage(true);
            button.SetDisabled(disable || false);
            if (!disable) {
                buttonComponent.onClick.AddListener(() => {
                    SetSelectedPlayer(button.Player);
                });
            }
        }
    }

    public void ToggleStuckAdd() {
        bool newValue = !_isAddStuck;
        _isAddStuck = newValue;
        _stuckAddButtonScript.SetSelected(newValue);
    }

    private void UpdateStuckRemoveText(int value) {
        _removeStuckCost.text = "Цена: " + value + " сила";
    }

    private void UpdateStuckBlocks() {
        if (_selectedPlayer == null) {
            _optionalSectionStuckAdd.SetActive(false);
            _stuckTextNormal.gameObject.SetActive(true);
            _stuckTextLocked.gameObject.SetActive(false);
            _stuckAddButtonScript.ShowSoapImage(false);
            _stuckAddButtonScript.SetDisabled(true);
            _optionalSectionStuckRemove.SetActive(false);
            _isAddStuck = false;
        } else {
            bool isSoap = _selectedPlayer.IsAbilitySoap;
            _optionalSectionStuckAdd.SetActive(_player.Boosters.Stuck > 0);
            _stuckTextNormal.gameObject.SetActive(!isSoap);
            _stuckTextLocked.gameObject.SetActive(isSoap);
            _stuckAddButtonScript.ShowSoapImage(isSoap);
            _stuckAddButtonScript.SetDisabled(isSoap);
            _optionalSectionStuckRemove.SetActive(!isSoap && _player.StuckAttached > 0);
            _isAddStuck = false;
        }
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
        UpdateStuckBlocks();
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
                int level = _player.Grind.MagicKick;
                result[0] = Manual.Instance.AttackMagicKick.GetEntityNameWithLevel(level);
                result[1] = Manual.Instance.AttackMagicKick.GetShortDescription(level);
                break;
            }
            case EAttackTypes.Vampire: {
                result[0] = Manual.Instance.AttackVampire.GetEntityName();
                result[1] = Manual.Instance.AttackVampire.GetShortDescription(1);
                break;
            }
            case EAttackTypes.Knockout: {
                int level = _player.Grind.Knockout;
                result[0] = Manual.Instance.AttackKnockout.GetEntityNameWithLevel(level);
                result[1] = Manual.Instance.AttackKnockout.GetShortDescription(level);
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
        int powerInitial = _player.Power;
        int powerNeed = 0;
        int kickLevel = _player.Grind.MagicKick;
        int knockoutLevel = _player.Grind.Knockout;
        int magicCost = Manual.Instance.AttackMagicKick.GetCost(kickLevel);
        int knockoutCost = Manual.Instance.AttackKnockout.GetCost(knockoutLevel);

        switch (_selectedAttackType) {
            case EAttackTypes.Usual:
            powerNeed =+ 1;
            break;
            case EAttackTypes.MagicKick:
            powerNeed =+ magicCost;
            break;
            case EAttackTypes.Vampire:
            powerNeed =- 1;
            break;
            case EAttackTypes.Knockout:
            powerNeed += knockoutCost;
            break;
        }

        // remove stuck count
        powerNeed += _counter.Count;

        int powerLeft = powerInitial - powerNeed;
        _powerNeed = powerNeed;
        string powerLeftString = powerLeft > 0 ? powerLeft.ToString() : Utils.Wrap(powerLeft.ToString(), UIColors.Red);

        _powerNow.text = "Сил в наличии: <b>" + powerInitial + "</b>";
        _powerLeft.text = "Сил останется: <b>" + powerLeftString + "</b>";

        UpdateAttackButtonStatus();
    }

    private void UpdateAttackButtonStatus() {
        if (_selectedPlayer == null) {
            SetButtonAttackInteractable(false);
            _warningText.text = "Выберите соперника";
        } else if (_player.Power - _powerNeed < 0) {
            SetButtonAttackInteractable(false);
            _warningText.text = "Мало сил";
        } else {
            SetButtonAttackInteractable(true);
            _warningText.text = "";
        }
    }

    public void OnIncreaseStuckClick() {
        _counter.OnIncrease();
        UpdateStuckRemoveText(_counter.Count);
        UpdatePower();
    }

    public void OnDiscreaseStuckClick() {
        _counter.OnDiscrease();
        UpdateStuckRemoveText(_counter.Count);
        UpdatePower();
    }

    private IEnumerator ConfirmAttackDefer() {
        yield return new WaitForSeconds(_attackDelay);
        _player.ExecuteAttack(_selectedAttackType, _isAddStuck, _counter.Count, _selectedPlayer);
    }

    private IEnumerator CancelAttackDefer() {
        yield return new WaitForSeconds(_attackDelay);
        ResetContent();
        _player.ExecuteCancelAttack();
    }

    public void ConfirmAttack() {
        StartCoroutine(ConfirmAttackDefer());
    }

    public void CancelAttack() {
        StartCoroutine(CancelAttackDefer());
    }
}

using System.Collections.Generic;
using UnityEngine;

public class BoostersControl : MonoBehaviour
{
    public static BoostersControl Instance { get; private set; }
    private Sprite _magnetSprite, _magnetSuperSprite, _lassoSprite, _shieldSprite, _shieldIronSprite;
    [SerializeField] private GameObject _magnetsRow, _lassoRow, _shieldsRow;
    private BoostersRow _magnetsRowScript, _lassoRowScript, _shieldsRowScript;
    private PopupMagnet _popupMagnet;
    [SerializeField] private List<GameObject> _boostersList;
    private List<BoosterButton> _boosterButtonsList = new();
    private TopPanel _topPanel;

    private void Awake() {
        Instance = this;
        GameObject Instances = GameObject.Find("Instances");

        _magnetSprite = Instances.transform.Find("magnet").GetComponent<SpriteRenderer>().sprite;
        _magnetSuperSprite = Instances.transform.Find("magnet-super").GetComponent<SpriteRenderer>().sprite;
        _shieldSprite = Instances.transform.Find("shield").GetComponent<SpriteRenderer>().sprite;
        _shieldIronSprite = Instances.transform.Find("shield-iron").GetComponent<SpriteRenderer>().sprite;
        _lassoSprite = Instances.transform.Find("lasso").GetComponent<SpriteRenderer>().sprite;

        if (_magnetsRow != null) {
            _magnetsRowScript = _magnetsRow.GetComponent<BoostersRow>();
        }
        if (_lassoRow != null) {
            _lassoRowScript = _lassoRow.GetComponent<BoostersRow>();
        }
        if (_shieldsRow != null) {
            _shieldsRowScript = _shieldsRow.GetComponent<BoostersRow>();
        }

        _popupMagnet = GameObject.Find("GameScripts").GetComponent<PopupMagnet>();
        foreach(GameObject button in _boostersList) {
            _boosterButtonsList.Add(button.GetComponent<BoosterButton>());
        }
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
    }

    public Sprite MagnetSprite {
        get { return _magnetSprite; }
        private set {}
    }

    public Sprite MagnetSuperSprite {
        get { return _magnetSuperSprite; }
        private set {}
    }

    public Sprite LassoSprite {
        get { return _lassoSprite; }
        private set {}
    }

    public Sprite ShieldSprite {
        get { return _shieldSprite; }
        private set {}
    }

    public Sprite ShieldIronSprite {
        get { return _shieldIronSprite; }
        private set {}
    }

    public void DisableAllButtons(BoosterButton exceptButton = null) {
        foreach(BoosterButton button in _boosterButtonsList) {
            button.SetDisabled(button != exceptButton);
        }
    }

    // Если у игрока активированы щиты, то кнопка проигнорирует SetDisabled
    // filterShields - энейблить только щиты, все остальное оставить дисейбл

    public void EnableAllButtons(bool filterShields = false) {
        if (filterShields) {
            foreach(BoosterButton button in _boosterButtonsList) {
                if (button.IsShield()) {
                    button.SetDisabled(false);
                }
                button.SetSelected(false);
            }
        } else {
            foreach(BoosterButton button in _boosterButtonsList) {
                button.SetDisabled(false);
                button.SetSelected(false);
            }
        }
    }

    public void UnselectAllButtons() {
        foreach(BoosterButton button in _boosterButtonsList) {
            button.SetSelected(false);
        }
    }

    // Обновление содержимого строки усилителей, в которых используются разные виды усилителей (магниты, щиты)

    private void UpdateBoostersRow(int booster1Count, int booster2Count, EBoosters booster1, EBoosters booster2, BoostersRow targetScript) {
        int buttonNumber = 1;

        for (int i = 0; i < booster1Count; i++) {
            if (buttonNumber > 3) {
                break;
            }
            targetScript.UpdateButton(buttonNumber, booster1);
            buttonNumber++;
        }

        for (int i = 0; i < booster2Count; i++) {
            if (buttonNumber > 3) {
                break;
            }
            targetScript.UpdateButton(buttonNumber, booster2);
            buttonNumber++;
        }

        // Очистка незадействованных кнопок

        if (buttonNumber <= 3) {
            for (int i = buttonNumber; i <= 3; i++) {
                targetScript.UpdateButton(i, EBoosters.None);
            }
        }
    }

    // Обновление содержимого панели усилителей у игрока

    public void UpdateBoostersFromPlayer(PlayerControl player) {
        // Лассо

        for (int i = 1; i <= 3; i++) {
            EBoosters booster = player.BoosterLasso >= i ? EBoosters.Lasso : EBoosters.None;
            _lassoRowScript.UpdateButton(i, booster);
        }

        // Магниты

        UpdateBoostersRow(player.BoosterMagnet, player.BoosterSuperMagnet, EBoosters.Magnet, EBoosters.MagnetSuper, _magnetsRowScript);

        // Щиты

        UpdateBoostersRow(player.BoosterShield, player.BoosterShieldIron, EBoosters.Shield, EBoosters.ShieldIron, _shieldsRowScript);
        UpdatePlayersArmorButtons(player);
    }

    // Открытие разных усилителей при нажатиях на кнопки в левой панели

    public void ActivateBooster(EBoosters booster) {
        CellControl cell = MoveControl.Instance.CurrentPlayer.GetCurrentCell();

        switch(booster) {
            case EBoosters.Magnet: {
                _popupMagnet.BuildContent(MoveControl.Instance.CurrentPlayer, cell, false);
                _popupMagnet.OnOpenWindow();
                break;
            }
            case EBoosters.MagnetSuper: {
                _popupMagnet.BuildContent(MoveControl.Instance.CurrentPlayer, cell, true);
                _popupMagnet.OnOpenWindow();
                break;
            }
            case EBoosters.Lasso: {
                CubicControl.Instance.SetCubicInteractable(false);
                _topPanel.SetText("Подвиньте свою фишку в пределах 3 шагов"); // todo зависит от уровня лассо
                _topPanel.OpenWindow();
                List<GameObject> collected = CellsControl.Instance.FindNearCellsDeepTwoSide(cell, 3);
                foreach(GameObject cellFound in collected) {
                    cellFound.GetComponent<CellControl>().TurnOnLassoMode();
                }
                _topPanel.SetCancelButtonActive(true, () => {
                    _topPanel.CloseWindow();
                    foreach(GameObject cellFound in collected) {
                        cellFound.GetComponent<CellControl>().TurnOffLassoMode();
                    }
                    EffectsControl.Instance.TryToEnableAllEffectButtons();
                    EnableAllButtons();
                    CubicControl.Instance.SetCubicInteractable(true);
                });
                break;
            }
        }
    }

    // Исполнение некоторых усилителей

    public void ExecuteLasso(CellControl targetCell) {
        foreach(CellControl cell in CellsControl.Instance.AllCellsControls) {
            cell.TurnOffLassoMode();
        }
        _topPanel.CloseWindow();
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " решил прокатиться на " + Utils.Wrap("лассо", UIColors.Orange);
        Messages.Instance.AddMessage(message);
        player.AddLasso(-1);
        UpdateBoostersFromPlayer(player);
        UnselectAllButtons();
        MoveControl.Instance.MakeLassoMove(targetCell);
    }

    public void ExecuteShield(PlayerControl player, bool _isIron) {
        TokenControl token = player.GetTokenControl();
        string shieldText;

        if (_isIron) {
            player.IsIronArmor = true;
            player.Armor = 12;
            token.UpdateShield(EBoosters.ShieldIron);
            shieldText = Utils.Wrap("железный щит", UIColors.ArmorIron);
        } else {
            player.IsIronArmor = false;
            player.Armor = 4;
            token.UpdateShield(EBoosters.Shield);
            shieldText = Utils.Wrap("щит", UIColors.Armor);
        }

        UpdatePlayersArmorButtons(player);

        string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " надевает " + shieldText;
        Messages.Instance.AddMessage(message);
    }

    public void DeactivateArmorButtons() {
        _shieldsRowScript.DeactivateShieldMode();
    }

    public void UpdatePlayersArmorButtons(PlayerControl player) {
        _shieldsRowScript.UpdateShieldMode(
            player.SelectedShieldButton,
            player.IsIronArmor,
            player.Armor
        );
    }
}

using System.Collections.Generic;
using UnityEngine;

public class BoostersControl : MonoBehaviour
{
    public static BoostersControl Instance { get; private set; }
    private Sprite _magnetSprite, _magnetSuperSprite, _lassoSprite, _shieldSprite, _shieldIronSprite, _vampireSprite;
    [SerializeField] private GameObject _magnetsRow, _lassoRow, _shieldsRow, _vampireButton;
    private BoostersRow _magnetsRowScript, _lassoRowScript, _shieldsRowScript;
    private BoosterButton _vampireButtonScript;
    private PopupMagnet _popupMagnet;
    [SerializeField] private List<GameObject> _boostersList;
    private List<BoosterButton> _boosterButtonsList = new();
    private TopPanel _topPanel;
    private ModalWarning _modalWarning;
    [SerializeField] private int _maxMagnets = 3;
    [SerializeField] private int _maxSuperMagnets = 3;
    [SerializeField] private int _maxMagnetsTotal = 3;
    [SerializeField] private int _maxLasso = 3;
    [SerializeField] private int _maxShields = 3;
    [SerializeField] private int _maxShieldsIron = 3;
    [SerializeField] private int _maxShieldsTotal = 3;
    [SerializeField] private int _maxVampires = 1;

    private void Awake() {
        Instance = this;
        GameObject Instances = GameObject.Find("Instances");

        _magnetSprite = Instances.transform.Find("magnet").GetComponent<SpriteRenderer>().sprite;
        _magnetSuperSprite = Instances.transform.Find("magnet-super").GetComponent<SpriteRenderer>().sprite;
        _shieldSprite = Instances.transform.Find("shield").GetComponent<SpriteRenderer>().sprite;
        _shieldIronSprite = Instances.transform.Find("shield-iron").GetComponent<SpriteRenderer>().sprite;
        _lassoSprite = Instances.transform.Find("lasso").GetComponent<SpriteRenderer>().sprite;
        _vampireSprite = Instances.transform.Find("vampire").GetComponent<SpriteRenderer>().sprite;

        if (_magnetsRow != null) {
            _magnetsRowScript = _magnetsRow.GetComponent<BoostersRow>();
        }
        if (_lassoRow != null) {
            _lassoRowScript = _lassoRow.GetComponent<BoostersRow>();
        }
        if (_shieldsRow != null) {
            _shieldsRowScript = _shieldsRow.GetComponent<BoostersRow>();
        }
        if (_vampireButton != null) {
            _vampireButtonScript = _vampireButton.GetComponent<BoosterButton>();
        }

        _popupMagnet = GameObject.Find("GameScripts").GetComponent<PopupMagnet>();
        foreach(GameObject button in _boostersList) {
            _boosterButtonsList.Add(button.GetComponent<BoosterButton>());
        }
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _modalWarning = GameObject.Find("ModalScripts").GetComponent<ModalWarning>();
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

    public Sprite VampireSprite {
        get { return _vampireSprite; }
        private set {}
    }

    public int MaxMagnets {
        get { return _maxMagnets; }
        private set {}
    }

    public int MaxSuperMagnets {
        get { return _maxSuperMagnets; }
        private set {}
    }

    public int MaxMagnetsTotal {
        get { return _maxMagnetsTotal; }
        private set {}
    }

    public int MaxShields {
        get { return _maxShields; }
        private set {}
    }

    public int MaxShieldsIron {
        get { return _maxShieldsIron; }
        private set {}
    }

    public int MaxShieldsTotal {
        get { return _maxShieldsTotal; }
        private set {}
    }

    public int MaxLasso {
        get { return _maxLasso; }
        private set {}
    }

    public int MaxVampires {
        get { return _maxVampires; }
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
            EBoosters booster = player.Boosters.Lasso >= i ? EBoosters.Lasso : EBoosters.None;
            _lassoRowScript.UpdateButton(i, booster);
        }

        // Магниты

        UpdateBoostersRow(player.Boosters.Magnet, player.Boosters.SuperMagnet, EBoosters.Magnet, EBoosters.MagnetSuper, _magnetsRowScript);

        // Щиты

        UpdateBoostersRow(player.Boosters.Shield, player.Boosters.ShieldIron, EBoosters.Shield, EBoosters.ShieldIron, _shieldsRowScript);
        UpdatePlayersArmorButtons(player);

        // Вампир

        _vampireButtonScript.BoosterType = player.Boosters.Vampire > 0 ? EBoosters.Vampire : EBoosters.None;
    }

    // Открытие разных усилителей при нажатиях на кнопки в левой панели

    public void ActivateBooster(EBoosters booster) {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        CellControl cell = player.GetCurrentCell();

        switch(booster) {
            case EBoosters.Magnet: {
                _popupMagnet.BuildContent(player, cell, false);
                _popupMagnet.OnOpenWindow();
                break;
            }
            case EBoosters.MagnetSuper: {
                _popupMagnet.BuildContent(player, cell, true);
                _popupMagnet.OnOpenWindow();
                break;
            }
            case EBoosters.Lasso: {
                CubicControl.Instance.SetCubicInteractable(false);

                int level = player.Grind.Lasso;
                ManualContent manual = Manual.Instance.BoosterLasso;
                int steps = manual.GetCauseEffect(level);

                string stepsText = steps == 1 ? " шага" : " шагов";
                _topPanel.SetText("Подвиньте свою фишку в пределах " + steps + stepsText);
                _topPanel.OpenWindow();
                List<GameObject> collected = CellsControl.Instance.FindNearCellsDeepTwoSide(cell, steps);
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
        string message = Utils.Wrap(player.PlayerName, UIColors.Yellow) + " решает прокатиться на " + Utils.Wrap("лассо", UIColors.Orange);
        Messages.Instance.AddMessage(message);
        player.Boosters.AddLasso(-1);
        UpdateBoostersFromPlayer(player);
        UnselectAllButtons();
        MoveControl.Instance.MakeLassoMove(targetCell);
    }

    public void ExecuteShield(PlayerControl player, bool _isIron) {
        TokenControl token = player.GetTokenControl();
        string shieldText;

        if (_isIron) {
            player.Boosters.IsIronArmor = true;
            player.Boosters.Armor = 12;
            token.UpdateShield(EBoosters.ShieldIron);
            shieldText = Utils.Wrap("железный щит", UIColors.ArmorIron);
        } else {
            player.Boosters.IsIronArmor = false;
            player.Boosters.Armor = 4;
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

    // Проверяет, является оверлей щита актуальным для данной кнопки

    private bool IsArmorOverlayActual(PlayerBoosters boosters) {
        BoosterButton button = boosters.SelectedShieldButton;

        if (button == null || boosters.Armor == 0) {
            return true;
        }

        if (boosters.IsIronArmor) {
            return button.BoosterType == EBoosters.ShieldIron;
        } else {
            return button.BoosterType == EBoosters.Shield;
        }
    }

    public void UpdatePlayersArmorButtons(PlayerControl player) {
        // В этот момент количество щитов могло измениться. Поэтому SelectedShieldButton также может измениться
        BoosterButton button = player.Boosters.SelectedShieldButton;
        bool isActual = IsArmorOverlayActual(player.Boosters);
        
        if (!isActual) {
            int index = player.Boosters.GetActualBoosterButtonIndex();
            button = _shieldsRowScript.List[index];
            player.Boosters.SelectedShieldButton = button;
        }

        _shieldsRowScript.UpdateShieldMode(
            button,
            player.Boosters.IsIronArmor,
            player.Boosters.Armor
        );
    }

    // разное

    public void ShowAttackOnlyWarning() {
        _modalWarning.SetHeadingText("Недоступно");
        _modalWarning.SetBodyText("Этот усилитель доступен только во время атаки на соперника.");
        _modalWarning.SetCallback();
        _modalWarning.OpenModal();
    }

    public ManualContent GetBoosterManual(EBoosters booster) {
        switch(booster) {
            case EBoosters.Magnet: {
                return Manual.Instance.BoosterMagnet;
            }
            case EBoosters.MagnetSuper: {
                return Manual.Instance.BoosterSuperMagnet;
            }
            case EBoosters.Lasso: {
                return Manual.Instance.BoosterLasso;
            }
            case EBoosters.Shield: {
                return Manual.Instance.BoosterShield;
            }
            case EBoosters.ShieldIron: {
                return Manual.Instance.BoosterIronShield;
            }
            case EBoosters.Vampire: {
                return Manual.Instance.BoosterVampire;
            }
            default: return null;
        }
    }
}

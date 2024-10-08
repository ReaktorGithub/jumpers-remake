using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostersControl : MonoBehaviour
{
    public static BoostersControl Instance { get; private set; }
    private Sprite _magnetSprite, _magnetSuperSprite, _lassoSprite, _shieldSprite, _shieldIronSprite, _vampireSprite, _boombasterSprite, _stuckSprite, _trapSprite, _flashSprite;
    [SerializeField] private GameObject _magnetsRow, _lassoRow, _shieldsRow, _vampireButton, _boombasterButton, _stuckButton, _trapButton, _flashButton, _flashBlock;
    private BoostersRow _magnetsRowScript, _lassoRowScript, _shieldsRowScript;
    private BoosterButton _vampireButtonScript, _boombasterButtonScript, _stuckButtonScript, _trapButtonScript, _flashButtonScript;
    private PopupMagnet _popupMagnet;
    [SerializeField] private List<GameObject> _boostersList;
    private List<BoosterButton> _boosterButtonsList = new();
    private TopPanel _topPanel;
    private ModalWarning _modalWarning;
    private CameraControl _cameraControl;
    private CameraButton _cameraButton;
    private Image _flashAnimateImage;
    private IEnumerator _flashCoroutine;
    [SerializeField] private List<EBoosters> _boostersWithGrind = new();
    [SerializeField] private float _flashPulseTime = 0.5f;
    [SerializeField] private int _maxMagnets = 3;
    [SerializeField] private int _maxSuperMagnets = 3;
    [SerializeField] private int _maxMagnetsTotal = 3;
    [SerializeField] private int _maxLasso = 3;
    [SerializeField] private int _maxShields = 3;
    [SerializeField] private int _maxShieldsIron = 3;
    [SerializeField] private int _maxShieldsTotal = 3;
    [SerializeField] private int _maxVampires = 1;
    [SerializeField] private int _maxBoombasters = 1;
    [SerializeField] private int _maxStuck = 1;
    [SerializeField] private int _maxTrap = 1;
    [SerializeField] private int _maxFlash = 1;

    private void Awake() {
        Instance = this;
        GameObject Instances = GameObject.Find("Instances");

        _magnetSprite = Instances.transform.Find("magnet").GetComponent<SpriteRenderer>().sprite;
        _magnetSuperSprite = Instances.transform.Find("magnet-super").GetComponent<SpriteRenderer>().sprite;
        _shieldSprite = Instances.transform.Find("shield").GetComponent<SpriteRenderer>().sprite;
        _shieldIronSprite = Instances.transform.Find("shield-iron").GetComponent<SpriteRenderer>().sprite;
        _lassoSprite = Instances.transform.Find("lasso").GetComponent<SpriteRenderer>().sprite;
        _vampireSprite = Instances.transform.Find("vampire").GetComponent<SpriteRenderer>().sprite;
        _boombasterSprite = Instances.transform.Find("boombaster-icon").GetComponent<SpriteRenderer>().sprite;
        _stuckSprite = Instances.transform.Find("stuck-icon").GetComponent<SpriteRenderer>().sprite;
        _trapSprite = Instances.transform.Find("trap-icon").GetComponent<SpriteRenderer>().sprite;
        _flashSprite = Instances.transform.Find("flash-icon").GetComponent<SpriteRenderer>().sprite;

        _magnetsRowScript = _magnetsRow.GetComponent<BoostersRow>();
        _lassoRowScript = _lassoRow.GetComponent<BoostersRow>();
        _shieldsRowScript = _shieldsRow.GetComponent<BoostersRow>();
        _vampireButtonScript = _vampireButton.GetComponent<BoosterButton>();
        _boombasterButtonScript = _boombasterButton.GetComponent<BoosterButton>();
        _stuckButtonScript = _stuckButton.GetComponent<BoosterButton>();
        _trapButtonScript = _trapButton.GetComponent<BoosterButton>();
        _flashButtonScript = _flashButton.GetComponent<BoosterButton>();

        _popupMagnet = GameObject.Find("GameScripts").GetComponent<PopupMagnet>();
        foreach(GameObject button in _boostersList) {
            _boosterButtonsList.Add(button.GetComponent<BoosterButton>());
        }
        _topPanel = GameObject.Find("TopBlock").GetComponent<TopPanel>();
        _modalWarning = GameObject.Find("ModalScripts").GetComponent<ModalWarning>();
        _cameraControl = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _cameraButton = GameObject.Find("CameraButton").GetComponent<CameraButton>();
        _flashAnimateImage = _flashBlock.transform.Find("AnimateImage").GetComponent<Image>();
        LockInterfaceByFlash(false);
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

    public Sprite BoombasterSprite {
        get { return _boombasterSprite; }
        private set {}
    }

    public Sprite StuckSprite {
        get { return _stuckSprite; }
        private set {}
    }

    public Sprite TrapSprite {
        get { return _trapSprite; }
        private set {}
    }

    public Sprite FlashSprite {
        get { return _flashSprite; }
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

    public int MaxBoombasters {
        get { return _maxBoombasters; }
        private set {}
    }

    public int MaxStuck {
        get { return _maxStuck; }
        private set {}
    }

    public int MaxTrap {
        get { return _maxTrap; }
        private set {}
    }

    public int MaxFlash {
        get { return _maxFlash; }
        private set {}
    }

    public List<EBoosters> BoostersWithGrind {
        get { return _boostersWithGrind; }
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

        // Другие

        _vampireButtonScript.BoosterType = player.Boosters.Vampire > 0 ? EBoosters.Vampire : EBoosters.None;
        _boombasterButtonScript.BoosterType = player.Boosters.Boombaster > 0 ? EBoosters.Boombaster : EBoosters.None;
        _stuckButtonScript.BoosterType = player.Boosters.Stuck > 0 ? EBoosters.Stuck : EBoosters.None;
        _trapButtonScript.BoosterType = player.Boosters.Trap > 0 ? EBoosters.Trap : EBoosters.None;
        _flashButtonScript.BoosterType = player.Boosters.Flash > 0 ? EBoosters.Flash : EBoosters.None;
    }

    // Открытие разных усилителей при нажатиях на кнопки в левой панели (без исполнения их эффекта)

    public void ActivateBooster(EBoosters booster) {
        switch(booster) {
            case EBoosters.Magnet: {
                OpenMagnet(false);
                break;
            }
            case EBoosters.MagnetSuper: {
                OpenMagnet(true);
                break;
            }
            case EBoosters.Lasso: {
                OpenLasso();
                break;
            }
            case EBoosters.Trap: {
                OpenTrap();
                break;
            }
        }
    }

    private void OpenMagnet(bool isSuper) {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        CellControl cell = player.GetCurrentCell();
        _popupMagnet.BuildContent(player, cell, isSuper);
        _popupMagnet.OnOpenWindow();
    }

    private void OpenLasso() {
        PlayerControl player = MoveControl.Instance.CurrentPlayer;
        CellControl cell = player.GetCurrentCell();

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
    }

    private void OpenTrap() {
        CubicControl.Instance.SetCubicInteractable(false);
        CellsControl.Instance.TurnOnTrapPlacementMode();
        _cameraControl.FollowOff();
        _cameraControl.MoveCameraToLevelCenter();
        _cameraButton.SetDisabled(true);
        _topPanel.SetText("Установите капкан на клетку без фишек");
        _topPanel.OpenWindow();
        _topPanel.SetCancelButtonActive(true, () => {
            _topPanel.CloseWindow();
            CellsControl.Instance.TurnOffTrapPlacementMode();
            EffectsControl.Instance.TryToEnableAllEffectButtons();
            EffectsControl.Instance.RestoreCamera();
            EnableAllButtons();
            CubicControl.Instance.SetCubicInteractable(true);
        });
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

    public void LockInterfaceByFlash(bool value) {
        if (value) {
            _flashBlock.SetActive(true);
            if (_flashCoroutine != null) {
                StopCoroutine(_flashCoroutine);
            }
            _flashCoroutine = Utils.StartPulseImage(_flashAnimateImage, _flashPulseTime, 0.2f);
            StartCoroutine(_flashCoroutine);
        } else {
            if (_flashCoroutine != null) {
                StopCoroutine(_flashCoroutine);
            }
            _flashBlock.SetActive(false);
        }
    }

    public int GetBoombasterPowerPenalty(int level, int areaRow) {
        switch(level) {
            case 1: {
                return areaRow switch {
                    0 => 3,
                    1 => 1,
                    _ => 0,
                };
            }
            case 2: {
                return areaRow switch {
                    0 => 3,
                    1 => 2,
                    2 => 1,
                    _ => 0,
                };
            }
            case 3: {
                return areaRow switch {
                    0 => 4,
                    1 => 3,
                    2 => 2,
                    _ => 1,
                };
            }
            default: return 0;
        }
    }
}

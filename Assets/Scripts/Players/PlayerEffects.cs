using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]

public class PlayerEffects : MonoBehaviour
{
    private PlayerControl _player;
    private ModalWin _modalWin;
    private ModalSurprise _modalSurprise;
    private CameraControl _camera;
    private Pedestal _pedestal;
    private HedgehogsControl _hedgehogsControl;
    [SerializeField] private int _green = 0;
    [SerializeField] private int _yellow = 0;
    [SerializeField] private int _black = 0;
    [SerializeField] private int _red = 0;
    [SerializeField] private int _star = 0;
    [SerializeField] private int _lightningMoves = 0; // осталось ходов с молнией
    [SerializeField] private bool _isLightning = false; // режим молнии
    private bool _isEffectPlaced;

    private void Awake() {
        _player = GetComponent<PlayerControl>();
        _modalWin = GameObject.Find("ModalScripts").GetComponent<ModalWin>();
        _modalSurprise = GameObject.Find("ModalScripts").GetComponent<ModalSurprise>();
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
        _hedgehogsControl = GameObject.Find("LevelScripts").GetComponent<HedgehogsControl>();
    }

    public int Green {
        get { return _green; }
        set { _green = value; }
    }

    public int Yellow {
        get { return _yellow; }
        set { _yellow = value; }
    }

    public int Red {
        get { return _red; }
        set { _red = value; }
    }

    public int Black {
        get { return _black; }
        set { _black = value; }
    }

    public int Star {
        get { return _star; }
        set { _star = value; }
    }

    public bool IsLightning {
        get { return _isLightning; }
        private set {}
    }

    public bool IsEffectPlaced {
        get { return _isEffectPlaced; }
        set { _isEffectPlaced = value; }
    }

    public void AddGreen(int value) {
        _green += value;
    }

    public void AddYellow(int value) {
        _yellow += value;
    }

    public void AddBlack(int value) {
        _black += value;
    }

    public void AddRed(int value) {
        _red += value;
    }

    public void AddStar(int value) {
        _star += value;
    }

    // исполнение эффектов

    public void ExecuteGreen(int level = 0) {
        ManualContent manual = Manual.Instance.GetEffectManual(EControllableEffects.Green);
        CellControl cell = _player.GetCurrentCell();
        int effectLevel = level == 0 ? cell.EffectLevel : level;
        int moves = manual.GetCauseEffect(effectLevel);
        int coins = effectLevel == 3 ? 50 : 0;

        _player.AddMovesToDo(moves);
        if (coins != 0) {
            _player.AddCoins(coins);
            PlayersControl.Instance.UpdatePlayersInfo();
        }

        string movesMessage = moves == 1 ? "раз" : (moves + " раза");
        string coinsMessage = coins > 0 ? ". " + Utils.Wrap("Бонус", UIColors.Green) + " +" + coins + " монет!" : "";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("зелёный", UIColors.Green) + " эффект и походит ещё " + movesMessage + coinsMessage;
        Messages.Instance.AddMessage(message);
    }

    public void ExecuteYellow(int level = 0) {
        ManualContent manual = Manual.Instance.GetEffectManual(EControllableEffects.Yellow);
        CellControl cell = _player.GetCurrentCell();
        int effectLevel = level == 0 ? cell.EffectLevel : level;
        int coins = manual.GetCauseEffect(effectLevel);

        _player.SkipMoveIncrease();
        if (coins != 0) {
            _player.AddCoins(coins);
            PlayersControl.Instance.UpdatePlayersInfo();
        }

        string coinsMessage = coins == 0 ? "" : ". " + Utils.Wrap("Штраф", UIColors.Red) + " " + coins + " монет";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("жёлтый", UIColors.Yellow) + " эффект и пропустит ход" + coinsMessage;
        Messages.Instance.AddMessage(message);
    }

    // Если игрок находится на желтой клетке 3 уровня и пропускает более 1 года подряд, то снизить его силу на 1

    public bool ExecuteYellowPowerPenalty() {
        CellControl cell = _player.GetCurrentCell();

        if (cell.Effect != EControllableEffects.Yellow || cell.EffectLevel != 3 || _player.SkipMoveCount < 2) {
            return true;
        }

        _player.AddPower(-1);

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " слишком долго отдыхает на жёлтой клетке 3 уровня! " + Utils.Wrap("Штраф", UIColors.Red) + " -1 сила";
        Messages.Instance.AddMessage(message);

        PlayersControl.Instance.CheckIsPlayerOutOfPower(_player, () => MoveControl.Instance.EndMove());
        
        return false;
    }

    public void ExecuteBlack() {
        if (_player.IsLuckyStar) {
            _player.OpenSavedByStarModal(() => {
                CellChecker.Instance.CheckCellArrows(_player);
            });
            return;
        }

        if (_player.Boosters.Armor > 0 && _player.Boosters.IsIronArmor) {
            _player.OpenSavedByShieldModal(() => {
                CellChecker.Instance.CheckCellArrows(_player);
            });
            return;
        }

        _player.AddPower(-1);
        PlayersControl.Instance.UpdatePlayersInfo();

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("ЧЁРНЫЙ", UIColors.Black) + " эффект! -1 сила";
        Messages.Instance.AddMessage(message);

        PlayersControl.Instance.CheckIsPlayerOutOfPower(_player, () => CellChecker.Instance.CheckCellArrows(_player));
    }

    public void ExecuteRed(int level = 0, GameObject penaltyCell = null) {
        _player.MovesToDo = 0;
        _player.StepsLeft = 0;

        if (_player.Boosters.Armor > 0 && _player.Boosters.IsIronArmor) {
            if (_player.IsMe()) {
                _player.OpenSavedByShieldModal(() => ProcessShieldRedEffect(penaltyCell));
            } else {
                ProcessShieldRedEffect(penaltyCell);
            }
            return;
        }

        CellControl cell = _player.GetCurrentCell();
        ManualContent manual = Manual.Instance.GetEffectManual(EControllableEffects.Red);
        int effectLevel = level == 0 ? cell.EffectLevel : level;
        int powerPenalty = manual.GetCauseEffect(effectLevel);

        _player.AddPower(-powerPenalty);
        PlayersControl.Instance.UpdatePlayersInfo();

        string powerText = powerPenalty == 1 ? "сила" : "силы";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("КРАСНЫЙ", UIColors.Red) + " эффект! -" + powerPenalty + " " + powerText + ". Возврат на чекпойнт";
        Messages.Instance.AddMessage(message);

        PlayersControl.Instance.CheckIsPlayerOutOfPower(
            _player,
            () => StartCoroutine(RedEffectTokenMoveDefer(penaltyCell))
        );
    }

    private void ProcessShieldRedEffect(GameObject penaltyCell = null) {
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("КРАСНЫЙ", UIColors.Red) + " эффект! Возврат на чекпойнт";
        Messages.Instance.AddMessage(message);
        StartCoroutine(RedEffectTokenMoveDefer(penaltyCell));
    }

    private IEnumerator RedEffectTokenMoveDefer(GameObject penaltyCell = null) {
        yield return new WaitForSeconds(PlayersControl.Instance.RedEffectDelay);
        RedEffectTokenMove(penaltyCell);
    }

    private void RedEffectTokenMove(GameObject penaltyCell = null) {
        TokenControl tokenControl = _player.GetTokenControl();
        CellControl cellControl = tokenControl.GetCurrentCellControl();
        
        GameObject targetCell = penaltyCell;
        if (targetCell == null) {
            if (!tokenControl.CurrentCell.TryGetComponent(out RedCell redCell)) {
                Debug.Log("Red cell not found");
                return;
            }

            if (redCell != null) {
                targetCell = redCell.PenaltyCell;
            } else {
                Debug.Log("Penalty cell not found");
            }
        }
        
        float moveTime = MoveControl.Instance.SpecifiedMoveTime;
        tokenControl.SetToSpecifiedCell(targetCell, moveTime, () => {
            cellControl.RemoveToken(_player.TokenObject);
            MoveControl.Instance.ConfirmNewPosition();
        });
    }

    public void ExecuteStar(int level = 0) {
        CellControl cell = _player.GetCurrentCell();
        ManualContent manual = Manual.Instance.GetEffectManual(EControllableEffects.Star);
        int effectLevel = level == 0 ? cell.EffectLevel : level;
        int powerBonus = manual.GetCauseEffect(effectLevel);

        string powerText = powerBonus == 1 ? " силу" : " силы";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("звезду", UIColors.DarkBlue) + " и получает " + powerBonus + powerText;
        Messages.Instance.AddMessage(message);

        bool isLucky = effectLevel == 3;
        if (isLucky && !_player.IsLuckyStar) {
            string luckyMessage = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " получает " + Utils.Wrap("счастливую звезду!", UIColors.DarkBlue) + " Защита от " + Utils.Wrap("чёрных", UIColors.Black) + " клеток до конца гонки";
            Messages.Instance.AddMessage(luckyMessage);
        }

        _player.IsLuckyStar = isLucky;
        _player.AddPower(powerBonus);
        PlayersControl.Instance.UpdatePlayersInfo();
    }

    public void ExecuteFinish() {
        if (_player.IsMe()) {
            _modalWin.OpenModal();
        }
        _player.IsFinished = true;
        int place = _pedestal.SetPlayerToMaxPlace(_player);

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + Utils.Wrap(" ФИНИШИРУЕТ ", UIColors.Green) + " на " + place + " месте!";
        Messages.Instance.AddMessage(message);

        TokenControl tokenControl = _player.GetTokenControl();
        IEnumerator coroutine = tokenControl.MoveToPedestalDefer(PlayersControl.Instance.FinishDelay, () => {
            _pedestal.SetTokenToPedestal(_player, place);
            StartCoroutine(MoveControl.Instance.EndMoveDefer());
        });
        StartCoroutine(coroutine);
        _camera.ClearFollow();
    }

    public void ExecuteHedgehogArrow(List<EBoosters> selectedBoosters) {
        foreach(EBoosters booster in selectedBoosters) {
            _player.Boosters.AddTheBooster(booster, -1);
        }
    }

    public void ExecuteHedgehogFinishPay(int cost) {
        CellControl cell = _player.GetCurrentCell();

        if (cell.TryGetComponent(out FinishCell finishCell)) {
            finishCell.AddCoinsCollected(cost);
        } else {
            Debug.Log("Finish cell not found");
        }
        
        _player.AddCoins(-cost);
        PlayersControl.Instance.UpdatePlayersInfo();

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " платит дань " + Utils.Wrap("ежу", UIColors.DarkGreen);
        Messages.Instance.AddMessage(message);

        ExecuteFinish();
    }

    public void ExecuteHedgehogFinishFight() {
        _player.AddPower(-3);
        PlayersControl.Instance.UpdatePlayersInfo();

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + Utils.Wrap(" ПОБИЛ ", UIColors.Red) + Utils.Wrap("ежа!", UIColors.DarkGreen) + " Финиш свободен";
        Messages.Instance.AddMessage(message);

        CellControl cell = _player.GetCurrentCell();

        if (cell.TryGetComponent(out FinishCell finishCell)) {
            StartCoroutine(_hedgehogsControl.SpawnHedgehogItemsDefer(finishCell, _player));
        } else {
            Debug.Log("Finish cell not found");
        }
    }

    public void ExecuteMoneybox(MoneyboxVault vault) {
        if (_player.Boosters.IsBlot()) {
            _player.Boosters.ExecuteBlotAsVictim("забрать бонус из копилки");
            _player.AddMovesSkip(1);
            MoveControl.Instance.CheckMoveSkipAndPreparePlayer();
            return;
        }

        (int, int, int) bonus = vault.GetBonus();
        if (bonus.Item1 != 0) {
            _player.AddPower(bonus.Item1);
        }
        if (bonus.Item2 != 0) {
           _player.AddCoins(bonus.Item2); 
        }
        if (bonus.Item3 != 0) {
            _player.AddRubies(bonus.Item3);
        }
        _player.AddMovesSkip(1);
        PlayersControl.Instance.UpdatePlayersInfo();

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " забирает бонус из " + Utils.Wrap("копилки", UIColors.Green);
        Messages.Instance.AddMessage(message);

        MoveControl.Instance.CheckMoveSkipAndPreparePlayer();

        vault.SetNextStep();

        if (vault.IsOver) {
            string vaultMessage = Utils.Wrap("Копилка", UIColors.Green) + " исчерпана";
            Messages.Instance.AddMessage(vaultMessage);
        }
    }

    public void LeaveMoneybox(MoneyboxVault vault) {
        RemovePlayerFromMoneybox(vault);
        MoveControl.Instance.PreparePlayerForMove();
    }

    public void RemovePlayerFromMoneybox(MoneyboxVault vault) {
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " уходит из " + Utils.Wrap("копилки", UIColors.Green);
        Messages.Instance.AddMessage(message);
        vault.ReassignPlayers();
    }

    public void ExecuteCoinBonus(int bonus) {
        _player.AddCoins(bonus);
        PlayersControl.Instance.UpdatePlayersInfo();

        string effectText = bonus > 0 ? Utils.Wrap("бонус", UIColors.Green) : Utils.Wrap("штраф", UIColors.Red);
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " получает " + effectText + " " + bonus + " монет";
        Messages.Instance.AddMessage(message);
    }

    public void ExecuteWall(bool showMessage) {
        _player.IsDeadEndMode = true;

        if (showMessage) {
            string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " уткнулся носом в " + Utils.Wrap("стену", UIColors.Brick);
            Messages.Instance.AddMessage(message);
        }
    }

    // Молнии

    // Молния: При попадании на клетку с молнией

    public void ExecuteLightning(bool isTame) {
        int oldValue = _lightningMoves;
        _lightningMoves = isTame && oldValue == 0 ? 1 : 3;
        _isLightning = true;
        TokenControl token = _player.GetTokenControl();

        if (oldValue > 0) {
            token.UpdateIndicator(ETokenIndicators.Lightning, _lightningMoves.ToString());
        } else {
            token.AddIndicator(ETokenIndicators.Lightning, _lightningMoves.ToString());
        }

        string message;
        if (isTame) {
            message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " вызывает " + Utils.Wrap("ручную молнию", UIColors.Green) + "! Очки на кубике x2";
        } else {
            message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("молнию", UIColors.Green) + "! Очки на кубике x2";
        }
        Messages.Instance.AddMessage(message);
    }

    // Молния: При подготовке игрока к ходу (в т.ч. дополнительному)

    public void CheckLightningStartMove() {
        bool isBlot = _player.Boosters.IsBlot();

        if (_isLightning && !isBlot && _player.IsMe()) {
            CubicControl.Instance.ModifiersControl.ShowModifierLightning(true);
        }

        if (_isLightning && isBlot) {
            _player.Boosters.ExecuteBlotAsVictim("использовать молнию");
        }
    }

    // Молния: Сразу после кидания кубика или пропуска хода

    public void SpendLightning() {
        if (_isLightning) {
            _lightningMoves--;
        }
    }

    // Молния: В конце хода

    public void CheckLightningEndMove() {
        if (_player.IsMe()) {
            CubicControl.Instance.ModifiersControl.ShowModifierLightning(false);
        }

        if (!_isLightning) {
            return;
        }

        TokenControl token = _player.GetTokenControl();
        if (_lightningMoves == 0) {
            token.RemoveIndicator(ETokenIndicators.Lightning);
            string message = "У " + Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " закончилась " + Utils.Wrap("молния", UIColors.Green);
            Messages.Instance.AddMessage(message);
            _isLightning = false;
        } else {
            token.UpdateIndicator(ETokenIndicators.Lightning, _lightningMoves.ToString());
        }
    }

    // Телепорт

    public void ExecuteTeleport() {
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + Utils.Wrap(" телепортируется", UIColors.DarkBlue) + " на случайную клетку";
        Messages.Instance.AddMessage(message);
        StartCoroutine(ExecuteTeleportDefer());
    }

    private IEnumerator ExecuteTeleportDefer() {
        TokenControl token = _player.GetTokenControl();
        token.DisableIndicators(true);
        yield return new WaitForSeconds(EffectsControl.Instance.TeleportDelay);
        token.StartTeleportInAnimation();
        yield return new WaitForSeconds(TokensControl.Instance.TeleportAnimationTime);

        CellControl initialCell = _player.GetCurrentCell();
        CellControl newCell = CellsControl.Instance.GetRandomCellForTeleport(initialCell);

        token.SetToSpecifiedCell(newCell.gameObject, 10f, () => {
            StartCoroutine(ConfirmTeleportPositionDefer(token));
        });
    }

    private IEnumerator ConfirmTeleportPositionDefer(TokenControl token) {
        token.StartTeleportOutAnimation();
        yield return new WaitForSeconds(TokensControl.Instance.TeleportAnimationTime);
        token.DisableIndicators(false);
        StartCoroutine(MoveControl.Instance.ConfirmNewPositionDefer());
    }

    // Сюрприз

    public void ExecuteSurprise() {
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("сюрприз", UIColors.Violet);
        Messages.Instance.AddMessage(message);

        if (_player.IsAi()) {
            StartCoroutine(GenerateAndProcessSurpriseDefer());
        } else {
            _modalSurprise.BuildContent();
            _modalSurprise.OpenModal();
        }
    }

    private IEnumerator GenerateAndProcessSurpriseDefer() {
        yield return new WaitForSeconds(EffectsControl.Instance.AiSurpriseDelay);
        (
            ESurprise surpriseType,
            EControllableEffects surpriseEffect,
            EBoosters surpriseBooster,
            int surpriseCoinsBonus,
            int surpriseCoinsPenalty,
            int surpriseLevel
        ) = SurpriseGenerator.GenerateSurprise();
        ProcessGeneratedSurprise(
            surpriseType,
            surpriseEffect,
            surpriseBooster,
            surpriseCoinsBonus,
            surpriseCoinsPenalty,
            surpriseLevel
        );
    }

    public void ProcessGeneratedSurprise(
        ESurprise surpriseType,
        EControllableEffects surpriseEffect,
        EBoosters surpriseBooster,
        int surpriseCoinsBonus,
        int surpriseCoinsPenalty,
        int surpriseLevel
    ) {
        switch(surpriseType) {
            case ESurprise.InventoryEffect: {
                AddTheEffect(surpriseEffect, 1);
                ManualContent manual = Manual.Instance.GetEffectManual(surpriseEffect);
                _player.BonusProcessing(manual.GetEntityName(true));
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            case ESurprise.Booster: {
                _player.Boosters.AddTheBooster(surpriseBooster, 1);
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            case ESurprise.Bonus: {
                ExecuteCoinBonus(surpriseCoinsBonus);
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            case ESurprise.Penalty: {
                ExecuteCoinBonus(surpriseCoinsPenalty);
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            case ESurprise.Mallow: {
                _player.AddMallows(1);
                _player.BonusProcessing("зефирка");
                PlayersControl.Instance.UpdatePlayersInfo();
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            default: {
                ExecuteSurpriseEffect(surpriseType, surpriseLevel);
                break;
            }
        }
    }

    private void ExecuteSurpriseEffect(ESurprise effect, int level) {
        switch(effect) {
            case ESurprise.Green: {
                ExecuteGreen(level);
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            case ESurprise.Yellow: {
                ExecuteYellow(level);
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            case ESurprise.Black: {
                ExecuteBlack();
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            case ESurprise.Red: {
                CellControl currentCell = _player.GetCurrentCell();
                GameObject penaltyCell = CellsControl.Instance.FindPenaltyCell(currentCell.gameObject);
                ExecuteRed(level, penaltyCell);
                break;
            }
            case ESurprise.Star: {
                ExecuteStar(level);
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
            case ESurprise.Teleport: {
                ExecuteTeleport();
                break;
            }
            case ESurprise.Lightning: {
                ExecuteLightning(false);
                CellChecker.Instance.CheckCellArrows(_player);
                break;
            }
        }
    }

    // Перемещение эффекта

    public void ExecuteReplaceEffect(EControllableEffects effect) {
        ManualContent manual = Manual.Instance.GetEffectManual(effect);
        CellControl cell = _player.GetCurrentCell();
        int effectLevel = cell.EffectLevel;
        int cost = manual.GetCost(effectLevel);
        
        if (manual.CostResourceType == EResourceTypes.Power) {
            _player.AddPower(-cost);
        } else {
            _player.AddCoins(-cost);
        }

        PlayersControl.Instance.UpdatePlayersInfo();
        AddTheEffect(effect, -1);
        PlayersControl.Instance.CheckIsPlayerOutOfPower(_player);
    }

    // Разное

    public bool IsEnoughEffects(EControllableEffects effect) {
        switch(effect) {
            case EControllableEffects.Green: {
                return Green > 0;
            }
            case EControllableEffects.Yellow: {
                return Yellow > 0;
            }
            case EControllableEffects.Black: {
                return Black > 0;
            }
            case EControllableEffects.Red: {
                return Red > 0;
            }
            default: return false;
        }
    }

    public void AddTheEffect(EControllableEffects effect, int count) {
        switch(effect) {
            case EControllableEffects.Black: {
                AddBlack(count);
                break;
            }
            case EControllableEffects.Yellow: {
                AddYellow(count);
                break;
            }
            case EControllableEffects.Green: {
                AddGreen(count);
                break;
            }
            case EControllableEffects.Red: {
                AddRed(count);
                break;
            }
            case EControllableEffects.Star: {
                AddStar(count);
                break;
            }
        }

        EffectsControl.Instance.UpdateQuantityText(_player);
        EffectsControl.Instance.UpdateButtonsVisual(_player);
    }
}

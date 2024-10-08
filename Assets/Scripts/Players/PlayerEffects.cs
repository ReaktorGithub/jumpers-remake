using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]

public class PlayerEffects : MonoBehaviour
{
    private PlayerControl _player;
    private ModalWin _modalWin;
    private CameraControl _camera;
    private Pedestal _pedestal;
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
        _camera = GameObject.Find("VirtualCamera").GetComponent<CameraControl>();
        _pedestal = GameObject.Find("Pedestal").GetComponent<Pedestal>();
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

    public void ExecuteGreen() {
        ManualContent manual = Manual.Instance.GetEffectManual(EControllableEffects.Green);
        CellControl cell = _player.GetCurrentCell();
        int moves = manual.GetCauseEffect(cell.EffectLevel);
        int coins = cell.EffectLevel == 3 ? 50 : 0;

        _player.AddMovesToDo(moves);
        _player.AddCoins(coins);

        string movesMessage = moves == 1 ? "раз" : (moves + " раза");
        string coinsMessage = coins > 0 ? ". " + Utils.Wrap("Бонус", UIColors.Green) + " +" + coins + " монет!" : "";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("зелёный", UIColors.Green) + " эффект и походит ещё " + movesMessage + coinsMessage;
        Messages.Instance.AddMessage(message);
    }

    public void ExecuteYellow() {
        ManualContent manual = Manual.Instance.GetEffectManual(EControllableEffects.Yellow);
        CellControl cell = _player.GetCurrentCell();
        int coins = manual.GetCauseEffect(cell.EffectLevel);

        _player.SkipMoveIncrease();
        _player.AddCoins(coins);

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

    public void ExecuteRed() {
        _player.MovesToDo = 0;
        _player.StepsLeft = 0;

        if (_player.Boosters.Armor > 0 && _player.Boosters.IsIronArmor) {
            _player.OpenSavedByShieldModal(() => {
                string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("КРАСНЫЙ", UIColors.Red) + " эффект! Возврат на чекпойнт";
                Messages.Instance.AddMessage(message);
                RedEffectTokenMove();
            });
            return;
        }

        CellControl cell = _player.GetCurrentCell();
        ManualContent manual = Manual.Instance.GetEffectManual(EControllableEffects.Red);
        int powerPenalty = manual.GetCauseEffect(cell.EffectLevel);

        _player.AddPower(-powerPenalty);
        PlayersControl.Instance.UpdatePlayersInfo();

        string powerText = powerPenalty == 1 ? "сила" : "силы";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("КРАСНЫЙ", UIColors.Red) + " эффект! -" + powerPenalty + " " + powerText + ". Возврат на чекпойнт";
        Messages.Instance.AddMessage(message);

        PlayersControl.Instance.CheckIsPlayerOutOfPower(_player, RedEffectTokenMove, () => StartCoroutine(RedEffectTokenMoveDefer()));
    }

    private IEnumerator RedEffectTokenMoveDefer() {
        yield return new WaitForSeconds(PlayersControl.Instance.RedEffectDelay);
        RedEffectTokenMove();
    }

    private void RedEffectTokenMove() {
        TokenControl tokenControl = _player.GetTokenControl();
        CellControl cellControl = tokenControl.GetCurrentCellControl();
        if (!tokenControl.CurrentCell.TryGetComponent(out RedCell redCell)) {
            Debug.Log("Red cell not found");
            return;
        }
        float moveTime = MoveControl.Instance.SpecifiedMoveTime;
        tokenControl.SetToSpecifiedCell(redCell.PenaltyCell, moveTime, () => {
            cellControl.RemoveToken(_player.TokenObject);
            MoveControl.Instance.ConfirmNewPosition();
        });
    }

    public void ExecuteStar() {
        CellControl cell = _player.GetCurrentCell();
        ManualContent manual = Manual.Instance.GetEffectManual(EControllableEffects.Star);
        int powerBonus = manual.GetCauseEffect(cell.EffectLevel);

        string powerText = powerBonus == 1 ? " силу" : " силы";
        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попадает на " + Utils.Wrap("звезду", UIColors.DarkBlue) + " и получает " + powerBonus + powerText;
        Messages.Instance.AddMessage(message);

        bool isLucky = cell.EffectLevel == 3;
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

        // todo нужно разбрасывать дань по полю
        ExecuteFinish();
    }

    public void ExecuteMoneybox(MoneyboxVault vault) {
        (int, int, int) bonus = vault.GetBonus();
        _player.AddPower(bonus.Item1);
        _player.AddCoins(bonus.Item2);
        _player.AddRubies(bonus.Item3);
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
        _player.AddMovesToDo(1);

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " покидает " + Utils.Wrap("копилку", UIColors.Green);
        Messages.Instance.AddMessage(message);

        MoveControl.Instance.PreparePlayerForMove();
        
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

    public void ExecuteLightning() {
        int oldValue = _lightningMoves;
        _lightningMoves = 3;
        _isLightning = true;
        TokenControl token = _player.GetTokenControl();

        if (oldValue > 0) {
            token.UpdateIndicator(ETokenIndicators.Lightning, _lightningMoves.ToString());
        } else {
            token.AddIndicator(ETokenIndicators.Lightning, _lightningMoves.ToString());
        }

        string message = Utils.Wrap(_player.PlayerName, UIColors.Yellow) + " попал на " + Utils.Wrap("молнию", UIColors.Green) + "! Очки на кубике x2";
        Messages.Instance.AddMessage(message);
    }

    // Молния: При подготовке игрока к ходу (в т.ч. дополнительному)

    public void CheckLightningStartMove() {
        if (_isLightning && _player.IsMe()) {
            CubicControl.Instance.ModifiersControl.ShowModifierLightning(true);
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

        switch(effect) {
            case EControllableEffects.Green: {
                AddGreen(-1);
                break;
            }
            case EControllableEffects.Red: {
                AddRed(-1);
                break;
            }
            case EControllableEffects.Yellow: {
                AddYellow(-1);
                break;
            }
            case EControllableEffects.Black: {
                AddBlack(-1);
                break;
            }
        }

        PlayersControl.Instance.CheckIsPlayerOutOfPower(_player);
    }

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
}

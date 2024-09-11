using System.Collections.Generic;
using UnityEngine;

public static class AiLib 
{
    private static int _exclude = -100000; // число, исключающее возможность события

    // Анализирует ситуацию текущего игрока: учитывает оставшиеся силы и расстояние до финиша

    public static int GetPointsByPowerAndDistance(PlayerControl player, GameObject currentCell) {
        int points = 0;

        int distance = CellsControl.Instance.GetStepsToFinish(currentCell);

        switch(player.AiType) {
            case EAiTypes.Normal: {
                if (player.Power > 2) points += 10;
                if (player.Power > 0 && player.Power < 3) points += 5;
                if (distance < 15) points += 5;
                break;
            }
            case EAiTypes.Risky: {
                if (player.Power > 0) points += 10;
                break;
            }
            case EAiTypes.Careful: {
                if (player.Power > 3) points += 10;
                if (player.Power > 1 && player.Power < 4) points += 5;
                if (distance < 15) points += 5;
                break;
            }
            default: {
                points = Utils.GetRandomDecision() ? 10 : 0;
                break;
            }
        }

        return points;
    }

    /*
        Ценность обычной атаки
        Учесть силы в запасе и число шагов до финиша
    */

    public static int GetAttackUsualPoints(PlayerControl player) {
        int needPower = Manual.Instance.AttackUsual.GetCost(1); // todo
        int restPower = player.Power - needPower;

        if (restPower < 0) {
            return _exclude;
        }

        CellControl cell = player.GetCurrentCell();
        return GetPointsByPowerAndDistance(player, cell.gameObject);
    }

    /*
        Ценность атаки волшебного пинка
        Учесть кол-во шагов на которое сместится соперник
        Учесть aiScore клетки, на которую попадет соперник
        Учесть силы в запасе и число шагов до финиша
    */

    public static int GetAttackMagicKickPoints(PlayerControl player) {
        int needPower = Manual.Instance.AttackMagicKick.GetCost(1); // todo
        int restPower = player.Power - needPower;

        if (!player.AvailableAttackTypes.Contains(EAttackTypes.MagicKick) || restPower < 0) {
            return _exclude;
        }

        CellControl cell = player.GetCurrentCell();
        int initialSteps = Manual.Instance.AttackMagicKick.GetCauseEffect(2); // todo
        (GameObject, int) cellResult = CellsControl.Instance.FindCellBySteps(cell.gameObject, false, initialSteps);

        int steps = initialSteps - cellResult.Item2;
        CellControl targetCell = cellResult.Item1.GetComponent<CellControl>();

        int points = -targetCell.AiScore; // инвертируем показатель (теперь минус это хорошо, а плюс это плохо)
        int analysePoints = GetPointsByPowerAndDistance(player, cell.gameObject);
        if (steps < 1) points -= 10;
        if (steps > 4) points += 10;

        return points + analysePoints;
    }

    /*
        Ценность атаки вампиром
        Учесть силы каждого соперника
        Учесть силы в запасе и число шагов до финиша
    */

    public static (PlayerControl, int) GetAttackVampirePoints(PlayerControl player) {
        if (player.BoosterVampire < 1) {
            return (null, _exclude);
        }

        CellControl cell = player.GetCurrentCell();
        int distance = CellsControl.Instance.GetStepsToFinish(cell.gameObject);

        int points = 0;

        switch(player.AiType) {
            case EAiTypes.Normal:
            case EAiTypes.Careful: {
                if (player.Power < 2) points += 10;
                if (distance < 15) points += 5;
                break;
            }
            case EAiTypes.Risky: {
                if (player.Power < 3) points += 10;
                break;
            }
            default: {
                points = Utils.GetRandomDecision() ? 10 : 0;
                break;
            }
        }

        // Найти соперников с силой меньше 2 - они должны быть атакованы с большой вероятностью

        List<PlayerControl> rivals = new();

        foreach(GameObject token in cell.CurrentTokens) {
            PlayerControl foundPlayer = token.GetComponent<TokenControl>().PlayerControl;
            if (foundPlayer != player && foundPlayer.Power < 2) {
                rivals.Add(foundPlayer);
            }
        }

        PlayerControl selectedPlayer = null;

        if (rivals.Count > 0) {
            selectedPlayer = PlayersControl.Instance.GetMostSuccessfulPlayer(rivals);
            int add = selectedPlayer.Power == 0 ? 20 : (selectedPlayer.Power == 1 ? 10 : 0);
            points += add;
        }

        return (selectedPlayer, points);
    }

    /*
        Ценность атаки нокаутом
        Учесть силы в запасе и дистанцию до финиша
    */

    public static int GetAttackKnockoutPoints(PlayerControl player) {
        int needPower = Manual.Instance.AttackKnockout.GetCost(1); // todo
        int restPower = player.Power - needPower;

        if (!player.AvailableAttackTypes.Contains(EAttackTypes.Knockout) || restPower < 0) {
            return _exclude;
        }
        
        CellControl cell = player.GetCurrentCell();
        int distance = CellsControl.Instance.GetStepsToFinish(cell.gameObject);

        int points = 0;

        switch(player.AiType) {
            case EAiTypes.Normal: {
                if (restPower < 1) points -= 5;
                if (restPower > 0 && restPower < 3) points += 5;
                if (restPower > 2) points += 10;
                if (distance < 10) points += 5;
                break;
            }
            case EAiTypes.Risky: {
                if (restPower == 1) points += 5;
                if (restPower > 1) points += 10;
                if (distance < 15) points += 5;
                break;
            }
            case EAiTypes.Careful: {
                if (restPower < 2) points -= 5;
                if (restPower > 1 && restPower < 4) points += 5;
                if (restPower > 3) points += 10;
                if (distance < 10) points += 5;
                break;
            }
            default: {
                points = Utils.GetRandomDecision() ? 10 : 0;
                break;
            }
        }

        return points;
    }

    public static int GetBranchVariantPoints(PlayerControl player, BranchButton branchButton, CellControl cell) {
        int points = cell.AiScore;

        if (cell.IsPenaltyEffect() && player.Power <= 1) points -= 80;

        if (branchButton.AiBranchType == EAiBranchTypes.Tasty) points += 10;
        if (branchButton.AiBranchType == EAiBranchTypes.Dirty) points -= 10;

        switch(player.AiType) {
            case EAiTypes.Risky: {
                if (branchButton.AiBranchType == EAiBranchTypes.Risky) points += 10;
                if (branchButton.AiBranchType == EAiBranchTypes.Careful) points -= 10;
                break;
            }
            case EAiTypes.Careful: {
                if (branchButton.AiBranchType == EAiBranchTypes.Risky) points -= 10;
                if (branchButton.AiBranchType == EAiBranchTypes.Careful) points += 10;
                break;
            }
        }

        if (player.Power > 2 && cell.CurrentTokens.Count > 0) points += 8;

        return points;
    }
}

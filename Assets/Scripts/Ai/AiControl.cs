using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AiControl : MonoBehaviour
{
    public static AiControl Instance { get; private set; }
    [SerializeField] bool _enableAi = true;
    [SerializeField] float _cubicThrowDelay = 1f;
    [SerializeField] float _thinkingDelay = 2f;
    private PopupAttack _popupAttack;

    private void Awake() {
        Instance = this;
        _popupAttack = GameObject.Find("GameScripts").GetComponent<PopupAttack>();
    }

    public bool EnableAi {
        get { return _enableAi; }
        private set {}
    }

    private IEnumerator ImitateThinking(PlayerControl player, Action callback) {
        TokenControl token = player.GetTokenControl();
        token.ShowAi(true);
        yield return new WaitForSeconds(_thinkingDelay);
        token.ShowAi(false);
        callback.Invoke();
    }

    public IEnumerator AiThrowCubic() {
        yield return new WaitForSeconds(_cubicThrowDelay);
        CubicControl.Instance.OnCubicClick();
    }

    /*
        1. Выбрать вид атаки todo
        2. Атаковать или нет?
            - Учесть число шагов до финиша
            - Учесть силы в запасе
        3. Решить, кого атаковать
            - Выбрать наиболее сильного игрока
    */

    public void AiAttackPlayer(PlayerControl player, List<PlayerControl> rivals) {
        CellControl cell = player.GetCurrentCell();

        int points = 0; // trigger by 10

        int distance = CellsControl.Instance.GetStepsToFinish(cell.gameObject);

        switch(player.AiType) {
            case EAiTypes.Normal: {
                if (player.Power > 2) points += 10;
                if (player.Power > 0) points += 5;
                if (distance < 15) points += 5;
                break;
            }
            case EAiTypes.Risky: {
                if (player.Power > 0) points += 10;
                break;
            }
            case EAiTypes.Careful: {
                if (player.Power > 3) points += 10;
                if (player.Power > 1) points += 5;
                if (distance < 15) points += 5;
                break;
            }
            default: {
                points = Utils.GetRandomDecision() ? 10 : 0;
                break;
            }
        }

        StartCoroutine(ImitateThinking(player, () => {
            if (points >= 10) {
                PlayerControl selectedPlayer = PlayersControl.Instance.GetMostSuccessfulPlayer(rivals);
                _popupAttack.SetSelectedPlayer(selectedPlayer);
                _popupAttack.ConfirmAttack();
            } else {
                _popupAttack.CancelAttack();
            }
            Debug.Log("Отчет Ai о решении атаки: Ai type: " + player.AiType + "; " + "Points: " + points + "; Trigger by: 10");
        }));
    }

    /*
        1. Собрать клетки, на которые возможно приземлиться, с привязкой к конкретной кнопке
        2. Узнать ценность каждой ветки
        3. Сопоставить данные и принять решение
    */

    public void AiSelectBranch(PlayerControl player, BranchControl branch, int rest) {
        if (player.AiType == EAiTypes.Random) {
            System.Random random = new();
            int index = random.Next(0, branch.BranchButtonsList.Count);
            BranchButton branchButton = branch.BranchButtonsList[index].GetComponent<BranchButton>();
            StartCoroutine(ImitateThinking(player, () => {
                branchButton.ConfirmNewDirection();
            }));
            return;
        }

        List<(BranchButton, int)> variants = new(); // branchButton и очки ai

        foreach(GameObject button in branch.BranchButtonsList) {
            BranchButton branchButton = button.GetComponent<BranchButton>();
            (GameObject, int) cells = CellsControl.Instance.FindCellBySteps(branchButton.NextCell, !branch.IsReverse, rest);
            GameObject targetCell = cells.Item1;
            bool isBranch = targetCell.TryGetComponent(out BranchCell _);
            if (targetCell == null) {
                variants.Add((branchButton, 0));
            } else if (targetCell.TryGetComponent(out BranchCell _)) {
                variants.Add((branchButton, 0));
            } else {
                int points = GetBranchVariantPoints(player, branchButton, targetCell.GetComponent<CellControl>());
                variants.Add((branchButton, points));
            }
        }

        // debug

        string message = "Ai type: " + player.AiType + "; ";
        foreach((BranchButton, int) variant in variants) {
            message += "Branch cell: " + variant.Item1.NextCell.name + ", Points: " + variant.Item2 + "; ";
        }
        Debug.Log("Отчет Ai о решении бранча: " + message);

        // выбрать лучший вариант

        BranchButton selectedBranch = SelectBranchVariantWithMaxInt(variants);
        StartCoroutine(ImitateThinking(player, () => {
            selectedBranch.ConfirmNewDirection();
        }));
    }

    private int GetBranchVariantPoints(PlayerControl player, BranchButton branchButton, CellControl cell) {
        if (cell.CellType == ECellTypes.Finish) return 1000;

        bool isPenaltyEffect = cell.Effect == EControllableEffects.Red || cell.Effect == EControllableEffects.Black;

        if (isPenaltyEffect && player.Power > 1) return -20;
        if (isPenaltyEffect && player.Power <= 1) return -100;

        int points = 0;

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

        if (cell.CellType == ECellTypes.Lightning || cell.CellType == ECellTypes.Moneybox || cell.CellType == ECellTypes.Surprise || cell.Effect == EControllableEffects.Green) points += 8;
        if (cell.Effect == EControllableEffects.Star) points += 12;
        if (player.Power > 2 && cell.CurrentTokens.Count > 0) points += 8;
        if (cell.Effect == EControllableEffects.Yellow) points -= 8;

        return points;
    }

    private BranchButton SelectBranchVariantWithMaxInt(List<(BranchButton, int)> list) {
        if (list.Count == 0) {
            throw new ArgumentException("Список не может быть пустым.");
        }

        int maxInt = list.Max(x => x.Item2);

        // Фильтруем список, оставляя только элементы с максимальным int
        var candidates = list.Where(x => x.Item2 == maxInt).ToList();

        // Выбираем случайный элемент из кандидатов
        System.Random random = new();
        int randomIndex = random.Next(candidates.Count);

        return candidates[randomIndex].Item1;
    }
}

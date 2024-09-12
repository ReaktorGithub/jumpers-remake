using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Публичные методы писать с приставкой Ai

public class AiControl : MonoBehaviour
{
    public static AiControl Instance { get; private set; }
    [SerializeField] bool _enableAi = true;
    [SerializeField] float _cubicThrowDelay = 1f;
    [SerializeField] float _thinkingDelay = 2f;
    [SerializeField] int _cubicScore = 0;

    private void Awake() {
        Instance = this;
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
        CubicControl.Instance.OnCubicClick(_cubicScore);
    }

    /*
        1. Пройтись по каждому виду атаки и выяснить, возможно ли ее применить
        2. Если применить атаку возможно, то вычислить очки
        3. Если в процессе вычисления выявился предпочитаемый соперник, то запомнить его
        4. Выбрать виды атак, у которых очков 10 или больше
        5. Выбрать одну из этих атак
        6. Решить, кого атаковать
            - Если нашелся предпочитаемый соперник в соответствии с выбранным типом атаки, то выбрать его
            - В противном случае выбрать наиболее сильного игрока
        7. Атаки не произойдет, если ни одна атака не набрала 10 или более очков
    */

    public void AiAttackPlayer(PlayerControl player, List<PlayerControl> rivals) {
        int attackUsualPoints = AiLib.GetAttackUsualPoints(player);
        int attackMagicKickPoints = AiLib.GetAttackMagicKickPoints(player);
        (PlayerControl preferedPlayer, int attackVampirePoints) = AiLib.GetAttackVampirePoints(player, rivals);
        int attackKnockout = AiLib.GetAttackKnockoutPoints(player);

        Debug.Log("Отчет Ai о решении атаки: Ai type: " + player.AiType + "; " + "Usual: " + attackUsualPoints + "; Magic kick: " + attackMagicKickPoints + "; Vampire: " + attackVampirePoints + "; Knockout: " + attackKnockout + "; Trigger by 10");
        if (preferedPlayer != null) {
            Debug.Log("Prefered vampire: " + preferedPlayer.PlayerName);
        }

        List<(EAttackTypes, int)> analyseResult = new() {
            (EAttackTypes.Usual, attackUsualPoints),
            (EAttackTypes.MagicKick, attackMagicKickPoints),
            (EAttackTypes.Vampire, attackVampirePoints),
            (EAttackTypes.Knockout, attackKnockout),
        };

        (EAttackTypes type, int points) = Utils.GetMostValuableElement(analyseResult);

        StartCoroutine(ImitateThinking(player, () => {
            if (points >= 10) {
                PlayerControl selectedPlayer;
                if (type == EAttackTypes.Vampire && preferedPlayer != null) {
                    selectedPlayer = preferedPlayer;
                } else {
                    selectedPlayer = PlayersControl.Instance.GetMostSuccessfulPlayer(rivals);
                }
                Debug.Log("selected attack: " + type + "; selectedPlayer: " + selectedPlayer);
                
                player.ExecuteAttack(type, selectedPlayer);
            } else {
                player.ExecuteCancelAttack();
            }
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
                int points = AiLib.GetBranchVariantPoints(player, branchButton, targetCell.GetComponent<CellControl>());
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

        (BranchButton selectedBranch, _) = Utils.GetMostValuableElement(variants);
        StartCoroutine(ImitateThinking(player, () => {
            selectedBranch.ConfirmNewDirection();
        }));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiControl : MonoBehaviour
{
    public static AiControl Instance { get; private set; }
    [SerializeField] float _cubicThrowDelay = 1f;
    [SerializeField] float _thinkingDelay = 2f;

    private void Awake() {
        Instance = this;
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
        CubicControl.Instance.OnCubicClick(1);
    }

    /*
        1. Решить, кого атаковать
            - Выбрать наиболее сильного игрока
        2. Выбрать вид атаки TODO
        3. Атаковать или нет?
            - Учесть число шагов до финиша
            - Учесть силы в запасе
    */

    public void AiAttackPlayer(PlayerControl player, CellControl currentCell, List<PlayerControl> rivals) {
        PlayerControl selectedPlayer = PlayersControl.Instance.GetMostSuccessfulPlayer(rivals);
        int distance = CellsControl.Instance.GetStepsToFinish(currentCell.gameObject);

        int points = 0; // trigger by 10

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
            Debug.Log("Ai");
            // todo нужно изолировать методы из popupAttack
        }));
    }
}

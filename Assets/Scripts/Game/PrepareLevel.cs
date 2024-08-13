using UnityEngine;

public class PrepareLevel : MonoBehaviour
{
    [SerializeField] private float moveToStartTime = 3.3f;
    [SerializeField] private GameObject startCell;

    private void Start() {
        // порядок важен
        PlayersControl.Instance.PrepareTokenLayerOrder();

        int currentPlayerIndex = MoveControl.Instance.CurrentPlayerIndex;
        PlayersControl.Instance.UpdatePlayersInfo(currentPlayerIndex);
        PlayersControl.Instance.GiveEffectsBeforeRace(currentPlayerIndex);

        MoveTokensToStart();
        
        PlayersControl.Instance.UpdateSqueezeAnimation(currentPlayerIndex);

        MoveControl.Instance.SwitchPlayer();
        
        string message = Utils.Wrap("ГОНКА НАЧАЛАСЬ!", UIColors.Yellow);
        Messages.Instance.AddMessage(message);
    }

    public void MoveTokensToStart() {
        CellControl startCellControl = startCell.GetComponent<CellControl>();
        startCellControl.AddToken("token_1");
        startCellControl.AddToken("token_2");
        startCellControl.AddToken("token_3");
        startCellControl.AddToken("token_4");
        PlayersControl.Instance.UpdateTokensCurrentCell(startCell);
        startCellControl.AlignTokens(moveToStartTime);
    }
}

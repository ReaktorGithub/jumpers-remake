using UnityEngine;

public class PrepareLevel : MonoBehaviour
{
    [SerializeField] private float _moveToStartTime = 3.3f;
    [SerializeField] private GameObject _startCell;

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
        CellControl startCellControl = _startCell.GetComponent<CellControl>();
        foreach(PlayerControl player in PlayersControl.Instance.Players) {
            startCellControl.AddToken(player.TokenObject);
        }
        PlayersControl.Instance.UpdateTokensCurrentCell(_startCell);
        startCellControl.AlignTokens(_moveToStartTime);
        // CellsControl.Instance.ShowTokensAtCells();
    }
}

using UnityEngine;

public class PrepareLevel : MonoBehaviour
{
    [SerializeField] private float _moveToStartTime = 3.3f;
    [SerializeField] private GameObject _startCell;
    private LevelData _levelData;

    private void Awake() {
        _levelData = GameObject.Find("LevelScripts").GetComponent<LevelData>();
    }

    private void Start() {
        // порядок важен
        _levelData.SetUIPrizeList();
        _levelData.SetInitialRandomBonuses();
        
        PlayersControl.Instance.PrepareTokenLayerOrder();
        PlayersControl.Instance.CreatePlayersInfo();
        PlayersControl.Instance.GiveEffectsBeforeRace();

        MoveTokensToStart();
        
        PlayersControl.Instance.UpdateSqueezeAnimation();

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

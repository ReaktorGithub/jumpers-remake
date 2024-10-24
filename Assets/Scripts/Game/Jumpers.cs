using UnityEngine;

// Класс предназначен для предварительной настройки игры перед запуском уровня

public class Jumpers : MonoBehaviour
{
    // private GameObject _testArrow, _arrowBody;

    // private void Awake() {
    //     _testArrow = GameObject.Find("ArrowHedge1d");
    //     _arrowBody = GameObject.Find("arrow-body");
    // }

    private void Start() {
        // CleanInstances();
        PlayersControl.Instance.BindTokensToPlayers();
        PlayersControl.Instance.UpdateAllIndicators();
        GameObject Instances = GameObject.Find("Instances");
        GameObject CanvasInstances = GameObject.Find("CanvasInstances");
        Instances.SetActive(false);
        CanvasInstances.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.F)) {
            MoveControl.Instance.CurrentPlayer.IsAbilitySoap = !MoveControl.Instance.CurrentPlayer.IsAbilitySoap;
        }
    }

    // private void Update() {
    //     if (Input.GetKeyUp(KeyCode.U)) {
    //         _arrowBody.SetActive(true);
    //         _testArrow.SetActive(!_testArrow.activeInHierarchy);
    //         _arrowBody.SetActive(false);
    //     }
    // }

    // private void Update() {
    //     if (Input.GetKeyUp(KeyCode.B)) {
    //         GameObject cell = GameObject.Find("c4");
    //         List<CellControl> list = CellsControl.Instance.GetCellsInArea(cell.GetComponent<CellControl>(), 1);
    //         Debug.Log("list count: " + list.Count);
    //         foreach(CellControl found in list) {
    //             Debug.Log("found: " + found.gameObject.name);
    //         }
    //     }
    // }

    // private void Update() {
    //     if (Input.GetKeyUp(KeyCode.Z)) {
    //         MoveControl.Instance.CurrentPlayer.Boosters.AddMagnets(1);
    //         BoostersControl.Instance.UpdateBoostersFromPlayer(MoveControl.Instance.CurrentPlayer);
    //     }
    //     if (Input.GetKeyUp(KeyCode.X)) {
    //         MoveControl.Instance.CurrentPlayer.Boosters.AddMagnetsSuper(1);
    //         BoostersControl.Instance.UpdateBoostersFromPlayer(MoveControl.Instance.CurrentPlayer);
    //     }
    //     if (Input.GetKeyUp(KeyCode.C)) {
    //         MoveControl.Instance.CurrentPlayer.Boosters.AddShield(1);
    //         BoostersControl.Instance.UpdateBoostersFromPlayer(MoveControl.Instance.CurrentPlayer);
    //     }
    //     if (Input.GetKeyUp(KeyCode.V)) {
    //         MoveControl.Instance.CurrentPlayer.Boosters.AddShieldIron(1);
    //         BoostersControl.Instance.UpdateBoostersFromPlayer(MoveControl.Instance.CurrentPlayer);
    //     }
    // }
}

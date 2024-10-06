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
        CleanInstances();
        PlayersControl.Instance.BindTokensToPlayers();
    }

    private void CleanInstances() {
        GameObject Instances = GameObject.Find("Instances");
        Instances.transform.Find("arrow-body").gameObject.SetActive(false);
        Instances.transform.Find("arrow-body-tax").gameObject.SetActive(false);
        Instances.transform.Find("magnet").gameObject.SetActive(false);
        Instances.transform.Find("magnet-super").gameObject.SetActive(false);
        Instances.transform.Find("boombaster-icon").gameObject.SetActive(false);
        Instances.transform.Find("lasso").gameObject.SetActive(false);
        Instances.transform.Find("shield").gameObject.SetActive(false);
        Instances.transform.Find("shield-iron").gameObject.SetActive(false);
        Instances.transform.Find("vampire").gameObject.SetActive(false);
        Instances.transform.Find("BoosterButtonPick").gameObject.SetActive(false);
        Instances.transform.Find("TokenIndicator").gameObject.SetActive(false);
        Instances.transform.Find("TokenBonusEvent").gameObject.SetActive(false);
        Instances.transform.Find("lightning").gameObject.SetActive(false);
        Instances.transform.Find("grind-dash2").gameObject.SetActive(false);
        Instances.transform.Find("grind-dash3").gameObject.SetActive(false);
        Instances.transform.Find("star-icon").gameObject.SetActive(false);
        Instances.transform.Find("Boombaster").gameObject.SetActive(false);
        Instances.transform.Find("stuck-icon").gameObject.SetActive(false);
        Instances.transform.Find("trap-icon").gameObject.SetActive(false);
    }

    // private void Update() {
    //     if (Input.GetKeyUp(KeyCode.F)) {
    //         MoveControl.Instance.CurrentPlayer.StuckAttached = 4;
    //         ModalStuck modal = GameObject.Find("ModalScripts").GetComponent<ModalStuck>();
    //         modal.OpenModal();
    //     }
    // }

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

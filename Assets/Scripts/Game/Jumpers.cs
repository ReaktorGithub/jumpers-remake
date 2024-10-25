using UnityEngine;

public class Jumpers : MonoBehaviour
{
    [SerializeField] private GameObject _ui, _field;

    private void Start() {
        PlayersControl.Instance.BindTokensToPlayers();
        PlayersControl.Instance.UpdateAllIndicators();
        GameObject Instances = GameObject.Find("Instances");
        GameObject CanvasInstances = GameObject.Find("CanvasInstances");
        Instances.SetActive(false);
        CanvasInstances.SetActive(false);
    }

    public void ShowUI(bool value) {
        _ui.SetActive(value);
    }

    public void ShowField(bool value) {
        _field.SetActive(value);
    }

    // private void Update() {
    //     if (Input.GetKeyUp(KeyCode.F)) {
    //         MoveControl.Instance.CurrentPlayer.IsAbilitySoap = !MoveControl.Instance.CurrentPlayer.IsAbilitySoap;
    //     }
    // }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.G)) {
            ShowUI(false);
            ShowField(false);
            ModalGarage modal = GameObject.Find("ModalScripts").GetComponent<ModalGarage>();
            PlayerControl player = PlayersControl.Instance.GetMe();
            GarageControl.Instance.BuildContent(player);
            modal.OpenModal();
        }

        if (Input.GetKeyUp(KeyCode.H)) {
            ModalGarage modal = GameObject.Find("ModalScripts").GetComponent<ModalGarage>();
            modal.CloseModal();
            ShowUI(true);
            ShowField(true);
        }
    }

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

using UnityEngine;

public class Pedestal : MonoBehaviour
{
    private bool[] _pedestal = new bool[4];
    private PedestalPlace _place1, _place2, _place3, _place4;
    

    private void Awake() {
        _place1 = transform.Find("place1").gameObject.GetComponent<PedestalPlace>();
        _place2 = transform.Find("place2").gameObject.GetComponent<PedestalPlace>();
        _place3 = transform.Find("place3").gameObject.GetComponent<PedestalPlace>();
        _place4 = transform.Find("place4").gameObject.GetComponent<PedestalPlace>();
    }

    private void Start() {
        CleanPedestal();
    }

    public int SetPlayerToMaxPlace(PlayerControl player) {
        for (int i = 0; i < _pedestal.Length; i++) {
            if (_pedestal[i] == false) {
                _pedestal[i] = true;
                player.PlaceAfterFinish = i + 1;
                return i + 1;
            }
        }
        return 0;
    }

    public int SetPlayerToMinPlace(PlayerControl player) {
        for (int i = _pedestal.Length - 1; i >= 0; i--) {
            if (_pedestal[i] == false) {
                _pedestal[i] = true;
                player.PlaceAfterFinish = i + 1;
                return i + 1;
            }
        }
        return 0;
    }

    public void SetTokenToPedestal(PlayerControl player, int place) {
        switch (place) {
            case 1:
            _place1.SetPlayer(player);
            break;
            case 2:
            _place2.SetPlayer(player);
            break;
            case 3:
            _place3.SetPlayer(player);
            break;
            case 4:
            _place4.SetPlayer(player);
            break;
        }
        player.TokenObject.SetActive(false); 
    }

    public void CleanPedestal() {
        _pedestal[0] = false;
        _pedestal[1] = false;
        _pedestal[2] = false;
        _pedestal[3] = false;
        _place1.CleanPedestalVisual();
        _place2.CleanPedestalVisual();
        _place3.CleanPedestalVisual();
        _place4.CleanPedestalVisual();
    }
}

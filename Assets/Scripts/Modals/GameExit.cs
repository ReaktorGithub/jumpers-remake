using UnityEngine;

public class GameExit : MonoBehaviour
{
    private GameObject _modal;

    private void Awake() {
        _modal = GameObject.Find("ModalExitGame");
        _modal.SetActive(false);
    }

    public void ExitApplication() {
        Application.Quit();
    }

    private void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("escape");
            _modal.SetActive(true);
        }
    }
}

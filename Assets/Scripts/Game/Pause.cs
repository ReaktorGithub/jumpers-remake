using UnityEngine;

public class Pause : MonoBehaviour
{
    public void Update() {
        if (Input.GetKeyUp(KeyCode.P)) {
            PauseGame();
        }
    }

    public void PauseGame() {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
}

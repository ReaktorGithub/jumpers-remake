using UnityEngine;

public class Pause : MonoBehaviour
{
    public void PauseGame() {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }
}

using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _imagePause;
    [SerializeField] private GameObject _imagePlay;
    [SerializeField] private GameObject _pauseBlock;
    [SerializeField] private float _timeScale = 0;

    private void Awake() {
        _imagePause.SetActive(true);
        _imagePlay.SetActive(false);
        _pauseBlock.SetActive(false);
    }

    public void Update() {
        if (Input.GetKeyUp(KeyCode.P)) {
            PauseGame();
        }
    }

    public void PauseGame() {
        bool isPause = Time.timeScale == _timeScale;
        Time.timeScale = isPause ? 1 : _timeScale;
        _imagePause.SetActive(isPause);
        _imagePlay.SetActive(!isPause);
        _pauseBlock.SetActive(!isPause);
    }
}

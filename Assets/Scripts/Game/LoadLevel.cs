using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private string _nextLevel;

    public void OnLoadLevel() {
        SceneManager.LoadScene(_nextLevel);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    [SerializeField] private string nextLevel;

    public void OnLoadLevel() {
        SceneManager.LoadScene(nextLevel);
    }
}

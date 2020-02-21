using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayGame : MonoBehaviour
{
    public void OnPlayGameButtonClicked()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(1);
    }
}

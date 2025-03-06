using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void RestartGame()
    {
        GameController.Instance.IsRestart = true;
    }

    public void ToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}

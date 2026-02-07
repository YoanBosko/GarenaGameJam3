using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.TogglePause();
            pauseUI.SetActive(GameManager.IsPaused);
        }
    }

    // ▶️ CONTINUE (RESUME GAME)
    public void ContinueGame()
    {
        GameManager.ResumeGame();
        pauseUI.SetActive(false);
    }

    // 🔄 RESTART SCENE
    public void RestartScene()
    {
        GameManager.ResumeGame();
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // ⏭️ NEXT SCENE
    public void LoadNextScene()
    {
        GameManager.ResumeGame();
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1
        );
    }

    // 🏠 MAIN MENU
    public void LoadMainMenu()
    {
        GameManager.ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }
}

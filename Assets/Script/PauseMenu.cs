using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.TogglePause();
            pauseUI.SetActive(GameManager.Instance.IsPaused);
        }
    }

    // ▶️ CONTINUE
    public void ContinueGame()
    {
        GameManager.Instance.ResumeGame();
        pauseUI.SetActive(false);
    }

    // 🔄 RESTART
    public void RestartScene()
    {
        GameManager.Instance.ResumeGame();
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // ⏭️ NEXT SCENE
    public void LoadNextScene()
    {
        GameManager.Instance.ResumeGame();
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1
        );
    }

    // 🏠 MAIN MENU
    public void LoadMainMenu()
    {
        GameManager.Instance.ResumeGame();
        SceneManager.LoadScene("MainMenu");
    }
}

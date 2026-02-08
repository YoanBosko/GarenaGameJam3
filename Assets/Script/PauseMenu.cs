using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    public string mainMenuSceneName = "MainMenu";

    private struct InitialTransform
    {
        public Transform transform;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    private List<InitialTransform> childData = new List<InitialTransform>();

    void Start()
    {
        pauseUI.SetActive(false);

        if (pauseUI != null)
        {
            foreach (Transform child in pauseUI.transform)
            {
                childData.Add(new InitialTransform
                {
                    transform = child,
                    position = child.localPosition,
                    rotation = child.localRotation,
                    scale = child.localScale
                });
            }
        }
    }

    void Update()
    {
        // ❌ ESC MATI TOTAL SAAT GAME OVER
        if (GameManager.Instance.IsGameOver)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        GameManager.Instance.TogglePause();

        bool isPaused = GameManager.Instance.IsPaused;

        if (isPaused)
            ResetChildPositions();

        pauseUI.SetActive(isPaused);
    }

    private void ResetChildPositions()
    {
        foreach (var data in childData)
        {
            if (data.transform != null)
            {
                data.transform.localPosition = data.position;
                data.transform.localRotation = data.rotation;
                data.transform.localScale = data.scale;
            }
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
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ⏭️ NEXT SCENE
    public void LoadNextScene()
    {
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // 🏠 MAIN MENU
    public void LoadMainMenu()
    {
        GameManager.Instance.ResetGameState();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // 🔥 DIPANGGIL DARI GAME OVER
    public void ForceClose()
    {
        pauseUI.SetActive(false);
    }
}

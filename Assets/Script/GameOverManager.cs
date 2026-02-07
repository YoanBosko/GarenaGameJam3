using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    // DIPANGGIL DARI PLAYER / MUSUH
    public void GameOver()
    {
        // 🔥 MATIKAN PAUSE PANEL JIKA ADA
        PauseMenu pause = FindObjectOfType<PauseMenu>();
        if (pause != null)
            pause.ForceClose();

        gameOverPanel.SetActive(true);

        // 🔥 FREEZE GAMEPLAY PERMANEN
        GameManager.Instance.GameOver();
    }

    public void Restart()
    {
        GameManager.Instance.ResetGameState();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void Quit()
    {
        Application.Quit();
    }
}

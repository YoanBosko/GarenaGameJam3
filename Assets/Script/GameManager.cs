using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }

    // SATU FLAG UNTUK GAMEPLAY
    public bool IsGameplayFrozen => IsPaused || IsGameOver;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ======================
    // PAUSE
    // ======================
    public void PauseGame()
    {
        IsPaused = true;
    }

    public void ResumeGame()
    {
        IsPaused = false;
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
    }

    // ======================
    // GAME OVER
    // ======================
    public void GameOver()
    {
        IsGameOver = true;
    }

    public void ResetGameState()
    {
        IsPaused = false;
        IsGameOver = false;
    }
}

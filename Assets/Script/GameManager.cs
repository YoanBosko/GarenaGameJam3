using UnityEngine;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }

    public bool IsGameplayFrozen => IsPaused || IsGameOver;

    public static event Action<bool> OnGameplayFreezeChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        IsPaused = false;
        IsGameOver = false;
    }

    private IEnumerator Start()
    {
        yield return null;
        SetFreezeState(false);
    }

    private void SetFreezeState(bool freeze)
    {
        OnGameplayFreezeChanged?.Invoke(freeze);
    }

    // ========= PAUSE =========
    public void PauseGame()
    {
        // ❌ TIDAK BOLEH PAUSE SAAT GAME OVER
        if (IsGameOver) return;
        if (IsPaused) return;

        IsPaused = true;
        SetFreezeState(true);
    }

    public void ResumeGame()
    {
        // ❌ TIDAK BISA RESUME SAAT GAME OVER
        if (IsGameOver) return;
        if (!IsPaused) return;

        IsPaused = false;
        SetFreezeState(false);
    }

    public void TogglePause()
    {
        // 🔥 GAME OVER = PAUSE TERKUNCI TOTAL
        if (IsGameOver) return;

        if (IsPaused) ResumeGame();
        else PauseGame();
    }

    // ========= GAME OVER =========
    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        IsPaused = false; // 🔥 pastikan pause mati
        SetFreezeState(true);
    }

    // ========= RESET =========
    public void ResetGameState()
    {
        IsPaused = false;
        IsGameOver = false;
        SetFreezeState(false);
    }
}

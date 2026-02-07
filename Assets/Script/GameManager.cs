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

        // state awal
        IsPaused = false;
        IsGameOver = false;
    }

    private IEnumerator Start()
    {
        // 🔥 TUNGGU 1 FRAME
        // pastikan semua OnEnable() sudah subscribe
        yield return null;

        // 🔥 broadcast state awal (INI FIX 2× KLIK)
        SetFreezeState(false);
    }

    private void SetFreezeState(bool freeze)
    {
        OnGameplayFreezeChanged?.Invoke(freeze);
    }

    // ========= PAUSE =========
    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;
        SetFreezeState(true);
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;

        if (!IsGameOver)
            SetFreezeState(false);
    }

    public void TogglePause()
    {
        if (IsPaused) ResumeGame();
        else PauseGame();
    }

    // ========= GAME OVER =========
    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
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

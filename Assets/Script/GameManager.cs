using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    public static void PauseGame()
    {
        IsPaused = true;
    }

    public static void ResumeGame()
    {
        IsPaused = false;
    }

    public static void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }
}

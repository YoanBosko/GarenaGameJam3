using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseUI;
    // Struktur untuk menyimpan data transformasi awal
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
        // Simpan posisi awal semua child dari pauseUI saat pertama kali dijalankan
        if (pauseUI != null)
        {
            foreach (Transform child in pauseUI.transform)
            {
                InitialTransform data = new InitialTransform
                {
                    transform = child,
                    position = child.localPosition,
                    rotation = child.localRotation,
                    scale = child.localScale
                };
                childData.Add(data);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void TogglePauseMenu()
    {
        GameManager.Instance.TogglePause();
        bool isPaused = GameManager.Instance.IsPaused;

        if (isPaused)
        {
            // Reset posisi semua child sebelum menampilkan UI
            ResetChildPositions();
        }

        pauseUI.SetActive(isPaused);
    }

    // Fungsi untuk mengembalikan posisi semua child ke tatanan semula
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

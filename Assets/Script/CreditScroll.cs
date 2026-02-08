using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 0.5f;

    [Header("Stop Target")]
    public Transform stopTarget;          // Titik berhenti credit
    public bool stopExactly = true;

    [Header("After Finish")]
    public float delayBeforeLoad = 2f;    // Waktu tunggu sebelum ke Main Menu
    public string mainMenuSceneName = "MainMenu";

    private bool stopped;

    void Update()
    {
        if (stopped || stopTarget == null) return;

        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime, Space.World);

        if (transform.position.y >= stopTarget.position.y)
        {
            if (stopExactly)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    stopTarget.position.y,
                    transform.position.z
                );
            }

            stopped = true;
            Invoke(nameof(LoadMainMenu), delayBeforeLoad);
        }
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

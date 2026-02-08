using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FailSafeManager : MonoBehaviour
{
    // --- SINGLETON PATTERN ----
    public static FailSafeManager Instance { get; private set; }

    [Header("Settings")]
    public KeyCode reloadKey = KeyCode.R;
    public float requiredHoldTime = 2.0f;

    private float currentHoldTimer = 0f;
    private bool isReloading = false;

    void Awake()
    {
        // Inisialisasi Singleton
        if (Instance == null)
        {
            Instance = this;
            // Biasanya failsafe tetap ada di sepanjang game
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isReloading) return;

        // Cek apakah tombol ditekan/ditahan
        if (Input.GetKey(reloadKey))
        {
            currentHoldTimer += Time.deltaTime;

            // Logika progress (bisa dihubungkan ke UI Fill Amount jika ada)
            // Debug.Log("Reloading in: " + (requiredHoldTime - currentHoldTimer).ToString("F1"));

            if (currentHoldTimer >= requiredHoldTime)
            {
                ReloadCurrentScene();
            }
        }
        else
        {
            // Reset timer jika tombol dilepas sebelum 2 detik
            currentHoldTimer = 0f;
        }
    }

    private void ReloadCurrentScene()
    {
        isReloading = true;
        
        Debug.Log("Failsafe Triggered: Reloading Scene...");

        // Opsional: Mainkan SFX jika AudioManager tersedia
        if (AudioManager.Instance != null)
        {
            // AudioManager.Instance.PlaySFX("ReloadFeedback");
        }

        // Memuat ulang scene yang sedang aktif
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        // Reset state setelah load (karena DontDestroyOnLoad)
        StartCoroutine(ResetReloadFlag());
    }

    private IEnumerator ResetReloadFlag()
    {
        yield return new WaitForSeconds(0.5f);
        currentHoldTimer = 0f;
        isReloading = false;
    }

    // Fungsi tambahan untuk melihat progress dari script lain (misal untuk UI Bar)
    public float GetReloadProgress()
    {
        return Mathf.Clamp01(currentHoldTimer / requiredHoldTime);
    }
}
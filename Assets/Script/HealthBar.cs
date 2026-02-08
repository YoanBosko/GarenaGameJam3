using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShieldHealthBar : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider slider;

    [Header("Visual Feedback")]
    public Image backgroundImage;      // Tarik Image Background Slider ke sini
    public Color damageColor = Color.red;
    public float feedbackDuration = 0.2f;
    public float shakeIntensity = 10f;

    private Color originalColor;
    private Vector3 originalPosition;
    private Coroutine feedbackCoroutine;

    void Awake()
    {
        currentHealth = maxHealth;
        slider.value = 1f;

        if (backgroundImage != null)
            originalColor = backgroundImage.color;
            
        originalPosition = transform.localPosition;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("ShieldHealthBar: Feedback effect ended.");
        // currentHealth -= damage;
        // currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // slider.value = currentHealth / maxHealth;

        // if (AudioManager.Instance != null)
        // {
        //     AudioManager.Instance.PlaySFX("UIAttack");
        // }

        // Jalankan Efek Visual (Shake & Color)
        if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
        feedbackCoroutine = StartCoroutine(PlayFeedbackEffect());

        Debug.Log($"Shield HP: {currentHealth}");

        if (currentHealth <= 0)
            // Berikan jeda sedikit agar efek getar terakhir terlihat sebelum hancur
            Invoke("DestroyShield", 0.1f);
    }

    private IEnumerator PlayFeedbackEffect()
    {
        if (backgroundImage != null)
            backgroundImage.color = damageColor;

        float elapsed = 0f;
        while (elapsed < feedbackDuration)
        {
            // Efek Getar (Random offset pada local position)
            // Vector3 shakeOffset = (Vector3)Random.insideUnitCircle * shakeIntensity;
            // transform.localPosition = originalPosition + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Kembalikan ke posisi dan warna semula
        transform.localPosition = originalPosition;
        if (backgroundImage != null)
            backgroundImage.color = originalColor;

            
    }

    private void DestroyShield()
    {
        Destroy(gameObject);
    }
}

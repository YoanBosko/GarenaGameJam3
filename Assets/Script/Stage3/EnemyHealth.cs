using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public Slider healthSlider; // optional (kalau slider nempel di enemy)

    [Header("On Death")]
    public GameObject objectToDisable;
    public bool destroyEnemy = true;

    private bool isDead;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        Debug.Log($"{name} kena damage: {damage}");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth / maxHealth;
    }

    void Die()
    {
        isDead = true;

        Debug.Log($"{name} MATI");

        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        if (destroyEnemy)
            Destroy(gameObject);
    }
}

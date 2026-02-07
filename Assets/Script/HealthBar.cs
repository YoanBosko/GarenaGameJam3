using UnityEngine;
using UnityEngine.UI;

public class ShieldHealthBar : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider slider;

    void Awake()
    {
        currentHealth = maxHealth;
        slider.value = 1f;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        slider.value = currentHealth / maxHealth;

        Debug.Log($"Shield HP: {currentHealth}");

        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}

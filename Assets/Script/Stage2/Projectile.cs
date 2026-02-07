using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed = 10f;
    public float damage = 10f;
    public float range = 20f;

    Vector3 startPosition;

    [Header("Collision Settings")]
    public string[] destroyOnTags =
    {
        "Ground",
        "Wall",
        "Player",
        "UI-HealthBar"
    };

    public float visualFeedbackDuration = 0.1f;

    // ==========================
    // SETUP (DIPAKAI OLEH EnemyShooter)
    // ==========================
    public void Setup(float _speed, float _damage, float _range)
    {
        speed = _speed;
        damage = _damage;
        range = _range;
        startPosition = transform.position;
    }

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameplayFrozen)
        return;
        // Gerak maju
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Hancur jika sudah melebihi jarak
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // =========================
        // DAMAGE KE PLAYER (SATU HEALTH)
        // =========================

        // Kena UI-HealthBar (slider / tameng UI)
        if (other.CompareTag("UI-HealthBar"))
        {
            Health playerHealth = FindObjectOfType<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            DestroyProjectile(other);
            return;
        }

        // Kena Player langsung
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            DestroyProjectile(other);
            return;
        }

        // =========================
        // UI BUTTON
        // =========================
        if (other.CompareTag("UI-Button"))
        {
            HandleButton(other.gameObject);
            DestroyProjectile(other);
            return;
        }

        // =========================
        // OBJECT LAIN (GROUND / WALL)
        // =========================
        DestroyProjectile(other);
    }

    // ==========================
    // DESTROY LOGIC
    // ==========================
    void DestroyProjectile(Collider other)
    {
        foreach (string tag in destroyOnTags)
        {
            if (other.CompareTag(tag))
            {
                Destroy(gameObject);
                break;
            }
        }
    }

    // ==========================
    // UI BUTTON VISUAL FEEDBACK
    // ==========================
    private void HandleButton(GameObject obj)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.Invoke();
            StartCoroutine(TriggerButtonVisual(btn));
        }
    }

    private IEnumerator TriggerButtonVisual(Button btn)
    {
        if (btn.transition == Selectable.Transition.ColorTint && btn.targetGraphic != null)
        {
            ColorBlock cb = btn.colors;

            btn.targetGraphic.CrossFadeColor(
                cb.pressedColor,
                cb.fadeDuration,
                true,
                true
            );

            yield return new WaitForSeconds(visualFeedbackDuration);

            btn.targetGraphic.CrossFadeColor(
                cb.normalColor,
                cb.fadeDuration,
                true,
                true
            );
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    private float speed;
    private float damage;
    private float range;
    private Vector3 startPosition;

    [Header("Collision Settings")]
    public string[] destroyOnTags = { "Ground", "Wall", "Player" }; // Tag yang membuat peluru hancur
    public float visualFeedbackDuration = 0.1f;

    public void Setup(float _speed, float _damage, float _range)
    {
        speed = _speed;
        damage = _damage;
        range = _range;
        startPosition = transform.position;
    }

    void Update()
    {
        // Gerakan proyektil maju searah sumbu Z lokal (atau Forward)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Cek jarak tempuh
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1. Interaksi dengan Player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player terkena damage: " + damage);
            // Panggil fungsi damage di PlayerController jika ada
            HandleDestruction(other.gameObject);
        }

        // 2. Interaksi dengan UI (Sama seperti OnHit Player)
        if (other.CompareTag("UI-Button"))
        {
            HandleButton(other.gameObject);
        }
        else if (other.CompareTag("UI-HealthBar"))
        {
            HandleHealthBar(other.gameObject);
        }

        // 3. Cek apakah harus hancur
        foreach (string tag in destroyOnTags)
        {
            if (other.CompareTag(tag))
            {
                Destroy(gameObject);
                break;
            }
        }
    }

    private void HandleButton(GameObject obj)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.Invoke();
            StartCoroutine(TriggerButtonVisual(btn));
        }
    }

    private void HandleHealthBar(GameObject obj)
    {
        Slider healthSlider = obj.GetComponentInChildren<Slider>();
        if (healthSlider != null)
        {
            healthSlider.value -= damage;
        }
    }

    private IEnumerator TriggerButtonVisual(Button btn)
    {
        if (btn.transition == Selectable.Transition.ColorTint && btn.targetGraphic != null)
        {
            ColorBlock cb = btn.colors;
            btn.targetGraphic.CrossFadeColor(cb.pressedColor, cb.fadeDuration, true, true);
            yield return new WaitForSeconds(visualFeedbackDuration);
            btn.targetGraphic.CrossFadeColor(cb.normalColor, cb.fadeDuration, true, true);
        }
    }

    private void HandleDestruction(GameObject hitObj)
    {
        // Logika tambahan jika ingin partikel hancur, dll.
        Destroy(gameObject);
    }
}
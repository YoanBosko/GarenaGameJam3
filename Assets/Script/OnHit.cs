using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OnHit : MonoBehaviour
{
    [Header("Settings")]
    public float damageValue = 10f;
    public float visualFeedbackDuration = 0.1f;

    // Anti double-hit dalam satu attack
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    // Dipanggil dari PlayerController / Animation Event
    public void ClearHitList()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Pastikan hanya aktif saat attack
        if (!AttackState.isAttacking) return;

        EnemyHealth enemy = null;

        // =========================
        // 1️⃣ COBA LANGSUNG (BODY ENEMY)
        // =========================
        enemy = other.GetComponent<EnemyHealth>();

        // =========================
        // 2️⃣ COBA VIA HEALTH BAR LINK
        // =========================
        if (enemy == null)
        {
            EnemyHealthBarLink link = other.GetComponent<EnemyHealthBarLink>();
            if (link != null)
                enemy = link.enemyHealth;
        }

        // =========================
        // HIT ENEMY / HEALTH BAR
        // =========================
        if (enemy != null)
        {
            // cegah double hit (body + healthbar)
            if (alreadyHit.Contains(enemy.gameObject)) return;

            alreadyHit.Add(enemy.gameObject);
            enemy.TakeDamage(damageValue);

            Debug.Log($"Enemy hit via {other.name}, damage: {damageValue}");
            return;
        }

        // =========================
        // HIT UI BUTTON
        // =========================
        if (other.CompareTag("UI-Button"))
        {
            if (alreadyHit.Contains(other.gameObject)) return;

            alreadyHit.Add(other.gameObject);
            HandleButton(other.gameObject);
            return;
        }

        // =========================
        // HIT UI HEALTH BAR (UI SAJA, BUKAN ENEMY)
        // =========================
        if (other.CompareTag("UI-HealthBar"))
        {
            if (alreadyHit.Contains(other.gameObject)) return;

            alreadyHit.Add(other.gameObject);
            HandleHealthBar(other.gameObject);
        }
    }

    // =========================
    // UI BUTTON
    // =========================
    private void HandleButton(GameObject obj)
    {
        Button btn = obj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.Invoke();
            StartCoroutine(TriggerButtonVisual(btn));
            Debug.Log("Button Hit: OnClick invoked!");
        }
    }

    private IEnumerator TriggerButtonVisual(Button btn)
    {
        if (btn.transition != Selectable.Transition.ColorTint) yield break;

        ColorBlock cb = btn.colors;
        Graphic targetGraphic = btn.targetGraphic;
        if (targetGraphic == null) yield break;

        targetGraphic.CrossFadeColor(cb.pressedColor, cb.fadeDuration, true, true);
        yield return new WaitForSeconds(visualFeedbackDuration);
        targetGraphic.CrossFadeColor(cb.normalColor, cb.fadeDuration, true, true);
    }

    // =========================
    // UI HEALTH BAR (UI ONLY)
    // =========================
    private void HandleHealthBar(GameObject obj)
    {
        Slider slider = obj.GetComponentInChildren<Slider>();
        if (slider != null)
        {
            slider.value -= damageValue;
            Debug.Log("UI Health Bar Hit: Value reduced!");
        }
    }
}

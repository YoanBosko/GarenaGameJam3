using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OnHit : MonoBehaviour
{
    [Header("Settings")]
    public float damageValue = 10f;

    // Menyimpan daftar objek yang sudah terkena hit dalam satu ayunan
    private HashSet<GameObject> alreadyHit = new HashSet<GameObject>();

    // Dipanggil oleh PlayerController saat animasi attack dimulai
    public void ClearHitList()
    {
        alreadyHit.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Jika objek sudah pernah terkena hit dalam ayunan ini, abaikan
        if (alreadyHit.Contains(other.gameObject)) return;

        // Tambahkan ke daftar agar tidak terkena hit lagi di ayunan yang sama
        alreadyHit.Add(other.gameObject);

        // Logika berdasarkan Tag
        if (other.CompareTag("UI-Button"))
        {
            HandleButton(other.gameObject);
        }
        else if (other.CompareTag("UI-HealthBar"))
        {
            HandleHealthBar(other.gameObject);
        }
    }

    private void HandleButton(GameObject obj)
    {
        // Mencoba mengambil komponen Button (UI atau World Space)
        Button btn = obj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.Invoke();
            Debug.Log("Button Hit: OnClick invoked!");
        }
    }

    private void HandleHealthBar(GameObject obj)
    {
        // Mencoba mengambil Slider dari object tersebut atau parent-nya
        Slider healthSlider = obj.GetComponentInChildren<Slider>();
        if (healthSlider != null)
        {
            healthSlider.value -= damageValue;
            Debug.Log("Health Bar Hit: Value reduced!");
        }
    }
}
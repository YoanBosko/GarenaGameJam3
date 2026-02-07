using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayButton : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    public string namaSceneTujuan; // Nama scene yang ingin dimuat

    [Header("Pengaturan Transisi")]
    public Animator animatorTransisi; // Referensi ke Animator panel fade
    public float durasiTunggu = 1f;    // Waktu tunggu sebelum pindah scene (sesuai durasi animasi)

    // Fungsi ini dipanggil melalui Button OnClick
    public void JalankanGame()
    {
        StartCoroutine(ProsesPindahScene());
    }

    private IEnumerator ProsesPindahScene()
    {
        // 1. Jalankan trigger untuk memulai animasi Fade In
        if (animatorTransisi != null)
        {
            animatorTransisi.SetTrigger("StartFade");
        }
        else
        {
            Debug.LogWarning("Animator Transisi belum dipasang di Inspector!");
        }

        // 2. Tunggu selama durasi animasi agar visual terlihat halus
        yield return new WaitForSeconds(durasiTunggu);

        // 3. Pindah ke scene tujuan
        if (!string.IsNullOrEmpty(namaSceneTujuan))
        {
            SceneManager.LoadScene(namaSceneTujuan);
        }
        else
        {
            Debug.LogError("Nama Scene Tujuan kosong!");
        }
    }
}
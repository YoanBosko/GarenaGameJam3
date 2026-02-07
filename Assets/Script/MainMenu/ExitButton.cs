using UnityEngine;

public class ExitButton : MonoBehaviour
{
    // Fungsi ini dipanggil melalui Button OnClick
    public void KeluarPermainan()
    {
        Debug.Log("Keluar dari permainan...");
        
        // Menutup aplikasi (Hanya bekerja di build final, bukan di Editor)
        Application.Quit();

        // Kode tambahan untuk testing di dalam Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
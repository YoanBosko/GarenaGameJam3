using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TiltSliderLoader : MonoBehaviour
{
    public enum SliderMode { Volume, Loading }

    [Header("Mode")]
    public SliderMode sliderMode = SliderMode.Loading;

    [Header("UI")]
    public Slider slider;

    [Header("Tilt Logic (New)")]
    [Tooltip("Faktor kecepatan: Semakin besar, semakin cepat slider bertambah pada sudut yang sama.")]
    public float fillSpeedMultiplier = 0.33f; 
    
    [Tooltip("Batas sudut maksimal untuk perhitungan kecepatan.")]
    public float maxTiltAngle = 60f;

    [Header("Slider Animation")]
    public float sliderSmoothTime = 0.1f;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float maxVolume = 1f;

    [Header("Scene Transition")]
    public bool loadNextSceneOnComplete = false;
    public float fallDelay = 3f;

    // Internal logic
    bool completed;
    float sliderVelocity;
    Vector3 defaultLeftDir;
    float currentInternalValue = 0f; // Penampung nilai progres

    void Start()
    {
        if (slider == null) slider = GetComponent<Slider>();
        
        defaultLeftDir = -transform.right;

        // Start dari 0 sesuai permintaan Anda
        currentInternalValue = 0f;
        slider.value = 0f;

        if (sliderMode == SliderMode.Volume)
            AudioListener.volume = 0f;
    }

    void Update()
    {
        if (completed) return;

        // 1. Hitung Sudut Kemiringan
        Vector3 currentLeftDir = -transform.right;
        float signedAngle = Vector3.SignedAngle(currentLeftDir, defaultLeftDir, Vector3.forward);

        // 2. Logika Baru: Tambahkan nilai berdasarkan derajat kemiringan
        // Jika signedAngle +15, maka bertambah. Jika -15, maka berkurang.
        // Kita bagi dengan suatu angka (misal 3) agar 15 derajat = +5 unit/detik
        float speed = signedAngle * fillSpeedMultiplier;

        // Update nilai progres berdasarkan waktu (Time.deltaTime)
        // Nilai slider Unity biasanya 0-1, jika slider Anda 0-100, sesuaikan limitnya
        currentInternalValue += speed * Time.deltaTime;
        currentInternalValue = Mathf.Clamp(currentInternalValue, 0f, 100f); // Contoh rentang 0-100

        // 3. Terapkan ke Slider dengan smoothing
        slider.value = Mathf.SmoothDamp(
            slider.value, 
            currentInternalValue, 
            ref sliderVelocity, 
            sliderSmoothTime
        );

        // 4. Update Volume
        if (sliderMode == SliderMode.Volume)
            AudioListener.volume = (slider.value / slider.maxValue) * maxVolume;

        // 5. Cek Selesai
        if (sliderMode == SliderMode.Loading && slider.value >= (slider.maxValue * 0.99f))
            StartCoroutine(CompleteSequence());
    }

    IEnumerator CompleteSequence()
    {
        completed = true;
        slider.value = slider.maxValue;
        yield return new WaitForSeconds(fallDelay);

        if (loadNextSceneOnComplete)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TiltSliderLoader : MonoBehaviour
{
    // =========================
    // MODE
    // =========================
    public enum SliderMode
    {
        Volume,
        Loading
    }

    [Header("Mode")]
    public SliderMode sliderMode = SliderMode.Loading;

    // =========================
    // UI
    // =========================
    [Header("UI")]
    public Slider slider;

    // =========================
    // TILT SETTINGS
    // =========================
    [Header("Tilt Settings")]
    [Tooltip("Sudut maksimum dari pose awal")]
    public float maxTilt = 60f;

    [Tooltip("Jika sisi kiri naik >= ini dari pose awal, langsung full")]
    public float instantFillAngle = 30f;

    // =========================
    // SLIDER ANIMATION
    // =========================
    [Header("Slider Animation")]
    public float sliderSmoothTime = 0.25f;

    // =========================
    // VOLUME
    // =========================
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float maxVolume = 1f;

    // =========================
    // SCENE
    // =========================
    [Header("Scene Transition")]
    public bool loadNextSceneOnComplete = false;
    public float fallDelay = 3f;

    // =========================
    // DEBUG
    // =========================
    [Header("DEBUG (Inspector Only)")]
    [SerializeField] float signedAngleFromDefault;
    [SerializeField] float targetValue;

    // =========================
    // INTERNAL
    // =========================
    bool completed;
    Rigidbody rb;
    float sliderVelocity;

    // arah kiri saat START (patokan nol derajat)
    Vector3 defaultLeftDir;

    // =========================
    // UNITY
    // =========================
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (slider == null)
            slider = GetComponent<Slider>();

        // =========================
        // SIMPAN POSE AWAL (0°)
        // =========================
        defaultLeftDir = -transform.right;

        // lurus = tengah
        slider.value = 0.5f;

        if (sliderMode == SliderMode.Volume)
            AudioListener.volume = slider.value * maxVolume;
    }

    void Update()
    {
        if (completed) return;

        // ==================================================
        // ARAH KIRI SAAT INI
        // ==================================================
        Vector3 currentLeftDir = -transform.right;

        // ==================================================
        // SUDUT RELATIF DARI POSE AWAL
        // (+) kiri naik dari default
        // (-) kiri turun dari default
        // ==================================================
        signedAngleFromDefault = Vector3.SignedAngle(
        currentLeftDir,
        defaultLeftDir,
        Vector3.forward
    );


        // ==================================================
        // HITUNG TARGET SLIDER
        // ==================================================
        if (sliderMode == SliderMode.Loading &&
            signedAngleFromDefault >= instantFillAngle)
        {
            // 💧 air tumpah langsung penuh
            targetValue = 1f;
        }
        else
        {
            // mapping:
            // -maxTilt → 0
            // 0 → 0.5
            // +maxTilt → 1
            targetValue = Mathf.Clamp01(
                (signedAngleFromDefault / maxTilt + 1f) * 0.5f
            );
        }

        // ==================================================
        // APPLY SLIDER
        // ==================================================
        slider.value = Mathf.SmoothDamp(
            slider.value,
            targetValue,
            ref sliderVelocity,
            sliderSmoothTime
        );

        // ==================================================
        // APPLY VOLUME
        // ==================================================
        if (sliderMode == SliderMode.Volume)
            AudioListener.volume = slider.value * maxVolume;

        // ==================================================
        // COMPLETE (LOADING)
        // ==================================================
        if (sliderMode == SliderMode.Loading && slider.value >= 0.99f)
            StartCoroutine(CompleteSequence());
    }

    // =========================
    // COMPLETE SEQUENCE
    // =========================
    IEnumerator CompleteSequence()
    {
        completed = true;
        slider.value = 1f;

        // if (rb != null)
        // {
        //     rb.useGravity = true;
        //     rb.drag = 0f;
        //     rb.angularDrag = 0f;
        // }

        yield return new WaitForSeconds(fallDelay);

        if (loadNextSceneOnComplete)
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex + 1
            );
        }
    }
}

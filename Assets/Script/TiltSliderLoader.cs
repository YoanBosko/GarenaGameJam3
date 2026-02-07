using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TiltSliderLoader : MonoBehaviour
{
    [Header("UI")]
    public Slider slider;

    [Header("Tilt Settings")]
    public float maxTilt = 60f;

    [Header("Slider Animation")]
    public float sliderSmoothTime = 0.4f;

    [Header("Finish Settings")]
    public float fallDelay = 3f;

    [Header("DEBUG (Inspector Only)")]
    [SerializeField] float currentAngle;     // derajat real
    [SerializeField] float progressPercent;  // 0–100 %

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float maxVolume = 1f;

    [Header("Scene Transition")]
    public bool loadNextSceneOnComplete = false;



    bool completed;
    Rigidbody rb;
    float sliderVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (slider == null)
            slider = GetComponent<Slider>();

        slider.value = 0f;
    }

    void Update()
    {
        if (completed) return;

        // === HITUNG SUDUT SLIDER ===
        currentAngle = Mathf.Abs(NormalizeAngle(transform.eulerAngles.z));

        // === HITUNG PERSENTASE ===
        progressPercent = Mathf.Clamp01(currentAngle / maxTilt) * 100f;

        // === TARGET SLIDER ===
        float targetValue = progressPercent / 100f;

        // === SET VOLUME (YOUTUBE STYLE) ===
        AudioListener.volume = slider.value * maxVolume;   


        // === ANIMASI SLIDER ===
        slider.value = Mathf.SmoothDamp(
            slider.value,
            targetValue,
            ref sliderVelocity,
            sliderSmoothTime
        );

        // === SELESAI ===
        if (currentAngle >= maxTilt)
        {
            StartCoroutine(CompleteSequence());
        }
    }

    IEnumerator CompleteSequence()
    {
        completed = true;

        Debug.Log($"DEBUG: COMPLETE at {currentAngle:F1}° ({progressPercent:F0}%)");

        // reset slider
        slider.value = 0f;

        // jatuh
        rb.useGravity = true;
        rb.drag = 0f;
        rb.angularDrag = 0f;

        yield return new WaitForSeconds(fallDelay);

    Debug.Log("Udh Full");

    if (loadNextSceneOnComplete)
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex + 1
        );
    }

    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}

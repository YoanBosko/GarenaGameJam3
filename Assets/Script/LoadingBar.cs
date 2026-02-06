using UnityEngine;

public class LoadingBar : MonoBehaviour
{
    [Header("Fill")]
    public Transform fill;                  // DRAG: GameObject "Fill" (EMPTY)
    public SpriteRenderer fillRenderer;     // DRAG: FillSprite

    [Header("Direction Requirement")]
    public float directionThreshold = 0.65f;

    [Header("Ultra Heavy Settings")]
    public float pushGain = 0.2f;           // tenaga per sundulan
    public float minPowerToMove = 3.0f;     // HARUS lewat ini
    public float powerDecay = 2.0f;         // tenaga cepat habis
    public float baseRotationPower = 15f;   // kekuatan awal rotasi
    public float maxTilt = 60f;             // DERajat selesai

    [Header("Debug")]
    public bool showDebug = true;

    float accumulatedPower;
    float currentFill;
    bool completed;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (fillRenderer == null && fill != null)
            fillRenderer = fill.GetComponentInChildren<SpriteRenderer>();

        if (showDebug)
            Debug.Log("DEBUG: LoadingBar initialized");
    }

    void FixedUpdate()
    {
        if (completed) return;

        // === POWER DECAY (BERAT) ===
        accumulatedPower = Mathf.MoveTowards(
            accumulatedPower,
            0f,
            powerDecay * Time.fixedDeltaTime
        );

        if (accumulatedPower < minPowerToMove)
            return;

        // === HITUNG SUDUT SAAT INI ===
        float angle = Mathf.Abs(NormalizeAngle(transform.eulerAngles.z));

        // === DIMINISHING RETURN ===
        float resistance = Mathf.Clamp01(angle / maxTilt);

        float rotationPower = Mathf.Lerp(
            baseRotationPower,
            baseRotationPower * 0.25f,
            resistance
        );

        float deltaTilt =
            (accumulatedPower - minPowerToMove)
            * rotationPower
            * Time.fixedDeltaTime;

        rb.MoveRotation(
            rb.rotation * Quaternion.Euler(0f, 0f, deltaTilt)
        );

        // === UPDATE FILL BERDASARKAN SUDUT ===
        currentFill = Mathf.Clamp01(angle / maxTilt);

        if (fill != null)
        {
            fill.localScale = new Vector3(currentFill, 1f, 1f);
        }

        if (fillRenderer != null)
        {
            // warna dari merah → kuning → hijau
            Color c = currentFill < 0.5f
                ? Color.Lerp(Color.red, Color.yellow, currentFill / 0.5f)
                : Color.Lerp(Color.yellow, Color.green, (currentFill - 0.5f) / 0.5f);

            fillRenderer.color = c;
        }

        if (showDebug)
        {
            Debug.Log($"DEBUG: Angle={angle:F2} Fill={currentFill:F2}");
        }

        // === CEK COMPLETE ===
        if (angle >= maxTilt)
        {
            Debug.Log("DEBUG: 60 DEGREE REACHED");
            Complete();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (completed) return;
        if (!other.CompareTag("Cursor")) return;

        Rigidbody cursorRb = other.attachedRigidbody;
        if (cursorRb == null) return;

        Vector3 vel = cursorRb.velocity;
        if (vel.sqrMagnitude < 0.02f) return;

        Vector3 dir = vel.normalized;

        // HARUS DARI KIRI + BAWAH
        float leftDot = Vector3.Dot(dir, Vector3.left);
        float downDot = Vector3.Dot(dir, Vector3.down);

        if (leftDot < directionThreshold || downDot < directionThreshold)
            return;

        accumulatedPower += pushGain * vel.magnitude;

        if (showDebug)
        {
            Debug.Log($"DEBUG: Power += {pushGain * vel.magnitude:F2} (Total {accumulatedPower:F2})");
        }
    }

    void Complete()
    {
        completed = true;
        accumulatedPower = 0f;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // pastikan fill penuh
        if (fill != null)
            fill.localScale = new Vector3(1f, 1f, 1f);

        if (fillRenderer != null)
            fillRenderer.color = Color.green;

        Debug.Log("DEBUG: COMPLETE() CALLED");
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}

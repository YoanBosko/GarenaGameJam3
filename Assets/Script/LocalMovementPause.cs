using UnityEngine;

public class LocalMovementPause : MonoBehaviour
{
    public KeyCode pauseKey = KeyCode.Escape;
    public Rigidbody rb;

    bool isPaused;
    Vector3 cachedVelocity;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
            TogglePause();
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            cachedVelocity = rb.velocity;
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = cachedVelocity;
        }
    }

    public bool IsPaused() => isPaused;
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GameplayFreezable : MonoBehaviour
{
    private Rigidbody rb;

    private bool originalKinematic;
    private Vector3 savedVelocity;
    private Vector3 savedAngularVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalKinematic = rb.isKinematic;
    }

    private void OnEnable()
    {
        GameManager.OnGameplayFreezeChanged += OnFreezeChanged;

        if (GameManager.Instance != null)
            OnFreezeChanged(GameManager.Instance.IsGameplayFrozen);
    }

    private void OnDisable()
    {
        GameManager.OnGameplayFreezeChanged -= OnFreezeChanged;
    }

    private void OnFreezeChanged(bool freeze)
    {
        if (freeze)
        {
            savedVelocity = rb.velocity;
            savedAngularVelocity = rb.angularVelocity;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = originalKinematic;
            rb.velocity = savedVelocity;
            rb.angularVelocity = savedAngularVelocity;
        }
    }
}

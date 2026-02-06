using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!AttackState.isAttacking) return;

        ButtonOnHit button = other.GetComponent<ButtonOnHit>();
        if (button != null)
        {
            button.Hit();
        }
    }
}

using UnityEngine;

public class AttackState : MonoBehaviour
{
    public static bool isAttacking = true;
    
    public void StartAttack()
    {
        isAttacking = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }
}

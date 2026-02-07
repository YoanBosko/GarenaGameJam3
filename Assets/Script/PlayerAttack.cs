using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public float damage = 20f;

    private HashSet<EnemyHealth> hitEnemies = new HashSet<EnemyHealth>();

    private void OnEnable()
    {
        hitEnemies.Clear(); // reset setiap attack
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!AttackState.isAttacking) return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null) return;

        if (hitEnemies.Contains(enemy)) return;

        hitEnemies.Add(enemy);
        enemy.TakeDamage(damage);
    }
}

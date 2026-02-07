using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float damage = 15f;
    public float attackCooldown = 1.2f;

    float lastAttackTime;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        Health health = player.GetComponent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}

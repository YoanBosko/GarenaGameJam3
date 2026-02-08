using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    public enum BossState { Idle, Chasing, Attacking, Retreating }

    [Header("Movement Settings")]
    public float chaseSpeed = 5f;
    public float attackDashSpeed = 12f;
    public float retreatSpeed = 3f;
    public float stopDistance = 3f;

    [Header("Attack Settings")]
    public float attackInterval = 3f;
    public float retreatDuration = 1.5f;
    public GameObject damageHitbox; // Hitbox yang akan selalu aktif

    [Header("References")]
    private Rigidbody rb;
    private Animator anim;
    private Transform player;

    private BossState currentState = BossState.Idle;
    private float attackTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
        // Mengunci rotasi dan posisi Z untuk kamera 2.5D
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

        // Sesuai permintaan: Hitbox selalu aktif sejak awal
        if (damageHitbox != null) 
        {
            damageHitbox.SetActive(true);
        }

        // Mencari player melalui Singleton
        if (PlayerController.Instance != null)
            player = PlayerController.Instance.transform;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameplayFrozen)
        return;
        if (player == null) return;

        // Hitung jarak ke player (X-axis saja)
        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        switch (currentState)
        {
            case BossState.Idle:
                HandleIdle(distanceToPlayer);
                break;
            case BossState.Chasing:
                HandleChasing(distanceToPlayer);
                break;
            case BossState.Retreating:
                // Logika mundur diatur oleh Coroutine RetreatSequence
                break;
        }

        FlipTowardsPlayer();
    }

    private void HandleIdle(float distance)
    {
        anim.SetFloat("Speed", 0);
        attackTimer += Time.deltaTime;

        // Jika interval serangan tercapai dan player cukup dekat
        if (attackTimer >= attackInterval && distance <= stopDistance + 1f)
        {
            StartCoroutine(PerformAttack());
        }
        else if (distance > stopDistance)
        {
            currentState = BossState.Chasing;
        }
    }

    private void HandleChasing(float distance)
    {
        anim.SetFloat("Speed", 1);
        
        if (distance <= stopDistance)
        {
            currentState = BossState.Idle;
        }
        else
        {
            float direction = player.position.x > transform.position.x ? 1 : -1;
            rb.velocity = new Vector3(direction * chaseSpeed, rb.velocity.y, 0);
        }
    }

    private IEnumerator PerformAttack()
    {
        currentState = BossState.Attacking;
        attackTimer = 0;

        // Picu Animasi
        anim.SetTrigger("attack");

        // Efek Maju/Dash saat menyerang (Menerjang Player)
        float attackDir = player.position.x > transform.position.x ? 1 : -1;
        rb.velocity = new Vector3(attackDir * attackDashSpeed, rb.velocity.y, 0);

        // Tunggu durasi animasi serangan selesai
        yield return new WaitForSeconds(0.8f);

        // Mulai Fase Mundur setelah menyerang
        StartCoroutine(RetreatSequence());
    }

    private IEnumerator RetreatSequence()
    {
        currentState = BossState.Retreating;
        float timer = 0;

        while (timer < retreatDuration)
        {
            // Mundur menjauhi player untuk menjaga jarak
            float retreatDir = player.position.x > transform.position.x ? -1 : 1;
            rb.velocity = new Vector3(retreatDir * retreatSpeed, rb.velocity.y, 0);
            anim.SetFloat("Speed", 0.5f); 

            timer += Time.deltaTime;
            yield return null;
        }

        currentState = BossState.Idle;
    }

    private void FlipTowardsPlayer()
    {
        // Jangan memutar arah saat sedang menerjang (Attacking) agar gerakan konsisten
        if (currentState == BossState.Attacking) return;

        if (player.position.x > transform.position.x)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else
            transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    // Fungsi Animation Events tetap dipertahankan jika nantinya ingin mengganti
    // jenis damage atau visual saat animasi tertentu, namun untuk saat ini 
    // hitbox utama sudah diatur aktif di Start().
    public void EnableBossHitbox() { if (damageHitbox != null) damageHitbox.SetActive(true); }
    public void DisableBossHitbox() { /* Hitbox selalu aktif, fungsi ini bisa dikosongkan */ }
}
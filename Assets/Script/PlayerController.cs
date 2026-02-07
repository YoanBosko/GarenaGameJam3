using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // --- SINGLETON PATTERN ---
    public static PlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    
    [Header("Detection")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Attack Settings")]
    public GameObject attackCollider; 
    private OnHit onHitScript;

    private Rigidbody rb;
    private Animator anim;
    private float horizontalInput;
    private bool isGrounded;
    private bool isAttacking = false;
    private Coroutine moveCoroutine; // Untuk melacak gerakan otomatis

    void Awake()
    {
        // Inisialisasi Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        if (attackCollider != null)
        {
            attackCollider.SetActive(false);
            onHitScript = attackCollider.GetComponent<OnHit>();
        }
        
        // Memastikan karakter tidak terguling
        // rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
    }

    void Update()
    {
        if (GameManager.Instance.IsPaused) return;
        if (GameManager.Instance.IsGameplayFrozen) return;
        // 1. Ambil Input (Hanya jika tidak sedang dalam Coroutine Move otomatis)
        if (moveCoroutine == null)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
        }

        // 2. Deteksi Tanah
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // A. Animasi Jalan/Lari
        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // B. Animasi Attack
        if (Input.GetKeyDown(KeyCode.F) && !isAttacking)
        {
            Attack();
        }

        // 3. Input Lompat
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
        }

        // 4. Memutar arah
        FlipCharacter();
    }

    void FixedUpdate()
    {
        // Gerakkan karakter
        float currentSpeed = isAttacking ? 0 : horizontalInput * moveSpeed;
        rb.velocity = new Vector3(currentSpeed, rb.velocity.y, 0);
    }

    void Attack()
    {
        isAttacking = true;
        if (onHitScript != null) onHitScript.ClearHitList();
        anim.SetTrigger("Attack");
    }

    // --- FUNGSI GERAK OTOMATIS (DIPANGGIL DARI UI/SCRIPT LAIN) ---

    public void MoveLeft()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(TimedMove(-1f, 0.1f));
    }

    public void MoveRight()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(TimedMove(1f, 0.1f));
    }

    public void MoveJump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
        }
    }

    public void MoveAttack()
    {
        if (!isAttacking)
        {
            Attack();
        }
    }

    private IEnumerator TimedMove(float direction, float duration)
    {
        horizontalInput = direction;
        yield return new WaitForSeconds(duration);
        horizontalInput = 0;
        moveCoroutine = null;
    }

    // --- FUNGSI UNTUK ANIMATION EVENTS ---

    public void EnableAttackCollider()
    {
        if (attackCollider != null) attackCollider.SetActive(true);
        isAttacking = false; // Mengembalikan kontrol gerak setelah animasi selesai
    }

    public void DisableAttackCollider()
    {
        if (attackCollider != null) attackCollider.SetActive(false);
        // isAttacking = false; // Mengembalikan kontrol gerak setelah animasi selesai
    }

    void FlipCharacter()
    {
        if (isAttacking) return;

        if (horizontalInput > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else if (horizontalInput < 0)
            transform.rotation = Quaternion.Euler(0, -90, 0);
    }
}
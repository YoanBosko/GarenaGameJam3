using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    
    [Header("Detection")] // Untuk boolean mendeteksi tanah
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Animator anim; // [BARU] Variabel untuk Animator
    private float horizontalInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        
        // Memastikan karakter tidak terguling saat menabrak objek
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                         RigidbodyConstraints.FreezeRotationY | 
                         RigidbodyConstraints.FreezeRotationZ | 
                         RigidbodyConstraints.FreezePositionZ;
    }

    void Update()
    {
        // 1. Ambil Input (A/D atau Panah)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 2. Deteksi apakah karakter menyentuh tanah
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // A. Animasi Jalan/Lari
        // Kita kirim nilai absolut (selalu positif) dari input horizontal.
        // Jika 0 = diam, jika 1 = jalan.
        anim.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // B. Animasi Attack
        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetTrigger("Attack"); // Memicu parameter 'Trigger' bernama Attack
        }

        // 3. Input Lompat
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
            // anim.SetTrigger("Jump"); // Aktifkan baris ini jika sudah ada animasi lompat
        }

        // 4. Memutar arah karakter (Facing Direction)
        FlipCharacter();
    }

    void FixedUpdate()
    {
        // Gerakkan karakter menggunakan Velocity agar fisika tetap konsisten
        rb.velocity = new Vector3(horizontalInput * moveSpeed, rb.velocity.y, 0);
    }

    void FlipCharacter()
    {
        if (horizontalInput > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0); // Menghadap kanan
        else if (horizontalInput < 0)
            transform.rotation = Quaternion.Euler(0, -90, 0); // Menghadap kiri
    }
}
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject projectilePrefab; // Prefab proyektil
    public Transform firePoint;         // Titik tempat peluru muncul
    public float fireInterval = 2.0f;   // Jeda waktu antar tembakan

    [Header("Projectile Stats")]
    public float projectileSpeed = 10f;
    public float projectileDamage = 10f;
    public float projectileRange = 20f;

    [Header("Animation Settings")]
    private Animator anim;              // Komponen Animator

    // Variabel untuk menyimpan lokasi awal
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float timer;
    
    void Start()
    {
        // Mengambil komponen Animator pada object ini
        anim = GetComponent<Animator>();

        // Menyimpan posisi dan rotasi awal saat game dimulai
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameplayFrozen)
        return;

        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        // --- RESET POSISI KE AWAL ---
        // Mengembalikan posisi karakter agar tidak bergeser akibat animasi
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        
        if (anim != null)
        {
            anim.SetTrigger("Slash");
            Debug.Log("EnemyShooter: Shoot animation triggered.");
        }

        if (projectilePrefab != null && firePoint != null)
        {
            // Spawn proyektil
            GameObject projGo = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            
            // Set stats ke komponen Projectile
            Projectile proj = projGo.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Setup(projectileSpeed, projectileDamage, projectileRange);
            }
        }
        // Opsional: Jalankan SFX jika kamu ingin musuh mengeluarkan suara saat menembak
        // AudioManager.Instance.PlaySFX("EnemyShoot");
    }
}
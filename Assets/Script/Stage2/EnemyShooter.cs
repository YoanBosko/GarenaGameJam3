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

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireInterval)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
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
    }
}
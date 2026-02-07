using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CursorFollowMouse : MonoBehaviour
{
    Camera cam;
    Rigidbody rb;

    [Header("World Settings")]
    public float cursorWorldZ = 0f;
    public float sensitivity = 1f;

    Vector2 screenPos;
    float radiusPx;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.drag = 20f;          // rem biar tidak drift
        rb.angularDrag = 20f;

        Cursor.visible = false;

        // mulai dari tengah layar
        screenPos = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // set posisi awal cursor ke world Z yang benar
        float dist = cursorWorldZ - cam.transform.position.z;
        Vector3 startWorldPos = cam.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, dist)
        );
        transform.position = startWorldPos;
        rb.velocity = Vector3.zero;

        // === HITUNG RADIUS KURSOR DALAM PIXEL ===
        Collider col = GetComponent<Collider>();
        Bounds b = col.bounds;

        Vector3 centerScreen = cam.WorldToScreenPoint(transform.position);
        Vector3 rightScreen  = cam.WorldToScreenPoint(transform.position + Vector3.right * b.extents.x);

        radiusPx = Mathf.Abs(rightScreen.x - centerScreen.x);
    }

    void FixedUpdate()
    {
        // === 1. AMBIL DELTA MOUSE ===
        float dx = Input.GetAxisRaw("Mouse X") * sensitivity * 100f;
        float dy = Input.GetAxisRaw("Mouse Y") * sensitivity * 100f;

        // === 2. UPDATE POSISI SCREEN (LOGIKA) ===
        screenPos.x += dx;
        screenPos.y += dy;

        // === 3. CLAMP DENGAN RADIUS (BIAR BENAR-BENAR KE POJOK) ===
        screenPos.x = Mathf.Clamp(
            screenPos.x,
            radiusPx,
            Screen.width - radiusPx
        );

        screenPos.y = Mathf.Clamp(
            screenPos.y,
            radiusPx,
            Screen.height - radiusPx
        );

        // === 4. SCREEN → WORLD ===
        float dist = cursorWorldZ - cam.transform.position.z;

        Vector3 targetWorld = cam.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, dist)
        );

        // === 5. GERAKKAN CURSOR (PHYSICS FRIENDLY) ===
        rb.velocity = (targetWorld - rb.position) / Time.fixedDeltaTime;
    }
}

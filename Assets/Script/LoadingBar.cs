using UnityEngine;

public class LoadingBar : MonoBehaviour
{
    [Header("Frames")]
    public GameObject redFrame;
    public GameObject yellowFrame;
    public GameObject greenFrame;

    [Header("Rotation Threshold")]
    public float redAngle = 20f;
    public float yellowAngle = 40f;
    public float greenAngle = 60f;

    bool redShown;
    bool yellowShown;
    bool greenShown;

    void Start()
    {
        // pastikan semua mati di awal
        redFrame.SetActive(false);
        yellowFrame.SetActive(false);
        greenFrame.SetActive(false);
    }

    void FixedUpdate()
    {
        float angle = Mathf.Abs(NormalizeAngle(transform.eulerAngles.z));

        // 🔴 20°
        if (!redShown && angle >= redAngle)
        {
            redFrame.SetActive(true);
            redShown = true;
            Debug.Log("DEBUG: RED FRAME ACTIVE (20°)");
        }

        // 🟡 40°
        if (!yellowShown && angle >= yellowAngle)
        {
            yellowFrame.SetActive(true);
            yellowShown = true;
            Debug.Log("DEBUG: YELLOW FRAME ACTIVE (40°)");
        }

        // 🟢 60°
        if (!greenShown && angle >= greenAngle)
        {
            greenFrame.SetActive(true);
            greenShown = true;
            Debug.Log("DEBUG: GREEN FRAME ACTIVE (60°)");
        }
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}

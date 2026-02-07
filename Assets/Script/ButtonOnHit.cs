using UnityEngine;
using UnityEngine.UI;

public class ButtonOnHit: MonoBehaviour
{
    Button btn;
    bool alreadyHit;

    void Awake()
    {
        btn = GetComponent<Button>();
    }

    public void Hit()
    {
        if (alreadyHit) return;

        alreadyHit = true;
        Debug.Log("BUTTON TERKENA SERANGAN → KEKLIK");
        btn.onClick.Invoke();
    }
}

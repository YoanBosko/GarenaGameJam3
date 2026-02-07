using UnityEngine;
using TMPro;

public class SceneStopwatch : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timeText;

    [Header("Save Key")]
    public string timeKey = "LAST_SCENE_TIME";

    float elapsedTime;
    bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        float seconds = elapsedTime % 60f;
        timeText.text = $"{minutes:00}:{seconds:00.00}";
    }

    // ======================
    // STOP & SAVE
    // ======================
    public void StopAndSave()
    {
        isRunning = false;

        PlayerPrefs.SetFloat(timeKey, elapsedTime);
        PlayerPrefs.Save();

        Debug.Log($"STOPWATCH STOPPED: {elapsedTime:F2} seconds");
    }

    public float GetFinalTime()
    {
        return elapsedTime;
    }
}

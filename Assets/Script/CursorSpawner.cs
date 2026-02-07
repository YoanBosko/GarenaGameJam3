using UnityEngine;
using System.Collections;

public class CursorSpawner : MonoBehaviour
{
    public GameObject cursorPrefab;
    public float spawnDelay = 3f;

    static GameObject currentCursor;

    void Awake()
    {
        if (currentCursor != null)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(SpawnCursorDelayed());
    }

    IEnumerator SpawnCursorDelayed()
    {
        yield return new WaitForSeconds(spawnDelay);

        currentCursor = Instantiate(cursorPrefab);
        DontDestroyOnLoad(currentCursor);
    }
}

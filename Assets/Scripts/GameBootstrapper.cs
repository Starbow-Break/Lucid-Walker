using UnityEngine;

public static class GameBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        // LevelManager 생성
        if (LevelManager.Instance == null)
        {
            GameObject levelManagerPrefab = Resources.Load<GameObject>("LevelManager");
            if (levelManagerPrefab != null)
            {
                GameObject go = Object.Instantiate(levelManagerPrefab);
                Object.DontDestroyOnLoad(go);
            }
        }

        // DataPersistenceManager 생성
        if (DataPersistenceManager.instance == null)
        {
            GameObject dpPrefab = Resources.Load<GameObject>("DataPersistenceManager");
            if (dpPrefab != null)
            {
                GameObject go = Object.Instantiate(dpPrefab);
                Object.DontDestroyOnLoad(go);
            }
        }

        Debug.Log("📌 Game Managers Initialized");
    }
}

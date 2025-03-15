using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject GetPooledObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        string tag = prefab.name;
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("풀에 " + tag + "가 없습니다!");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        // 만약 객체가 이미 파괴되었다면, 새 객체를 생성하거나, 계속해서 큐에서 꺼내보세요.
        if (objectToSpawn == null)
        {
            objectToSpawn = Instantiate(prefab);
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);
        return objectToSpawn;
    }


    public void ReturnPooledObject(GameObject obj)
    {
        // 반환 시 추가로 처리할 게 있다면 여기서 처리
        obj.SetActive(false);
    }
}

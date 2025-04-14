using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PunchSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct PunchPatternData
    {
        public int interval;
        public List<int> pattern;
    }

    [SerializeField] BossPunch punchPrefab;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<PunchPatternData> datas;
    [SerializeField] int orderShuffleMin;
    [SerializeField] int orderShuffleMax;

    public bool isSpawning { get; private set; } = false;
    public int spawnedPunch { get; private set; } = 0;

    public void SpawnPunch()
    {
        StartCoroutine(SpawnSequence());
    }

    private IEnumerator SpawnSequence()
    {
        isSpawning = true;

        // 패턴 데이터 랜덤 선별
        PunchPatternData data = datas[Random.Range(0, datas.Count)];

        List<int> order = new List<int>();
        foreach(int index in data.pattern)
        {
            order.Add(index);
        }

        // 순서 섞기
        int shuffleCount = Random.Range(orderShuffleMin, orderShuffleMax + 1);
        for(int i = 0; i < shuffleCount; i++)
        {
            int a = Random.Range(0, order.Count);
            int b = Random.Range(0, order.Count);
            (order[a], order[b]) = (order[b], order[a]);
        }

        var wfs = new WaitForSeconds(data.interval);

        foreach(int index in order)
        {
            BossPunch punch = Instantiate(punchPrefab, spawnPoints[index].position, Quaternion.identity);
            spawnedPunch++;
            punch.OnDestroyed += () => spawnedPunch--;
            yield return wfs;
        }

        isSpawning = false;
    }
}

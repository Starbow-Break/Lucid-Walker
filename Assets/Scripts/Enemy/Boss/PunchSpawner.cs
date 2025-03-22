using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PunchSpawner : MonoBehaviour
{
    [SerializeField, Min(1)] int count = 3;
    [SerializeField, Min(1.0f)] float interval = 4.0f;
    [SerializeField] GameObject punchPrefab;

    public void SpawnPunch()
    {
        StartCoroutine(SpawnSequence());
    }

    private IEnumerator SpawnSequence()
    {
        for(int i = 0; i < count; i++) {
            Instantiate(punchPrefab, new Vector3(140f, -25f, 0f), Quaternion.identity);
            yield return new WaitForSeconds(interval);
        }
    }
}

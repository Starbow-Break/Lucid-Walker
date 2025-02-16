using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskMonsterSpawner : MonoBehaviour
{
    [SerializeField, Min(1)] int spawnCount = 5;
    [SerializeField] AttackRange attackRangePrefab;
    [SerializeField] MonsterBullet bulletPrefab;
    [SerializeField, Min(0.0f)] float attackDelay;
    [SerializeField, Min(0.0f)] float interval;

    List<AttackRange> spawnedAttackRange = new();
    List<MonsterBullet> spawnedBullets = new();

    public void SpawnMonsters() {
        StartCoroutine(SpawnMonstersFlow());
    }

    IEnumerator SpawnMonstersFlow() {
        UpdateSpawnedList();

        for(int i = 0; i < spawnCount; i++) {
            Vector2 end = new(Random.Range(-13.5f, 13.5f), -5.5f);
            Vector2 start;

            Vector2[] startPositions = new Vector2[2];
            startPositions[0] = new(-17.0f, Random.Range(2.0f, 6.0f));
            startPositions[1] = new(17.0f, Random.Range(2.0f, 6.0f));

            if(end.x < -3.0f) {
                start = startPositions[1];
            }
            else if(end.x > 3.0f) {
                start = startPositions[0];
            }
            else {
                start = startPositions[Random.Range(0, 2)];
            }

            StartCoroutine(Attack(start, end, attackDelay));

            yield return new WaitForSeconds(interval);
        }
    }

    void UpdateSpawnedList() {
        List<MonsterBullet> spawnedBulletsTemp = new();
        foreach(MonsterBullet bullet in spawnedBullets) {
            if(bullet != null) {
                spawnedBulletsTemp.Add(bullet);
            }
        }
        spawnedBullets = spawnedBulletsTemp;

        List<AttackRange> spawnedAttackRangeTemp = new();
        foreach(AttackRange attackRange in spawnedAttackRange) {
            if(attackRange != null) {
                spawnedAttackRangeTemp.Add(attackRange);
            }
        }
        spawnedAttackRange = spawnedAttackRangeTemp;
    }

    public void StopSpawnAndReset() {
        StopAllCoroutines();

        foreach(MonsterBullet monster in spawnedBullets) {
            if(monster != null) {
                Destroy(monster.gameObject);
            }
        }

        foreach(AttackRange attackRange in spawnedAttackRange) {
            if(attackRange != null) {
                Destroy(attackRange.gameObject);
            }
        }

        spawnedBullets.Clear();
        spawnedAttackRange.Clear();
    }

    IEnumerator Attack(Vector3 start, Vector3 end, float attackDelay) {
        yield return AttackReady(start, end, attackDelay);
        SpawnMonster(start, end - start);
    }

    // 공격 준비
    IEnumerator AttackReady(Vector3 start, Vector3 end, float time) {
        Vector2 spawnPoint = (start + end) / 2;
        AttackRange attackRange = Instantiate(attackRangePrefab, spawnPoint, Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2((end - start).y, (end - start).x) * 180.0f / Mathf.PI));
        
        spawnedAttackRange.Add(attackRange);

        attackRange.transform.localScale = new(60.0f, 1.2f, 1.0f);

        float currentTime = 0.0f;
        while(attackRange != null && currentTime < time) {
            yield return null;
            currentTime += Time.deltaTime;
            attackRange.SetProgress(currentTime / time);
        }

        Destroy(attackRange.gameObject);
    }

    // 총알 스폰
    void SpawnMonster(Vector3 start, Vector3 direction) {
        MonsterBullet bullet = Instantiate(bulletPrefab, start, Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * 180.0f / Mathf.PI));
        spawnedBullets.Add(bullet);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMaskMonsterSkill : Skill
{
    [SerializeField] Animator casterAnimator;
    [SerializeField, Min(1)] int spawnCount = 5;
    [SerializeField] GameObject attackRangePrefab;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField, Min(0.0f)] float attackDelay;
    [SerializeField, Min(0.0f)] float interval;

    List<GameObject> spawnedAttackRange;
    List<GameObject> spawnedBullet;

    bool spawningBullet;

    void Start() {
        spawnedAttackRange = new List<GameObject>();
        spawnedBullet = new List<GameObject>();
        spawningBullet = false;
    }

    protected override IEnumerator SkillFlow()
    {
        spawnedBullet.Clear();
        spawningBullet = false;

        casterAnimator.SetTrigger("whistle");

        yield return new WaitUntil(() => spawningBullet);
        yield return new WaitForSeconds(1.5f);

        for(int i = 0; i < spawnCount; i++) {
            Vector2 end = new(Random.Range(-13.5f, 13.5f), -5.5f);
            Vector2 start = Vector2.zero;

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

    // 스킬 리셋 로직
    protected override void DoReset() {
        StopAllCoroutines();

        foreach(GameObject bullet in spawnedBullet) {
            if(bullet != null) {
                Destroy(bullet);
            }
        }

        foreach(GameObject attackRange in spawnedAttackRange) {
            if(attackRange != null) {
                Destroy(attackRange);
            }
        }

        spawnedBullet.Clear();
    }

    // 스폰 시작을 알림
    public void StartSpawn() {
        spawningBullet = true;
    }

    // 공격
    IEnumerator Attack(Vector3 start, Vector3 end, float attackDelay) {
        yield return AttackReady(start, end, attackDelay);
        SpawnBullet(start, end - start);
    }

    // 공격 준비
    IEnumerator AttackReady(Vector3 start, Vector3 end, float time) {
        Vector2 spawnPoint = (start + end) / 2;
        GameObject attackRangeObj = Instantiate(attackRangePrefab, spawnPoint, Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2((end - start).y, (end - start).x) * 180.0f / Mathf.PI));
        spawnedAttackRange.Add(attackRangeObj);
        attackRangeObj.transform.localScale = new(60.0f, 1.2f, 1.0f);

        float currentTime = 0.0f;
        AttackRange attackRange = attackRangeObj.GetComponent<AttackRange>();
        while(attackRange != null && currentTime < time) {
            yield return null;
            currentTime += Time.deltaTime;
            attackRange.SetProgress(currentTime / time);
        }

        Destroy(attackRange.gameObject);
    }

    // 총알 스폰
    void SpawnBullet(Vector3 start, Vector3 direction) {
        GameObject bullet = Instantiate(bulletPrefab, start, Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * 180.0f / Mathf.PI));
        spawnedBullet.Add(bullet);
    }
}

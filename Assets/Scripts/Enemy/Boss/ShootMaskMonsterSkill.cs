using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ShootMaskMonsterSkill : Skill
{
    [SerializeField] GameObject attackRangePrefab;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField, Min(0.0f)] float attackDelay;
    [SerializeField, Min(0.0f)] float interval;

    List<GameObject> spawnedAttackRange;
    List<GameObject> spawnedBullet;

    void Start() {
        spawnedAttackRange = new List<GameObject>();
        spawnedBullet = new List<GameObject>();
    }

    protected override IEnumerator SkillFlow()
    {
        spawnedBullet.Clear();

        for(int i = 0; i < 5; i++) {
            Vector2 start = new((1.0f - 2.0f * Random.Range(0, 2)) * 17.0f, Random.Range(0.0f, 6f));
            Vector2 end = new(Random.Range(-13.5f, 13.5f), -5.5f);

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

    // 공격
    IEnumerator Attack(Vector3 start, Vector3 end, float attackDelay) {
        yield return AttackReady(start, end, attackDelay);
        SpawnBullet(start, end - start, 3.0f);
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
    void SpawnBullet(Vector3 start, Vector3 direction, float time) {
        GameObject bullet = Instantiate(bulletPrefab, start, Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(direction.y, direction.x) * 180.0f / Mathf.PI));
        spawnedBullet.Add(bullet);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMaskMonsterSkill : Skill
{
    [SerializeField] Animator casterAnimator;
    [SerializeField] MaskMonsterSpawner spawner;

    bool spawnBulletFlag;

    void Start() {
        spawnBulletFlag = false;
    }

    protected override IEnumerator SkillFlow()
    {
        casterAnimator.SetTrigger("whistle");

        yield return new WaitUntil(() => spawnBulletFlag);
        yield return new WaitForSeconds(1.5f);

        spawner.SpawnMonsters();
        spawnBulletFlag = false;
    }

    // 스킬 리셋 로직
    protected override void DoReset() {
        spawner.StopSpawnAndReset();
    }

    // 스폰 시작을 알림
    public void StartSpawn() {
        spawnBulletFlag = true;
    }
}

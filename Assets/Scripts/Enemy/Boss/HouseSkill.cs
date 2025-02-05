using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class HouseSkill : Skill
{
    [SerializeField] MonsterHouse spawnHouse; // 소환할 몬스터 집
    [SerializeField] Transform spawnPoint; // 집을 스폰할 위치

    MonsterHouse spawnedHouse;

    protected override IEnumerator SkillFlow() {
        spawnedHouse = Instantiate(spawnHouse, spawnPoint.position, Quaternion.identity);
        yield return null;
    }

    protected override void DoReset() {
        if(spawnedHouse != null) {
            spawnedHouse.DeadAllSpawnedMonster();
            Destroy(spawnedHouse);
            spawnedHouse = null;
        }
    }
}

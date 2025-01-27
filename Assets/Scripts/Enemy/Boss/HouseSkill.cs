using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class HouseSkill : Skill
{
    [SerializeField] MonsterHouse spawnHouse; // 소환할 몬스터 집
    [SerializeField] Transform spawnPoint; // 집을 스폰할 위치

    protected override void Play() {
        StartCoroutine(SkillFlow());
    }

    protected override IEnumerator SkillFlow() {
        Instantiate(spawnHouse, spawnPoint.position, Quaternion.identity);
        yield return null;
    }
}

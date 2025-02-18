using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public class HouseSkill : Skill
{
    [SerializeField] MonsterHouse spawnHouse; // 소환할 몬스터 집
    [SerializeField] Transform spawnPoint; // 집을 스폰할 위치

    MonsterHouse spawnedHouse;
    List<GameObject> spawnedMonsters;

    void Start() {
        spawnedMonsters = new();
    }

    protected override IEnumerator SkillFlow() {
        // 스폰된 몬스터 목록 정리
        List<GameObject> temp = new();
        foreach(GameObject monster in spawnedMonsters) {
            if(monster != null) {
                WalkingMonster walkingMonster = monster.GetComponent<WalkingMonster>();
                if(!walkingMonster.isDead) {
                    temp.Add(monster);
                }
            }
        }

        spawnedMonsters = temp;

        spawnedHouse = Instantiate(spawnHouse, spawnPoint.position, Quaternion.identity);
        spawnedHouse.SetSpawner(this);
        yield return null;
    }

    public override void ResetSkill() {
        base.ResetSkill();

        if(spawnedHouse != null) {
            DieAllSpawnedMonster();
            Destroy(spawnedHouse.gameObject);
            spawnedHouse = null;
        }
    }

    public void AddSpawnedMonster(GameObject monster) {
        spawnedMonsters.Add(monster);
    }

    // 스폰된 모든 몬스터 사망
    public void DieAllSpawnedMonster() {
        Debug.Log(spawnedMonsters.Count);
        foreach(GameObject monster in spawnedMonsters) {
            if(!monster.IsDestroyed()) {
                WalkingMonster walkingMonster = monster.GetComponent<WalkingMonster>();
                if(!walkingMonster.isDead) {
                    walkingMonster.Die();
                }
            }
        }

        spawnedMonsters.Clear();
    }
}

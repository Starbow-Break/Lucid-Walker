using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MaskBossStats : MonoBehaviour, IDamageable
{
    [SerializeField] BossStatsData statsData;

    float hp;
    Coroutine coroutine = null;

    void Start() {
        hp = statsData.hp;
        Debug.Log("Boss Hp : " + hp);
    }

    void Update() {
        if(coroutine == null && statsData.healthType == BossStatsData.HealthType.TIME_ATTACK) {
            coroutine = StartCoroutine(TimeAttackFlow());
        }
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        hp -= damage;

        if(hp <= 0) {
            // 사망 또는 다음 페이즈로 이동
        }
    }

    IEnumerator TimeAttackFlow() {
        while(hp > 0) {
            yield return new WaitForSeconds(1.0f);
            TakeDamage(1, transform);
            Debug.Log("Boss Hp : " + hp);
        }
    }
}

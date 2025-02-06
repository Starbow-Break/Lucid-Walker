using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MaskBossStats : MonoBehaviour, IDamageable
{
    [SerializeField] BossStatsData statsData;
    [SerializeField] HealthBar healthBar;

    int maxHp;
    int hp;
    int maxSp;
    public int sp { get; private set; }
    Coroutine coroutine = null;
    MaskBoss owner;

    void Start() {
        owner = GetComponent<MaskBoss>();

        maxHp = statsData.hp;
        hp = maxHp;
        maxSp = statsData.sp;
        sp = maxSp;
        healthBar.SetValue(hp, maxHp);
        Debug.Log("Boss Hp : " + hp);
    }

    void Update() {
        if(owner.battle && coroutine == null && statsData.healthType == BossStatsData.HealthType.TIME_ATTACK) {
            coroutine = StartCoroutine(TimeAttackFlow());
        }
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        hp -= damage;
        healthBar.SetValue(hp, maxHp);

        if(hp <= 0) {
            owner.Die();
        }
    }

    public void SpendSp(int value) => sp -= value;
    public void RecoverySp(int value) => sp += value;

    IEnumerator TimeAttackFlow() {
        while(hp > 0) {
            yield return new WaitForSeconds(0.01f);
            TakeDamage(1, transform);
            //Debug.Log("Boss Hp : " + hp);
        }
    }
}

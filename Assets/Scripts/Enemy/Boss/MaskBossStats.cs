using System;
using System.Collections;
using UnityEngine;

public class MaskBossStats : MonoBehaviour, IDamageable
{
    [SerializeField] BossStatsData statsData;

    int maxHp;
    int hp;
    Coroutine coroutine = null;
    MaskBoss owner;

    public event Action<float> OnDamage;

    public float attackBatTime
    {
        get { return statsData.attackBatTime; }
    }

    void Start() {
        owner = GetComponent<MaskBoss>();
    }

    void OnEnable() {
        maxHp = statsData.hp;
        hp = maxHp;
    }

    void Update() {
        if(owner.battle && coroutine == null && statsData.healthType == BossStatsData.HealthType.TIME_ATTACK) {
            coroutine = StartCoroutine(TimeAttackFlow());
        }
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if(hp <= 0) return;

        hp -= damage;
        OnDamage?.Invoke(Mathf.Clamp01(1f * hp / maxHp) * 100f);

        if (hp <= 0)
        {
            owner.Die();
        }
    }

    IEnumerator TimeAttackFlow() {
        while(hp > 0) {
            yield return new WaitForSeconds(0.01f);
            TakeDamage(1, transform);
        }
    }
}

using System.Collections;
using UnityEngine;

public class MaskBossStats : MonoBehaviour, IDamageable
{
    [SerializeField] BossStatsData statsData;
    [SerializeField] HealthBar healthBar;

    int maxHp;
    int hp;
    Coroutine coroutine = null;
    MaskBoss owner;

    public float attackBatTime {
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

        UpdateUI();
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        hp -= damage;

        if(hp <= 0) {
            owner.Die();
        }
    }

    void UpdateUI() {
        healthBar.SetValue(hp, maxHp);
    }

    IEnumerator TimeAttackFlow() {
        while(hp > 0) {
            yield return new WaitForSeconds(0.01f);
            TakeDamage(1, transform);
            //Debug.Log("Boss Hp : " + hp);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskBossStats : MonoBehaviour, IDamageable
{
    [SerializeField] BossStatsData statsData;
    [SerializeField] List<SpriteRenderer> _renderers;

    [SerializeField] Color _hurtColor;

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

    void Update()
    {
        if (owner.battle && coroutine == null && statsData.healthType == BossStatsData.HealthType.TIME_ATTACK)
        {
            coroutine = StartCoroutine(TimeAttackFlow());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(HurtFlow());
        }
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if(hp <= 0) return;

        hp -= damage;
        Debug.Log($"Boss Hp : {hp}");
        OnDamage?.Invoke(Mathf.Clamp01(1f * hp / maxHp) * 100f);
        StartCoroutine(HurtFlow());

        if (hp <= 0)
        {
            owner.Die();
        }
    }

    IEnumerator HurtFlow()
    {
        Debug.Log("Hurt");
        yield return ColorChangeFlow(_renderers, Color.white, _hurtColor, 0.1f);
        yield return ColorChangeFlow(_renderers, _hurtColor, Color.white, 0.1f);
    }

    IEnumerator ColorChangeFlow(List<SpriteRenderer> renderers, Color start, Color end, float duration)
    {
        float currentTime = 0.0f;
        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            Color color = Color.Lerp(start, end, currentTime / duration);
            foreach(var renderer in renderers) {
                renderer.color = color;
            }
            yield return null;
        }
    }

    IEnumerator TimeAttackFlow() {
        while(hp > 0) {
            yield return new WaitForSeconds(0.01f);
            TakeDamage(1, transform);
        }
    }
}

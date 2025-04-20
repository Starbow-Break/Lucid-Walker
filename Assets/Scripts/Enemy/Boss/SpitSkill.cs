using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitSkill : Skill
{
    [SerializeField] private BulletSpawner bulletSpawner;
    [SerializeField, Min(1)] private int attackCount = 2;
    [SerializeField, Min(0.0f)] private float attackInterval = 1f;
    private Animator casterAnim;

    private void Awake()
    {
        casterAnim = GetComponent<Animator>();
    }

    protected override IEnumerator SkillFlow()
    {
        for(int i = 0; i < attackCount; i++)
        {
            casterAnim.SetTrigger("spit");
            yield return null;
            yield return new WaitForSeconds(casterAnim.GetCurrentAnimatorStateInfo(0).length + attackInterval);
        }
    }

    public void SpawnBullets()
    {
        bulletSpawner.Spawn();
    }
}

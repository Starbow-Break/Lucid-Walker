using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitSkill : Skill
{
    [SerializeField] private BulletSpawner bulletSpawner;
    private Animator casterAnim;

    private void Awake()
    {
        casterAnim = GetComponent<Animator>();
    }

    protected override IEnumerator SkillFlow()
    {
        casterAnim.SetTrigger("spit");
        yield return null;
    }

    public void SpawnBullets()
    {
        bulletSpawner.Spawn();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAttackSkill : Skill
{
    [SerializeField] PunchSpawner punchSpawner;
    MaskBossPhase3 maskBoss;
    Rigidbody2D maskBossRigidBody;

    void Start()
    {
        maskBoss = GetComponent<MaskBossPhase3>();
        maskBossRigidBody = GetComponent<Rigidbody2D>();
    }

    protected override IEnumerator SkillFlow() {
        maskBoss.StopTimer();
        
        float gravityScale = maskBossRigidBody.gravityScale;

        maskBoss.TriggerJump();
        yield return new WaitUntil(() => maskBossRigidBody == null || maskBossRigidBody.velocity.y <= 0.0f && !maskBoss.isGround);

        maskBossRigidBody.velocity = Vector2.zero;
        maskBossRigidBody.gravityScale = 0.0f;

        punchSpawner.SpawnPunch();
        yield return null;
        yield return new WaitWhile(() => punchSpawner.isSpawning || punchSpawner.spawnedPunch > 0);

        maskBoss.Flip();
        maskBossRigidBody.gravityScale = gravityScale;
    }
}

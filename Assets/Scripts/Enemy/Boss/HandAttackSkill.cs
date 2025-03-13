using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAttackSkill : Skill
{
    MaskBossPhase3 maskBoss;
    Rigidbody2D maskBossRigidBody;

    void Start()
    {
        maskBoss = GetComponent<MaskBossPhase3>();
        maskBossRigidBody = GetComponent<Rigidbody2D>();
    }

    protected override IEnumerator SkillFlow() {
        float gravityScale = maskBossRigidBody.gravityScale;

        maskBoss.TriggerJump();
        yield return new WaitUntil(() => maskBossRigidBody == null || maskBossRigidBody.velocity.y <= 0.0f && !maskBoss.isGround);

        maskBossRigidBody.velocity = Vector2.zero;
        maskBossRigidBody.gravityScale = 0.0f;

        // TODO : 손으로 내려찍기
        for(int i = 0; i < 3; i++) {
            maskBoss.Shake();
            yield return new WaitForSeconds(1.0f);
        }

        maskBoss.Flip();
        maskBossRigidBody.gravityScale = gravityScale;
    }
}

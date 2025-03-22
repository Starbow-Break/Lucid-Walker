using System;
using System.Collections;
using UnityEngine;

public class TongueSkill : Skill
{
    [SerializeField, Min(0)] int count = 3;     // 공격 횟수
    [SerializeField] float bodyMin;
    [SerializeField] float bodyMax;
    [SerializeField] float bodyVelocity = 2.0f;

    MaskBossPhase3 maskBoss;
    Animator casterAnimator;

    void Start()
    {
        maskBoss = GetComponent<MaskBossPhase3>();
        casterAnimator = GetComponent<Animator>();
    }

    protected override IEnumerator SkillFlow() {
        casterAnimator.SetBool("tongue_ready", true);
        casterAnimator.SetFloat("tongue_position", BlendValue(maskBoss.bodyLocalPosition.y));

        float currentValue = casterAnimator.GetFloat("tongue_position");
        for(int i = 0; i < count; i++) {
            float targetValue = 1.0f * i / (count - 1);
            float valueVelocity = (targetValue > currentValue ? bodyVelocity : -bodyVelocity) / (bodyMax - bodyMin);
            Predicate<float> check = valueVelocity > 0 ? (v) => v > 0 : (v) => v < 0;

            Debug.Log("What?");

            while(check(targetValue - currentValue)) {
                currentValue += valueVelocity * Time.deltaTime;
                casterAnimator.SetFloat("tongue_position", currentValue);
                yield return null;
            }

            if(i <= count) {
                casterAnimator.SetTrigger("tongue");
                yield return new WaitForSeconds(0.5f);
            }
        }

        casterAnimator.SetBool("tongue_ready", false);
    }

    float BlendValue(float bodyPos)
    {
        return (bodyPos - bodyMin) / (bodyMax - bodyMin);
    }
}

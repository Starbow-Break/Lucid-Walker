using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueSkill : Skill
{
    public readonly float BodyMin = 4.2f;
    public readonly float BodyMax = 8.34f;

    public struct TongueSkillData
    {
        public float interval;
        public List<float> normalizeValues;
    }

    [SerializeField, Min(0)] int count = 3;     // 공격 횟수
    [SerializeField] float bodyNormalizeVelocity = 2.0f;    // 정규화 기준 속도
    [SerializeField] List<TongueSkillData> patternDatas;

    private MaskBossPhase3 maskBoss;
    private Animator casterAnimator;

    void Start()
    {
        maskBoss = GetComponent<MaskBossPhase3>();
        casterAnimator = GetComponent<Animator>();
    }

    protected override IEnumerator SkillFlow() {
        casterAnimator.SetBool("tongue_ready", true);

        float initValue = (maskBoss.bodyLocalPosition.y - BodyMin) / (BodyMax - BodyMin);
        casterAnimator.SetFloat("tongue_position", initValue);

        float currentValue = casterAnimator.GetFloat("tongue_position");
        var data = patternDatas[UnityEngine.Random.Range(0, patternDatas.Count)];
        foreach(float targetValue in data.normalizeValues) {
            float valueVelocity = targetValue > currentValue ? bodyNormalizeVelocity : -bodyNormalizeVelocity;
            Predicate<float> check = valueVelocity > 0 ? (v) => v > 0 : (v) => v < 0;

            Debug.Log("What?");

            while(check(targetValue - currentValue)) {
                currentValue += valueVelocity * Time.deltaTime;
                casterAnimator.SetFloat("tongue_position", currentValue);
                yield return null;
            }

            casterAnimator.SetTrigger("tongue");
            yield return new WaitForSeconds(data.interval);
        }

        casterAnimator.SetBool("tongue_ready", false);
    }
}

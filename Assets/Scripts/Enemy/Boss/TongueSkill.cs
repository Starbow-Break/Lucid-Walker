using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TongueSkill : Skill
{
    public readonly float BodyMin = 3.26f;
    public readonly float BodyMax = 8.34f;

    // [Serializable]
    // public struct SerializableTuple<T, U>
    // {
    //     public T Item1;
    //     public U Item2;
    // }

    // [Serializable]
    // public struct TongueSkillData
    // {
    //     public float interval;
    //     public List<SerializableTuple<float, float>> normalizeValueRanges;
    // }

    [SerializeField] float bodyNormalizeVelocity = 2.0f;    // 정규화 기준 속도
    [SerializeField] float attackCount = 3;
    [SerializeField] float interval = 2.0f;

    // [SerializeField] List<TongueSkillData> patternDatas;
    // [SerializeField] int orderShuffleMin;
    // [SerializeField] int orderShuffleMax;

    private MaskBossPhase3 maskBoss;
    private Animator casterAnimator;

    void Start()
    {
        maskBoss = GetComponent<MaskBossPhase3>();
        casterAnimator = GetComponent<Animator>();
    }

    protected override IEnumerator SkillFlow()
    {
        casterAnimator.SetBool("tongue_ready", true);

        float initValue = (maskBoss.bodyLocalPosition.y - BodyMin) / (BodyMax - BodyMin);
        casterAnimator.SetFloat("tongue_position", initValue);

        float currentValue = casterAnimator.GetFloat("tongue_position");

        float targetValue = 0f;
        float valueVelocity = targetValue > currentValue ? bodyNormalizeVelocity : -bodyNormalizeVelocity;
        Predicate<float> check = valueVelocity > 0 ? (v) => v > 0 : (v) => v < 0;

        while (check(targetValue - currentValue))
        {
            currentValue += valueVelocity * Time.deltaTime;
            casterAnimator.SetFloat("tongue_position", currentValue);
            yield return null;
        }

        for (int i = 0; i < attackCount; i++)
        {
            casterAnimator.SetTrigger("tongue");
            yield return new WaitForSeconds(interval);
        }

        targetValue = initValue;
        valueVelocity = targetValue > currentValue ? bodyNormalizeVelocity : -bodyNormalizeVelocity;
        check = valueVelocity > 0 ? (v) => v > 0 : (v) => v < 0;
        while (check(targetValue - currentValue))
        {
            currentValue += valueVelocity * Time.deltaTime;
            casterAnimator.SetFloat("tongue_position", currentValue);
            yield return null;
        }

        casterAnimator.SetBool("tongue_ready", false);
    }
}

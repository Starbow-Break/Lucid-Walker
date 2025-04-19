using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TongueSkill : Skill
{
    public readonly float BodyMin = 4.2f;
    public readonly float BodyMax = 8.34f;

    [Serializable]
    public struct SerializableTuple<T, U>
    {
        public T Item1;
        public U Item2;
    }

    [Serializable]
    public struct TongueSkillData
    {
        public float interval;
        public List<SerializableTuple<float, float>> normalizeValueRanges;
    }

    [SerializeField] float bodyNormalizeVelocity = 2.0f;    // 정규화 기준 속도
    [SerializeField] List<TongueSkillData> patternDatas;
    [SerializeField] int orderShuffleMin;
    [SerializeField] int orderShuffleMax;

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
        var data = patternDatas[Random.Range(0, patternDatas.Count)];

        List<float> order = new();
        foreach(var valueRange in data.normalizeValueRanges)
        {
            order.Add(Random.Range(valueRange.Item1, valueRange.Item2));
        }

        int shuffleCount = Random.Range(orderShuffleMin, orderShuffleMax + 1);
        ListShuffler.Shuffle(order, shuffleCount);
        order.Add(initValue);

        for(int i = 0; i < order.Count; i++) {
            float targetValue = order[i];
            float valueVelocity = targetValue > currentValue ? bodyNormalizeVelocity : -bodyNormalizeVelocity;
            Predicate<float> check = valueVelocity > 0 ? (v) => v > 0 : (v) => v < 0;

            while(check(targetValue - currentValue)) {
                currentValue += valueVelocity * Time.deltaTime;
                casterAnimator.SetFloat("tongue_position", currentValue);
                yield return null;
            }

            if(i != order.Count - 1)
            {
                casterAnimator.SetTrigger("tongue");
                yield return new WaitForSeconds(data.interval);
            }
        }

        casterAnimator.SetBool("tongue_ready", false);
    }
}

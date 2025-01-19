using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSkill : Skill
{   
    [SerializeField] MaskBossLampGroup lampGroup;
    [SerializeField, Min(1)] int count = 5; // 조명이 켜지는 횟수
    [SerializeField, Min(0)] int lampCount = 0; // 켜지는 조명의 갯수
    [SerializeField, Min(0.0f)] float attackDelay = 2.0f; // 공격 딜레이
    [SerializeField, Min(0.0f)] float turnOffTimeAfterAttack = 2.0f; // 공격 이후 조명 끄기까지의 시간
    [SerializeField, Min(0.0f)] float interval = 1.0f; // 조명 패턴 간 시간 간격

    private void OnValidate() {
        lampCount = Mathf.Min(lampCount, lampGroup == null ? 0 : lampGroup.LampCount);  
    }

    protected override IEnumerator SkillFlow()
    {
        // 조명 내리기
        yield return lampGroup.MoveDown();

        for(int i = 0; i < count; i++) {
            List<int> lampIndex = GetRandomValues(0, lampGroup.LampCount, lampCount);

            // 공격 전 대기
            yield return AttackReady(lampIndex, attackDelay);
            
            // 공격
            yield return Attack(lampIndex);

            // 공격 후 대기
            yield return new WaitForSeconds(turnOffTimeAfterAttack);
            
            // 조명 끄기
            lampGroup.TurnOffAllLamps();

            // 다음 조명 패턴 시전하기 전에 대기
            if(i < count - 1) {
                yield return new WaitForSeconds(interval);
            }
        }

        // 조명 올리기
        yield return lampGroup.MoveUp();
    }

    // 공격 전 대기
    IEnumerator AttackReady(List<int> lampIdxList, float time) {
        // 노란색 불빛으로 조명이 켜짐
        foreach(int index in lampIdxList) {
            lampGroup.TurnOnYellowLight(index);
        }

        float currentTime = 0.0f;
        while(currentTime < time) {
            yield return null;
            currentTime += Time.deltaTime;
            foreach(int index in lampIdxList) {
                lampGroup.GetLamp(index).SetAttackRangeProgress(currentTime / time);
            }
        }
    }

    // 공격
    IEnumerator Attack(List<int> lampIdxList) {
        // 붉은색 불빛으로 바꾸면서 공격
        foreach(int index in lampIdxList) {
            lampGroup.TurnOnRedLight(index);
        }
        yield return null;
    }

    // [min, max)범위에서 서로 다른 num개의 값을 선택
    List<int> GetRandomValues(int min, int max, int num) {
        if(num < 0 || num > max-min) {
            throw new System.ArgumentException("인자 값이 잘못되었습니다. num값은 0 이상 max-min 이하여야 합니다.");
        }

        bool[] check = new bool[max-min];

        if(num <= (max-min)/2) {
            int cnt = 0;
            while(cnt < num) {
                int value = Random.Range(0, max-min);
                if(!check[value]) {
                    check[value] = true;
                    ++cnt;
                }
            }
        }
        else {
            for(int i = 0; i < max-min; i++) {
                check[i] = true;
            }

            int cnt = max-min;
            while(cnt > num) {
                int value = Random.Range(0, max-min);
                if(!check[value]) {
                    check[value] = false;
                    --cnt;
                }
            }
        }

        List<int> ret = new();
        for(int i = 0; i < max-min; i++) {
            if(check[i]) {
                ret.Add(min+i);
            }
        }

        return ret;
    }
}

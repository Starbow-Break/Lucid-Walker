using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSkill : Skill
{   
    [System.Serializable]
    struct LightSkillPatternData {
        public List<int> pattern; // 조명 패턴
    }

    [SerializeField] MaskBossLampGroup lampGroup;
    [SerializeField] List<GameObject> theaterTiles; 
    [SerializeField, Min(1)] int count = 5; // 스킬 한번 당 조명이 켜지는 횟수
    [SerializeField] List<LightSkillPatternData> patternDatas; // 조명 패턴
    [SerializeField, Min(0.0f)] float attackDelay = 2.0f; // 공격 딜레이
    [SerializeField, Min(0.0f)] float turnOffTimeAfterAttack = 2.0f; // 공격 이후 조명 끄기까지의 시간
    [SerializeField, Min(0.0f)] float interval = 1.0f; // 조명 패턴 간 시간 간격

    int patternIndex = 0;
    Animator casterAnimator; // 시전자의 Animator

    void Start() {
        casterAnimator = GetComponent<Animator>();
    }   

    protected override IEnumerator SkillFlow()
    {
        if(patternDatas.Count == 0) {
            throw new System.Exception("patternDatas에 최소 한개 이상의 원소가 있어야 합니다!");
        }

        casterAnimator.SetTrigger("light");

        yield return new WaitUntil(() => lampGroup.gameObject.activeSelf);

        // 조명 내리기
        lampGroup.MoveDown(1.5f);
        yield return new WaitForSeconds(1.5f);

        for(int i = 0; i < count; i++) {
            // 공격 전 대기
            yield return AttackReady(patternDatas[patternIndex].pattern, attackDelay);
            
            // 공격
            yield return Attack(patternDatas[patternIndex].pattern);

            // 공격 후 대기
            yield return new WaitForSeconds(turnOffTimeAfterAttack);
            
            // 조명 끄기
            lampGroup.TurnOffAllLamps();

            // 다음 조명 패턴 시전하기 전에 대기
            if(i < count - 1) {
                yield return new WaitForSeconds(interval);
            }

            patternIndex = (patternIndex + 1) % patternDatas.Count;
        }

        // 조명 올리기
        lampGroup.MoveUp(1.5f);
        yield return new WaitForSeconds(1.5f);

        SetInActiveLampGroup();
    }

    // 스킬 리셋 로직
    public override void ResetSkill() {
        base.ResetSkill();
        lampGroup.Stop();
    }

    void SetActiveLampGroup() {
        lampGroup.gameObject.SetActive(true);
    }

    void SetInActiveLampGroup() {
        lampGroup.gameObject.SetActive(false);
    }

    // 공격 전 대기
    IEnumerator AttackReady(List<int> lampIdxList, float time) {
        // 노란색 불빛으로 조명이 켜짐
        foreach(int index in lampIdxList) {
            lampGroup.TurnOnYellowLight(index);
        }

        yield return new WaitForSeconds(time);
    }

    // 공격
    IEnumerator Attack(List<int> lampIdxList) {
        // 붉은색 불빛으로 바꾸면서 공격
        foreach(int index in lampIdxList) {
            lampGroup.TurnOnRedLight(index);

            Animator tileAnim = theaterTiles[index].GetComponent<Animator>();
            if(tileAnim != null) {
                tileAnim.speed = 1 / turnOffTimeAfterAttack;
                tileAnim.SetTrigger("attack");
            }
            
        }
        yield return null;
    }
}

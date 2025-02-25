using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;

public class MaskBoss : MonoBehaviour
{
    float attackCoolDownRemain = 0.0f;
    public bool battle { get; private set; }
    bool isDead = false;

    MaskBossStats stats;

    public UnityEvent dieEvent;

    // Start is called before the first frame update
    void Start()
    {
        battle = false;

        stats = GetComponent<MaskBossStats>();

        lightSkill = GetComponent<LightSkill>();
        houseSkill = GetComponent<HouseSkill>();
        shootMaskMonsterSkill = GetComponent<ShootMaskMonsterSkill>();
    }

    // Update is called once per frame
    void Update()
    {   
        // 싸우는 상태가 아니거나 사망 상태이면 아무 행동도 하지 않는다.
        if(!battle || isDead) return;

        attackCoolDownRemain -= Time.deltaTime;

        if(attackCoolDownRemain <= 0.0f) {
            attackCoolDownRemain = stats.attackBatTime;
            Think();
        }
    }

    public void BattleStart() {
        battle = true;
    }

    public void Die() {
        Debug.Log("Die");
        
        // 사망 상태 전환
        isDead = true;

        // 스킬 리셋
        lightSkill.ResetSkill();
        houseSkill.ResetSkill();
        shootMaskMonsterSkill.ResetSkill();

        //페이즈 전환 처리
        dieEvent.Invoke();
    }

    #region AI
    // 스킬의 우선순위가 높은 순서대로 정렬된 리스트
    LinkedList<int> skillProrityList = new LinkedList<int>(new List<int>{0, 1, 2});
    
    // 생각
    void Think() {
        LinkedListNode<int> node = skillProrityList.First;

        while(node != null) {
            if(TryCastSkill(node.Value)) {
                skillProrityList.Remove(node);
                skillProrityList.AddLast(node);
                break;
            }
            node = node.Next;
        }
    }

    // 스킬 사용 시도, 스킬 시전 여부를 반환
    bool TryCastSkill(int skillId) {
        Skill skill = GetSkillWithId(skillId);
        if(skill.isReady) {
            skill.Cast();
            return true;
        }
        return false;
    }
    #endregion

    #region SKILL
    LightSkill lightSkill;
    HouseSkill houseSkill;
    ShootMaskMonsterSkill shootMaskMonsterSkill;

    // 스킬 아이디로부터 스킬 획득
    Skill GetSkillWithId(int skillId) {
        return skillId switch
        {
            0 => lightSkill,
            1 => houseSkill,
            2 => shootMaskMonsterSkill,
            _ => null,
        };
    }
    #endregion
}

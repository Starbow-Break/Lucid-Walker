using UnityEngine;

public class MaskBoss : MonoBehaviour
{
    [SerializeField] BossStageManager bossStageManager;
    [SerializeField] Animator anim;
    [SerializeField] float coolDown = 10.0f;

    int turn;
    float coolDownRemain;
    public bool battle { get; private set; }
    bool isDead = false;

    MaskBossStats stats;

    // Start is called before the first frame update
    void Start()
    {
        turn = -1;
        coolDownRemain = 0.0f;
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

        if(coolDownRemain >= 0.0f) {
            coolDownRemain -= Time.deltaTime;
        }
        else {
            stats.RecoverySp(1);
            int think = Think();
            Debug.Log("Think : " + think);
            UseSkill(think);
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
        lightSkill.SkillReset();
        houseSkill.SkillReset();
        shootMaskMonsterSkill.SkillReset();

        //페이즈 전환 처리
        bossStageManager.StartNextPhase();
    }

    #region AI
    // 생각
    int Think() {
        if(lightSkill.sp <= stats.sp) {
            return 0;
        }

        turn = (turn + 1) % 2;
        return turn + 1;
    }

    // 스킬 사용
    void UseSkill(int skillId) {
        switch(skillId) {
            case 0:
            UseLightSkill();
            break;
            case 1:
            UseHouseSkill();
            break;
            case 2:
            UseShootMaskMonsterSkill();
            break;
        }
    }
    #endregion

    #region SKILL
    LightSkill lightSkill;
    HouseSkill houseSkill;
    ShootMaskMonsterSkill shootMaskMonsterSkill;

    void UseLightSkill() => CastSkill(lightSkill);
    void UseHouseSkill() => CastSkill(houseSkill);
    void UseShootMaskMonsterSkill() => CastSkill(shootMaskMonsterSkill);

    // 스킬 시전
    void CastSkill(Skill skill) { 
        stats.SpendSp(skill.sp);
        coolDownRemain = coolDown;
        skill.Cast();
    }
    #endregion
}

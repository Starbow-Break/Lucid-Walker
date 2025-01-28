using UnityEngine;

public class MaskBoss : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField, Min(0)] int maxSp = 4;
    [SerializeField] float coolDown = 10.0f;

    int turn;
    float coolDownRemain;
    float sp = 0;

    MaskBossStats stats;

    // Start is called before the first frame update
    void Start()
    {
        turn = -1;
        sp = maxSp;
        coolDownRemain = coolDown;

        stats = GetComponent<MaskBossStats>();

        lightSkill = GetComponent<LightSkill>();
        houseSkill = GetComponent<HouseSkill>();
        shootMaskMonsterSkill = GetComponent<ShootMaskMonsterSkill>();
    }

    // Update is called once per frame
    void Update()
    {
        if(coolDownRemain >= 0.0f) {
            coolDownRemain -= Time.deltaTime;
        }
        else {
            sp++;
            int result = Think();
            UseSkill(result);
        }

        // 스킬 테스트를 위한 코드
        // {
        //     if(Input.GetKeyDown(KeyCode.Alpha1)) {
        //         UseLightSkill();
        //     }
        //     else if(Input.GetKeyDown(KeyCode.Alpha2)) {
        //         UseHouseSkill();
        //     }
        //     else if(Input.GetKeyDown(KeyCode.Alpha3)) {
        //         UseShootMaskMonsterSkill();
        //     }
        // }
    }

    #region AI
    // 생각
    int Think() {
        if(lightSkill.sp <= sp) {
            return 0;
        }
        else {
            turn = (turn + 1) % 2;
            return turn + 1;
        }
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
        sp -= skill.sp;
        coolDownRemain = coolDown;
        skill.Cast(); 
    }
    #endregion
}

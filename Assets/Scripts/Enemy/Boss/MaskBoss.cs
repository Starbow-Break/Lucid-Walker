using UnityEngine;

public class MaskBoss : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float attackCoolDownMin = 10.0f;
    [SerializeField] float attackCoolDownMax = 20.0f;

    float coolDownRemain;

    MaskBossStats stats;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<MaskBossStats>();
        coolDownRemain = Random.Range(attackCoolDownMin, attackCoolDownMax);
    }

    // Update is called once per frame
    void Update()
    {
        if(coolDownRemain >= 0.0f) {
            coolDownRemain -= Time.deltaTime;
        }
        else {
            UseSkill();
            coolDownRemain = Random.Range(attackCoolDownMin, attackCoolDownMax);
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

    #region SKILL
    void UseSkill() {
        if(UseLightSkill()) return;
        if(UseHouseSkill()) return;
        if(UseShootMaskMonsterSkill()) return;
    }

    bool UseLightSkill() => CastSkill(GetComponent<LightSkill>());
    bool UseHouseSkill() => CastSkill(GetComponent<HouseSkill>());
    bool UseShootMaskMonsterSkill() => CastSkill(GetComponent<ShootMaskMonsterSkill>());

    bool CastSkill(Skill skill) => skill.Cast();
    #endregion
}

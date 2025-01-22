using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MaskBoss : MonoBehaviour
{
    [SerializeField] Animator anim;

    MaskBossStats stats;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<MaskBossStats>();
    }

    // Update is called once per frame
    void Update()
    {
        // 스킬 테스트를 위한 코드
        {
            if(Input.GetKeyDown(KeyCode.Alpha1)) {
                UseLightSkill();
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)) {
                UseHouseSkill();
            }
        }
    }

    #region SKILL
    void UseLightSkill() {
        anim.SetTrigger("skill_light");
    }

    void UseHouseSkill() {
        CastHouseSkill();
    }

    void CastLightSkill() {
        LightSkill skill = GetComponent<LightSkill>();
        skill.Cast();
    }

    void CastHouseSkill() {
        HouseSkill skill = GetComponent<HouseSkill>();
        skill.Cast();
    }
    #endregion
}

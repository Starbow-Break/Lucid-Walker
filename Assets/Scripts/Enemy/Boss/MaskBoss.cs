using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            else if(Input.GetKeyDown(KeyCode.Alpha2)) {
                UseHouseSkill();
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3)) {
                UseShootMaskMonstrSkill();
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

    void UseShootMaskMonstrSkill() {
        CastShootMaskMonstrSkill();
    }

    void CastLightSkill() {
        LightSkill skill = GetComponent<LightSkill>();
        skill.Cast();
    }

    void CastHouseSkill() {
        HouseSkill skill = GetComponent<HouseSkill>();
        skill.Cast();
    }

    void CastShootMaskMonstrSkill() {
        ShootMaskMonstrSkill skill = GetComponent<ShootMaskMonstrSkill>();
        skill.Cast();
    }
    #endregion
}

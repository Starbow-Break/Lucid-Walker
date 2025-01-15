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
        
    }

    IEnumerator SkillTest() {
        yield return new WaitForSeconds(5.0f);
        anim.SetTrigger("skill_light");
    }
}

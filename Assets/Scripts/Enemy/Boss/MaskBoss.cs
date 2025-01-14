using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskBoss : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SkillTest());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SkillTest() {
        while(true) {
            GetComponent<LightSkill>().Cast();
            yield return new WaitForSeconds(7.0f);
        }
    }
}

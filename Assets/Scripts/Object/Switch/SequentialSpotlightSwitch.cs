using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialSpotlightSwitch : MonoBehaviour
{
    // 목표 스포트라이트 묶음
    [SerializeField] SequentialSpotlightGroup sequentialSpotlightGroup;
    [SerializeField] Animator anim;
    [SerializeField] bool isBroken;

    SpriteRenderer sr; // 스위치 스프라이트

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim.SetBool("isOn", sequentialSpotlightGroup.isOn);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player")) {
            if(isBroken && sequentialSpotlightGroup.isOn) {
                anim.SetTrigger("broken");
            }
            
            sequentialSpotlightGroup.Switch();
            anim.SetBool("isOn", sequentialSpotlightGroup.isOn);
        }
    }
}

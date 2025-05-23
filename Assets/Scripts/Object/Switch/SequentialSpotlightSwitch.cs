using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SequentialSpotlightSwitch : MonoBehaviour
{
    // 목표 스포트라이트 묶음
    [SerializeField] SequentialSpotlightGroup sequentialSpotlightGroup;
    [SerializeField] Animator anim;
    [SerializeField] bool isBroken;

    BoxCollider2D switchCollider;

    void Awake()
    {
        switchCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        anim.SetBool("isOn", sequentialSpotlightGroup.isOn);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            sequentialSpotlightGroup.Switch();
            if (isBroken && !sequentialSpotlightGroup.isOn)
            {
                switchCollider.enabled = false;
                anim.SetTrigger("broken");
            }
            anim.SetBool("isOn", sequentialSpotlightGroup.isOn);
        }
    }
}

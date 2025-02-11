using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitchOn : MonoBehaviour
{
    [SerializeField] SequentialSpotlightGroup sequentialSpotlightGroup;
    [SerializeField] Animator anim;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Case"))
        {
            sequentialSpotlightGroup.Switch();
            sequentialSpotlightGroup.isOn = true; // 플레이어가 밟으면 켜짐
            anim.SetBool("isOn", true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Case"))
        {
            sequentialSpotlightGroup.Switch();
            sequentialSpotlightGroup.isOn = false; // 플레이어가 떠나면 꺼짐
            anim.SetBool("isOn", false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairMonsterDetection : MonoBehaviour
{

    public Animator anim; // 애니메이터



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Attack", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Attack", false);

        }
    }
}

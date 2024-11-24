using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepingMonster : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("Awake", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        anim.SetBool("Awake", false);

    }

}

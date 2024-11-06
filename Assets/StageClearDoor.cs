using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearDoor : MonoBehaviour
{
    Animator anim;
    bool isInteracting = false;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(isInteracting && Input.GetKeyDown(KeyCode.Z)) {
            Open();
        }
    }

    void Open()
    {
        anim.SetTrigger("open");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        isInteracting = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        isInteracting = false;
    }
}

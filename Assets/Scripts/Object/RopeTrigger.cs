using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeTrigger : MonoBehaviour
{
    public Collider2D pathcollision;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 재부착 허용 상태일 때만 밧줄에 붙도록 함
        if (other.CompareTag("Player"))
        {
            pathcollision.enabled = true;
        }
    }
}

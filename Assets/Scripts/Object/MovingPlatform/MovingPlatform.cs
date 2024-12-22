using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    protected bool isStepped = false; // 밟힌 상태
    protected Transform originalParent = null; // 밟은 플레이어의 원래 부모

    // 플레이어가 플랫폼을 밟으면 플레이어를 해당 플랫폼의 자식으로 옮긴다.
    protected virtual void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("Player")) {
            isStepped = true;
            originalParent = other.transform.parent;
           other.transform.SetParent(transform);
        } 
    }

    // 플레이어가 플랫폼을 벗어나면 플레이어를 원래 부모의 자식으로 옮긴다.
    protected virtual void OnCollisionExit2D(Collision2D other) {
        if(other.collider.CompareTag("Player")) {
            isStepped = false;
            other.transform.SetParent(originalParent);
            originalParent = null;
        }
    }
}

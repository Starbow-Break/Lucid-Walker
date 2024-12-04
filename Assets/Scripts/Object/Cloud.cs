using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] float disappearTime = 2.0f; // 사라지는 시간
    Collider2D cloudCollider; // 콜리전
    Animator anim; // 애니매이터
    

    private void Awake() {
        cloudCollider = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    // 밟으면 구름이 사라질 징조를 보여준다.
    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("Player")) {
            anim.SetTrigger("step");
            StartCoroutine(DisappearFlow());
        }
    }

    IEnumerator DisappearFlow() {
        yield return new WaitForSeconds(disappearTime);
        cloudCollider.enabled = false;
        anim.SetTrigger("disappear");
    }
}

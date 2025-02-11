using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSwitch : MonoBehaviour
{
    private Animator anim;
    private bool isPlayerInRange = false; // 플레이어가 트리거 안에 있는지 확인
    public RopeCreate ropeCreate; // RopeCreate 스크립트 참조

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // 플레이어가 트리거 범위 내에 있을 때만 Z 키 입력 가능
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            bool currentState = anim.GetBool("On");
            anim.SetBool("On", !currentState); // 현재 상태 반전

            if (!currentState) // On 상태로 변경 (로프 생성)
            {
                ropeCreate.CreateRope();
            }
            else // Off 상태로 변경 (로프 삭제)
            {
                ropeCreate.ClearRope();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어가 범위 안에 들어오면
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어가 범위를 나가면
        {
            isPlayerInRange = false;
        }
    }
}

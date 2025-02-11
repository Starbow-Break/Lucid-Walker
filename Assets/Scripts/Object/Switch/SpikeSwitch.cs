using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSwitch : MonoBehaviour
{
    private Animator anim;
    private bool isPlayerInRange = false; // 플레이어가 트리거 안에 있는지 확인
    public GameObject spike;            // 움직일 스파이크 오브젝트

    // 스파이크가 이동할 오프셋과 이동에 걸리는 시간
    public float moveOffset = 0.5f;
    public float moveDuration = 0.2f;

    private Vector3 originalSpikePosition; // 스파이크의 원래 위치
    private Vector3 targetSpikePosition;   // 스파이크가 올라갔을 때의 위치

    private void Start()
    {
        anim = GetComponent<Animator>();

        if (spike != null)
        {
            originalSpikePosition = spike.transform.position;
            targetSpikePosition = originalSpikePosition + new Vector3(0, moveOffset, 0);
        }
    }

    private void Update()
    {
        // 플레이어가 트리거 범위 내에 있을 때만 Z 키 입력 처리
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            // 현재 스파이크 애니메이터의 "On" 상태를 반전
            bool currentState = anim.GetBool("On");
            bool newState = !currentState;
            anim.SetBool("On", newState);

            // 상태에 따라 스파이크 위치를 이동시킴
            if (newState)
            {
                // 스파이크가 켜지면 위로 이동
                StartCoroutine(MoveSpike(spike.transform.position, targetSpikePosition, moveDuration));
            }
            else
            {
                // 스파이크가 꺼지면 원래 위치로 이동
                StartCoroutine(MoveSpike(spike.transform.position, originalSpikePosition, moveDuration));
            }
        }
    }

    // Lerp를 이용해 스파이크 위치를 부드럽게 이동시키는 코루틴
    private IEnumerator MoveSpike(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            spike.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spike.transform.position = endPos;
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
        if (collision.CompareTag("Player")) // 플레이어가 범위를 벗어나면
        {
            isPlayerInRange = false;
        }
    }
}

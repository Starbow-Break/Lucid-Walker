using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBoundSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera ActiveCamera;
    public PolygonCollider2D NewBoundingShape;
    private Vector2 triggerPosition; // 트리거의 위치

    private bool isSwitching = false; // 중복 트리거 방지 플래그
    private float switchCooldown = 0.2f; // 쿨다운 시간

    private void Start()
    {
        // 현재 트리거 오브젝트의 위치 저장
        triggerPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (other.CompareTag("Player"))
        {
            // 플레이어의 X축 위치와 트리거 위치를 비교하여 방향 판별
            bool isPlayerMovingRight = player.transform.position.x > triggerPosition.x;

            // 플레이어가 트리거를 올바른 방향으로 통과하는지 확인
            if ((!isPlayerMovingRight && player.IsFacingRight) || (isPlayerMovingRight && !player.IsFacingRight))
            {
                SwitchCameraBound();
                StartCoroutine(TriggerCooldown()); // 중복 방지용 쿨다운 시작
            }
        }
    }

    private IEnumerator TriggerCooldown()
    {
        isSwitching = true; // 중복 방지 활성화
        yield return new WaitForSeconds(switchCooldown); // 쿨다운 시간 대기
        isSwitching = false; // 중복 방지 해제
    }
    public void SwitchCameraBound()
    {
        CinemachineConfiner2D _cinemachineConfinder = ActiveCamera.GetComponent<CinemachineConfiner2D>();

        _cinemachineConfinder.m_BoundingShape2D = NewBoundingShape;

        _cinemachineConfinder.InvalidateCache();

    }
}

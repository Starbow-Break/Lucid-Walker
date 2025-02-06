using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class SlidingTrigger : MonoBehaviour
{
    public CameraNewTilemap cameraNewTilemap; // CameraNewTilemap 스크립트 참조
    public Transform teleportDestination; // 플레이어 텔레포트 목적지
    public float slideSpeed = 10f; // 슬라이딩 속도

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }
        player.SetSlidingState(true);
        if (player != null)
        {
            // 슬라이드 시작: Z 회전을 25도로 설정
            player.transform.rotation = Quaternion.Euler(0, 0, 25);

            // 슬라이드 속도 적용
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 플레이어의 현재 방향에 따라 슬라이드 속도를 적용
                Vector2 slideDirection = new Vector2(player.transform.localScale.x, 0).normalized; // x 방향으로 슬라이딩
                rb.velocity = slideDirection * slideSpeed;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            return;
        }
        player.SetSlidingState(false);
        if (player != null)
        {
            // 슬라이드 종료: Z 회전 복구
            player.transform.rotation = Quaternion.Euler(0, 0, 0);

            // 카메라 타일맵 전환 호출
            if (cameraNewTilemap != null)
            {
                cameraNewTilemap.MoveCameraToNewTilemap(true, false, player.transform, teleportDestination); // 새 타일맵으로 카메라 이동
            }
            else
            {
                Debug.LogWarning("CameraNewTilemap reference is missing!");
            }
        }
    }
}

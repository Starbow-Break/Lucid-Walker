using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

using UnityEngine;

public class SlidingTrigger : MonoBehaviour
{
    public CameraNewTilemap cameraNewTilemap; // CameraNewTilemap 스크립트 참조
    public Transform teleportDestination; // 플레이어 텔레포트 목적지

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            // 슬라이드 시작: Z 회전을 30도로 설정
            player.transform.rotation = Quaternion.Euler(0, 0, 25);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
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

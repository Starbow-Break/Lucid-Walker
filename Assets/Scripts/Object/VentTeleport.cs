using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentTeleport : MonoBehaviour
{
    [SerializeField]
    private Transform teleportDestination; // 텔레포트 목적지
    [SerializeField]
    private Animator ventAnimator; // vent 애니메이터 참조
    [SerializeField]
    private CameraNewTilemap cameraNewTilemap; // CameraNewTilemap 스크립트 참조
    [SerializeField]
    private Animator destinationAnimator; // 목적지 애니메이터


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어의 SpriteRenderer를 가져오기
            SpriteRenderer playerSprite = other.GetComponent<SpriteRenderer>();

            if (playerSprite != null)
            {
                playerSprite.sortingOrder = 10; // Sorting Layer를 2로 설정
            }
            // vent 애니메이션 실행
            if (ventAnimator != null)
            {
                ventAnimator.SetTrigger("Open"); // vent 오픈 애니메이션 트리거
            }

            StartCoroutine(TeleportSet(other.transform)); // 플레이어 텔레포트
        }
    }

    private IEnumerator TeleportSet(Transform player)
    {
        // 텔레포트 실행
        if (teleportDestination != null)
        {
            // 도착 시 destination 애니메이션 실행
            if (destinationAnimator != null)
            {
                destinationAnimator.SetTrigger("Open"); // 목적지의 Open 애니메이션 트리거
            }

            // 텔레포트 후 카메라를 새 타일맵으로 이동
            if (cameraNewTilemap != null)
            {
                cameraNewTilemap.MoveCameraToNewTilemap(true, false, player, teleportDestination); // 새 타일맵으로 카메라 이동
            }
        }
        else
        {
            Debug.LogWarning("Teleport destination is not set!");
        }

        yield return null; // 코루틴의 반환값 추가
    }
}


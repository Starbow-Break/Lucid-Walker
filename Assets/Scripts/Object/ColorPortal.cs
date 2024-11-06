using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPortal : MonoBehaviour
{
    public Transform targetPosition; // 이동할 타깃 위치
    public ParticleSystem playerParticleSystem; // 플레이어의 파티클 시스템
    public Color portalParticleColor; // 포탈 이동 시 적용할 파티클 색상

    public GameObject tileMapToDeactivate; // 비활성화할 타일맵 (특정 조건일 때만)
    public List<Collider2D> collidersToDisable; // 비활성화할 Collider2D 리스트 (특정 조건일 때만)
    public float targetCameraSize = 7.5f; // 목표 카메라 사이즈
    public float cameraLerpSpeed = 2f; // 카메라 크기 변경 속도
    private bool isPlayerInPortal = false; // 플레이어가 포탈에 있는지 여부 확인

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 포탈에 들어왔을 때
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true; // 플레이어가 포탈에 있음을 표시
            StartCoroutine(WaitForKeyPress(other)); // 키 입력을 기다리는 코루틴 시작
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 포탈에서 나갔을 때
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false; // 플레이어가 포탈을 벗어났음을 표시
            StopCoroutine(WaitForKeyPress(other)); // 코루틴 중지
        }
    }

    private IEnumerator WaitForKeyPress(Collider2D player)
    {
        while (isPlayerInPortal)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                TeleportPlayer(player); // 키 입력을 받으면 텔레포트 실행
                yield break; // 코루틴 종료
            }
            yield return null; // 다음 프레임까지 대기
        }
    }

    private void TeleportPlayer(Collider2D player)
    {
        // 플레이어가 타깃 위치로 이동
        if (targetPosition != null)
        {
            player.transform.position = targetPosition.position;
        }

        // 파티클 색상 변경
        if (playerParticleSystem != null)
        {
            var mainModule = playerParticleSystem.main;
            mainModule.startColor = portalParticleColor;
        }

        // 특정 조건 (타깃 위치의 태그가 "PortalEnd"인 경우)에서 비활성화할 항목 처리
        if (targetPosition != null && targetPosition.CompareTag("PortalEnd"))
        {
            // 타일맵 비활성화
            if (tileMapToDeactivate != null)
            {
                tileMapToDeactivate.SetActive(false);
            }

            // 콜라이더 비활성화
            foreach (Collider2D collider in collidersToDisable)
            {
                collider.enabled = false;
            }

            SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerSprite.sortingOrder = 10;
            }

            // 카메라 크기를 Lerp로 자연스럽게 변경
            StartCoroutine(LerpCameraSize(targetCameraSize));
        }
    }

    private IEnumerator LerpCameraSize(float targetSize)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float initialSize = mainCamera.orthographicSize;
            float elapsedTime = 0f;

            while (Mathf.Abs(mainCamera.orthographicSize - targetSize) > 0.01f)
            {
                elapsedTime += Time.deltaTime * cameraLerpSpeed;
                mainCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, elapsedTime);
                yield return null;
            }

            mainCamera.orthographicSize = targetSize; // 목표 크기에 정확히 도달하도록 설정
        }
    }
}

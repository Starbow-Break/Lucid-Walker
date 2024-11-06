using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoviePortal : MonoBehaviour
{
    public Transform SeatBehindMap; // 새로운 맵의 플레이어 시작 위치
    public GameObject tileMapToActivate; // 비활성화된 타일맵 (활성화할 타일맵을 여기에 할당)
    public List<Collider2D> collidersToEnable; // 포탈 이동 시 활성화할 Collider2D 리스트
    public Color portalParticleColor = Color.red; // 포탈 이동 시 적용할 파티클 색상
    public ParticleSystem playerParticleSystem; // 플레이어의 Particle System 참조
    public float cameraTargetSize = 16f; // 카메라의 목표 크기
    public float cameraLerpSpeed = 2f; // 카메라 크기 변경 속도
    private bool isPlayerInPortal = false; // 플레이어가 포탈에 있는지 여부 확인

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 포탈에 들어왔을 때
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInPortal = true; // 플레이어가 포탈에 있음을 표시
            StartCoroutine(WaitForKeyPress(other)); // 키 입력을 기다리는 코루틴 시작
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 포탈에서 나갔을 때
        if (other.gameObject.CompareTag("Player"))
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
        // 플레이어를 새로운 맵의 시작 지점으로 이동
        player.transform.position = SeatBehindMap.position;


        // 카메라 확대
        StartCoroutine(LerpCameraSize(cameraTargetSize));

        // 비활성화된 타일맵 활성화
        if (tileMapToActivate != null)
        {
            tileMapToActivate.SetActive(true);
        }

        // 포탈 이동 시 리스트에 담긴 Collider2D들을 활성화
        foreach (Collider2D collider in collidersToEnable)
        {
            collider.enabled = true;
        }

        // 플레이어의 Order in Layer를 0으로 설정
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite != null)
        {
            playerSprite.sortingOrder = 0;
        }

        // 플레이어의 Particle System 색상을 빨간색으로 변경
        if (playerParticleSystem != null)
        {
            var mainModule = playerParticleSystem.main;
            mainModule.startColor = portalParticleColor; // 색상을 빨간색으로 변경
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

            mainCamera.orthographicSize = targetSize; // 정확히 목표 크기에 도달하도록 설정
        }
    }
}

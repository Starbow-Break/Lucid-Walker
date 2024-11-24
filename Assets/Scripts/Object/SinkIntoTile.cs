using System.Collections;
using UnityEngine;

public class SinkIntoTile : MonoBehaviour
{
    [SerializeField]
    private Transform teleportDestination; // 텔레포트 목적지
    [SerializeField]
    private Animator ventAnimator; // vent 애니메이터 참조
    [SerializeField]
    private CameraNewTilemap cameraNewTilemap; // CameraNewTilemap 스크립트 참조
    [SerializeField]
    private Animator playerAnimator; // Inspector에서 플레이어 애니메이터 참조


    public float sinkDelay = 1.0f; // 얼마나 천천히 빠질지 설정
    public float sinkSpeed = 0.5f; // 플레이어가 천천히 가라앉는 속도
    public float postTeleportFallDuration = 1f; // 텔레포트 후 천천히 떨어지는 시간
    public float postTeleportFallDistance = 1f; // 텔레포트 후 떨어지는 거리

    private bool isSinking = false; // 중복 실행 방지 플래그

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isSinking)
        {
            // 플레이어의 SpriteRenderer를 가져오기
            SpriteRenderer playerSprite = other.GetComponent<SpriteRenderer>();

            if (playerSprite != null)
            {
                playerSprite.sortingOrder = 2; // Sorting Layer를 2로 설정
            }
            else
            {
                Debug.LogWarning("Player SpriteRenderer not found!");
            }

            if (playerAnimator != null)
            {
                playerAnimator.SetBool("Falling", true); // "Falling"을 true로 설정
            }
            else
            {
                Debug.LogWarning("Player Animator reference is not set!");
            }

            StartCoroutine(SinkAndTeleport(other.transform)); // 플레이어 천천히 가라앉고 텔레포트
        }
    }

    private IEnumerator SinkAndTeleport(Transform player)
    {
        isSinking = true; // 중복 실행 방지
        Vector3 originalPosition = player.position;

        // 플레이어가 천천히 아래로 가라앉기
        float elapsedTime = 0f;
        while (elapsedTime < sinkDelay)
        {
            elapsedTime += Time.deltaTime;
            player.position = Vector3.Lerp(originalPosition,
                originalPosition + Vector3.down * 0.5f, // 가라앉는 깊이 조정
                elapsedTime / sinkDelay); // 부드럽게 아래로 이동
            yield return null;
        }

        // 텔레포트 실행
        if (teleportDestination != null)
        {
            // vent 애니메이션 실행
            if (ventAnimator != null)
            {
                ventAnimator.SetTrigger("Open"); // vent 오픈 애니메이션 트리거
            }

            // 텔레포트 후 카메라를 새 타일맵으로 이동
            if (cameraNewTilemap != null && teleportDestination != null)
            {
                cameraNewTilemap.MoveCameraToNewTilemap(true, true, player, teleportDestination);
            }

            // Falling 상태를 false로 설정
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("Falling", false);
            }
        }
        else
        {
            Debug.LogWarning("Teleport destination is not set!");
        }

        // 텔레포트 후 천천히 아래로 떨어지는 효과
        elapsedTime = 0f;
        Vector3 postTeleportStartPosition = player.position;
        Vector3 postTeleportTargetPosition = postTeleportStartPosition + Vector3.down * postTeleportFallDistance;

        while (elapsedTime < postTeleportFallDuration)
        {
            elapsedTime += Time.deltaTime;
            player.position = Vector3.Lerp(postTeleportStartPosition, postTeleportTargetPosition, elapsedTime / postTeleportFallDuration);
            yield return null;
        }

        // 가라앉기 종료
        isSinking = false;
    }
}

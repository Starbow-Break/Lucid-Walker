using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class CameraNewTilemap : MonoBehaviour
{
    public CameraFollow cameraFollow; // 카메라 이동 스크립트 참조
    public Tilemap newTilemap; // 이동할 새 타일맵 참조
    public CircleWipe circleWipe; // CircleWipe 참조

    // 새 타일맵으로 카메라 이동 + 전환 효과
    public void MoveCameraToNewTilemap(bool vertical, bool isEntering, Transform player, Transform teleportDestination)
    {
        if (circleWipe == null)
        {
            circleWipe = FindObjectOfType<CircleWipe>(); // CircleWipe 자동 검색
        }

        if (circleWipe != null)
        {
            // CircleWipe 애니메이션 실행
            StartCoroutine(TransitionAndMove(vertical, isEntering, player, teleportDestination));
        }
        else
        {
            Debug.LogWarning("CircleWipe reference is missing!");
        }
    }

    private IEnumerator TransitionAndMove(bool vertical, bool isEntering, Transform player, Transform teleportDestination)
    {
        // CircleWipe 전환 효과 시작
        circleWipe.PlayTransition(vertical, isEntering);

        // 애니메이션 대기 (1초로 가정, CircleWipe에서 설정한 시간)
        yield return new WaitForSeconds(1f);

        // 카메라를 새 타일맵으로 이동
        if (cameraFollow != null && newTilemap != null)
        {
            cameraFollow.SetTarget(newTilemap); // 카메라 타겟을 새 타일맵으로 설정
        }
        else
        {
            Debug.LogWarning("CameraFollow or newTilemap reference is missing!");
        }

        // 카메라 이동 완료 후 플레이어 위치 이동
        if (teleportDestination != null)
        {
            player.position = teleportDestination.position; // 플레이어를 텔레포트 목적지로 이동
        }
        else
        {
            Debug.LogWarning("Teleport destination is not set!");
        }
    }
}

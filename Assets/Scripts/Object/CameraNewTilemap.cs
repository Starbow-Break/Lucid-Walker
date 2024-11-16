using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraNewTilemap : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public Tilemap newTilemap;

    // 새 타일맵으로 카메라 이동 메서드
    public void MoveCameraToNewTilemap()
    {
        if (cameraFollow != null && newTilemap != null)
        {
            // 카메라 타겟을 새로운 타일맵으로 설정
            cameraFollow.SetTarget(newTilemap);
        }
        else
        {
            Debug.LogWarning("CameraFollow or newTilemap reference is missing!");
        }
    }
}

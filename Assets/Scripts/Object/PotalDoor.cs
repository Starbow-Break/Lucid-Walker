using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotalDoor : MonoBehaviour
{
    public Transform SeatBehindMap; // 새로운 맵의 플레이어 시작 위치
    public GameObject tileMapToActivate; // 비활성화된 타일맵 (활성화할 타일맵을 여기에 할당)

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 문에 부딪혔을 때 새로운 맵으로 이동
        if (other.gameObject.CompareTag("Player"))
        {
            // 플레이어를 새로운 맵의 시작 지점으로 이동
            other.transform.position = SeatBehindMap.position;

            // 비활성화된 타일맵 활성화
            if (tileMapToActivate != null)
            {
                tileMapToActivate.SetActive(true);
            }

            // 플레이어의 Order in Layer를 1로 설정
            SpriteRenderer playerSprite = other.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerSprite.sortingOrder = 0;
            }
        }
    }
}

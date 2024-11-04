using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileColider : MonoBehaviour
{
    public TilemapCollider2D targetTilemapCollider; // 참조할 타일맵 콜라이더

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // 플레이어가 트리거에 닿았을 때만 작동
        {
            if (targetTilemapCollider != null)
            {
                targetTilemapCollider.enabled = true; // 타일맵 콜라이더 활성화
            }
        }
    }
}

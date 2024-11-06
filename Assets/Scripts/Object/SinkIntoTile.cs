using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SinkIntoTile : MonoBehaviour
{
    [SerializeField]
    private TilemapCollider2D tileCollider;  // 타일맵의 Collider 가져오기
    public float sinkDelay = 2.0f;  // 얼마나 천천히 빠질지 설정
    private bool isSinking = false;  // 중복 실행 방지 플래그

    // OnTriggerEnter2D는 Trigger Collider가 있는 GameObject에서 호출됨
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isSinking)
        {
            StartCoroutine(DisableColliderAfterDelay());
        }
    }


    IEnumerator DisableColliderAfterDelay()
    {
        isSinking = true;  // 코루틴 실행 중으로 설정
        yield return new WaitForSeconds(sinkDelay);  // 설정된 시간만큼 대기
        tileCollider.enabled = false;  // 타일맵의 Collider 비활성화
    }
}

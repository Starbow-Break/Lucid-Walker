using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotalDoor : MonoBehaviour
{
    public Transform SeatBehindMap; // 새로운 맵의 플레이어 시작 위치

    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 문에 부딪혔을 때 새로운 맵으로 이동
        if (other.gameObject.CompareTag("Door"))
        {
            // 플레이어를 새로운 맵의 시작 지점으로 이동
            transform.position = SeatBehindMap.position;
        }
    }
}
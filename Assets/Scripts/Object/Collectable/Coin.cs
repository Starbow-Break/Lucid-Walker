using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, ICollectable
{
    // 획득
    public void Collect(GameObject owner)
    {
        /*
        * TODO : 코인 획득 로직 구현해야 함
        */
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // 플레이어에게 닿으면 플레이어는 코인을 얻는다.
        if(other.CompareTag("Player")) {
            Collect(other.gameObject); // 코인 획득
            gameObject.SetActive(false); // 코인 비활성화 (최척화 상태에 따라 Destroy로 수정할 수도 있음)
        }
    }
}

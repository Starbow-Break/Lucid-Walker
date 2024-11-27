using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticket : MonoBehaviour, IFollowCollectable
{
    [SerializeField] float followSpeed = 5.0f; // 따라가는 속도

    public bool isFollow { get; set; } = false;

    Vector2 targetPosition = Vector2.zero; // 목표 위치
    Coroutine followCoroutine = null; // 코루틴
    public static int collectedTicketCount = 0; // 획득한 티켓 개수 (전역적으로 관리)

    // 획득
    public void Collect(GameObject owner)
    {
        collectedTicketCount++;
        Debug.Log($"티켓을 획득했습니다! 현재 티켓 개수: {collectedTicketCount}");

        isFollow = true;
        ItemFollowBag bag = owner.GetComponent<ItemFollowBag>();
        bag.AddItem(this);

        // 티켓 3개를 모았을 때 이벤트 트리거
        if (collectedTicketCount >= 3)
        {
            // TriggerSpecialEvent();
        }
    }

    // 목표 설정
    public void FollowTarget(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
        if(isFollow) {
            followCoroutine ??= StartCoroutine(FollowTargetFlow());
        }
    }

    // 목표 위치를 향해 움직인다.
    IEnumerator FollowTargetFlow()
    {
        while(isFollow) {
            transform.position -= followSpeed * Time.deltaTime * (transform.position - (Vector3)targetPosition);
            yield return null;
        }
        followCoroutine = null;
    }

    // // 3개의 티켓을 모으면 호출되는 특별 이벤트
    // private void TriggerSpecialEvent()
    // {
    //     Debug.Log("티켓 3개를 모았습니다! 특별 이벤트가 발생합니다.");
    //     // 예: 다리가 놓이는 연출 트리거
    //     GameObject.FindObjectOfType<BridgeManager>()?.ActivateBridge();

    //     // 예: 흑화 연출 트리거
    //     GameObject.FindObjectOfType<StageManager>()?.TransitionToDarkStage();
    // }

    private void OnTriggerEnter2D(Collider2D other) {
        // 플레이어에게 닿으면 플레이어는 열쇠을 얻는다.
        if(!isFollow && other.CompareTag("Player")) {
            Collect(other.gameObject); // 코인 획득
        }
    }
}

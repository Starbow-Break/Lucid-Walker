using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticket : MonoBehaviour, IFollowCollectable
{
    [SerializeField] float followSpeed = 5.0f; // 따라가는 속도
    bool isFollow = false; // 특정 위치로 따라가는지 여부
    public bool isCollect = false; // 획득 여부
    Transform targetTransform = null; // 목표 위치

    Coroutine followCoroutine = null; // 코루틴
    public static int collectedTicketCount = 0; // 획득한 티켓 개수 (전역적으로 관리)

    // 획득
    public void Collect(GameObject owner)
    {
        isCollect = true;
        collectedTicketCount++;
        Debug.Log($"티켓을 획득했습니다! 현재 티켓 개수: {collectedTicketCount}");

        ItemFollowBag bag = owner.GetComponent<ItemFollowBag>();
        bag.AddItem(this);

        // 티켓 3개를 모았을 때 이벤트 트리거
        if (collectedTicketCount >= 3)
        {
            // TriggerSpecialEvent();
        }
    }

    // 목표를 향해 움직일 것인지를 설정
    public void SetFollow(bool follow)
    {
        if (follow && !isFollow)
        {
            followCoroutine = StartCoroutine(FollowTarget());
        }
        if (!follow && isFollow)
        {
            StopCoroutine(followCoroutine);
            followCoroutine = null;
        }

        isFollow = follow;
    }

    // 목표 설정
    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }

    // 목표 위치를 향해 움직인다
    IEnumerator FollowTarget()
    {
        while (true)
        {
            if (targetTransform != null)
            {
                transform.position -= followSpeed * Time.deltaTime * (transform.localPosition - targetTransform.position);
            }
            yield return null;
        }
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어에게 닿으면 티켓 획득
        if (!isCollect && other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }
}

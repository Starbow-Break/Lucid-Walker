using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Key : MonoBehaviour, IFollowCollectable
{
    [SerializeField] float followSpeed = 5.0f; // 속도

    public bool isFollow { get; set; } = false;

    Vector2 targetPosition = Vector2.zero; // 목표 위치
    Coroutine followCoroutine = null;

    // 획득
    public void Collect(GameObject owner)
    {
        isFollow = true;
        ItemFollowBag bag = owner.GetComponent<ItemFollowBag>();
        bag.AddItem(this);
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

    private void OnTriggerEnter2D(Collider2D other) {
        // 플레이어에게 닿으면 플레이어는 열쇠을 얻는다.
        if(!isFollow && other.CompareTag("Player")) {
            Collect(other.gameObject); // 코인 획득
        }
    }
}

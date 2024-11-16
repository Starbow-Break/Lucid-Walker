using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Key : MonoBehaviour, IFollowCollectable
{
    [SerializeField] float followSpeed = 5.0f; // 속도

    bool isFollow = false; // 특정 위치를 찾아가는지의 여부
    public bool isCollect = false; // 획득 여부
    Transform targetTransform = null; // 목표 위치

    Coroutine followCoroutine = null; // 목표를 따라갈 때 사용하는 코루틴

    // 획득
    public void Collect(GameObject owner)
    {
        isCollect = true;
        ItemFollowBag bag = owner.GetComponent<ItemFollowBag>();
        bag.AddItem(this);
    }

    // 목표를 향해 움직일것인지를 설정
    public void SetFollow(bool follow)
    {
        if(follow && !isFollow) {
            StartCoroutine(FollowTarget());
        }
        if(!follow && isFollow) {
            StopCoroutine(followCoroutine);
            followCoroutine = null;
        }

        isFollow = follow;
    }

    // 목표 설정W
    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }

    // 목표 위치를 향해 움직인다.
    IEnumerator FollowTarget()
    {
        while(true) {
            if(targetTransform != null) {
                transform.position -= followSpeed * Time.deltaTime * (transform.localPosition - targetTransform.position);
            }
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // 플레이어에게 닿으면 플레이어는 열쇠을 얻는다.
        if(!isCollect && other.CompareTag("Player")) {
            Collect(other.gameObject); // 코인 획득
        }
    }
}

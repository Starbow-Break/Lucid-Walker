using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trolley : MonoBehaviour, IFollowCollectable
{
    [SerializeField] float followSpeed = 5.0f; // 속도
    Zipline zipline = null;
    PlayerController interactingPlayerController = null; // 상호작용중인 플레이어
    public bool isFollow { get; set; } = false;
    Vector2 targetPosition = Vector2.zero; // 목표 위치
    Coroutine followCoroutine = null;

    // Update is called once per frame
    void Update()
    {
        if(zipline && Input.GetKeyDown(KeyCode.Z) && interactingPlayerController) {
            zipline.BoardPlayer(interactingPlayerController);
        }
    }

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

    // 옷걸이에 연결된 짚라인 설정
    public void SetZipline(Zipline zipline) {
        this.zipline = zipline;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(zipline != null) {
                interactingPlayerController = other.GetComponent<PlayerController>();
            }
            else {
                if(!isFollow) {
                    Collect(other.gameObject);
                }
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            if(zipline != null) {
                interactingPlayerController = null;
            }
        }
    }
}

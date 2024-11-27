using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageClearDoor : MonoBehaviour
{
    [SerializeField] Key key; // 열쇠
    [SerializeField] Transform keyHole; // 열쇠 구멍

    Animator anim;
    GameObject interactingPlayer = null;
    bool isOpen = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isOpen && anim.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            isOpen = true;
        }

        if (interactingPlayer != null && Input.GetKeyDown(KeyCode.Z))
        {
            if (isOpen)
            {
                /*
                * TODO : 스테이지 클리어 로직
                */
                Debug.Log("스테이지 클리어!!!!!!!!");
            }
            else
            {
                ItemFollowBag bag = interactingPlayer.GetComponent<ItemFollowBag>();
                if (bag != null && bag.HasItem(key))
                {
                    Open();
                }
            }
        }
    }

    void Open()
    {
        StartCoroutine(DoorOpenFlow());
    }

    IEnumerator DoorOpenFlow()
    {
        // ItemFollowBag에서 해당 열쇠 분리
        ItemFollowBag bag = interactingPlayer.GetComponent<ItemFollowBag>();

        // 맞는 열쇠가 있다면 문을 연다.
        if(bag.HasItem(key)) {
            bag.RemoveItem(key);

            key.isFollow = true;
            key.FollowTarget(keyHole.position);
            while ((keyHole.position - key.transform.position).sqrMagnitude <= 0.0001f)
            {
                yield return null;
            }

            // 대기
            yield return new WaitForSeconds(1f);

            // 문 열기
            Destroy(key.gameObject);
            anim.SetTrigger("open");
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactingPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (interactingPlayer == other.gameObject)
        {
            interactingPlayer = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFollowBag : MonoBehaviour
{
    [SerializeField] Transform targetParent; // 목표 위치들의 부모
    [SerializeField] int initializeSize = 5; // 초기 가방사이즈

    int capacity = 0; // 가방 수용량
    List<IFollowCollectable> collectItems; // 얻은 아이템들 (떠다니는 아이템들이 담길 예정)

    public void Awake()
    {
        collectItems = new List<IFollowCollectable>();

        // 처음에 설정한 갯수만큼 슬롯 생성
        for(int i = 0; i < initializeSize; i++) {
            GameObject obj = new GameObject("Item");
            obj.transform.parent = targetParent;
            capacity++;
            obj.transform.localPosition = Vector2.left * (capacity - 1);
        }
    }

    // 아이템 소지 여부
    public bool HasItem(IFollowCollectable collectable)
    {
        foreach(IFollowCollectable col in collectItems) {
            if(col == collectable) return true;
        }

        return false;
    }
    
    // 아이템 추가
    public void AddItem(IFollowCollectable collectable)
    {
        collectItems.Add(collectable); // 아이템 추가
        collectable.SetFollow(true); // 따라다니도록 설정
        collectable.SetTargetTransform(targetParent.GetChild(collectItems.Count - 1));
    }

    // 아이템 제거
    public void RemoveItem(IFollowCollectable collectable)
    {
        collectItems.Remove(collectable); // 아이템 제거
        collectable.SetFollow(false); // 따라다니지 못하도록 설정
        StartCoroutine(ModifyTargets()); // 타겟 수정
    }

    // 타겟 재조정
    IEnumerator ModifyTargets()
    {
        for(int i = 0; i < collectItems.Count; i++) {
            collectItems[i].SetTargetTransform(targetParent.GetChild(collectItems.Count - 1));
        }
        yield return null;
    }
}

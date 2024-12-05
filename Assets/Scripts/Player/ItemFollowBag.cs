using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemFollowBag : MonoBehaviour
{
    [SerializeField] Transform bagPoint; // 가방 기준점
    [SerializeField] Vector2 interval; // 간격

    List<IFollowCollectable> collectItems; // 얻은 아이템들 (떠다니는 아이템들이 담길 예정)
    // 읽기 전용으로 collectItems를 노출
    public IReadOnlyList<IFollowCollectable> CollectItems => collectItems;


    void Awake()
    {
        collectItems = new List<IFollowCollectable>();
    }

    void Start()
    {
        StartCoroutine(BringItem());
    }

    // 아이템 소지 여부
    public bool HasItem(IFollowCollectable collectable)
        => collectItems.Contains(collectable);

    // 아이템 추가
    public void AddItem(IFollowCollectable collectable)
    {
        collectable.isFollow = true;
        collectItems.Add(collectable);
    }

    // 아이템 제거
    public void RemoveItem(IFollowCollectable collectable)
    {
        collectable.isFollow = false;
        collectItems.Remove(collectable);
    }

    // 아이템 끌고 오기
    IEnumerator BringItem()
    {
        while (true)
        {
            for (int i = 0; i < collectItems.Count; i++)
            {
                Vector2 targetPosition = Vector2.zero;
                float multiplierX = Mathf.Cos(transform.rotation.eulerAngles.y / 180.0f * Mathf.PI);

                if (i > 0)
                {
                    if (collectItems[i - 1] is MonoBehaviour mb)
                    {
                        Vector3 offset = new(multiplierX * interval.x, interval.y, 0.0f);
                        targetPosition = mb.transform.position + offset;
                    }
                }
                else
                {
                    targetPosition = bagPoint.position;
                }
                collectItems[i].FollowTarget(targetPosition);
            }
            yield return null;
        }
    }
}

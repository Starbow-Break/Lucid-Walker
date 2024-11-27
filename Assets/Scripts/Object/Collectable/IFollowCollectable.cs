using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수집 가능한 아이템 중에서 플레이어를 따라다니는 아이템들
public interface IFollowCollectable : ICollectable
{
    public bool isFollow { get; set; } // 따라가는 상태
    public void FollowTarget(Vector2 targetPosition); // 목표를 따라감
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFollowCollectable : ICollectable
{
    
    public void SetFollow(bool follow); // 목표를 향해 움직일것인지를 설정
    public void SetTargetTransform(Transform target); // 목표 설정
}

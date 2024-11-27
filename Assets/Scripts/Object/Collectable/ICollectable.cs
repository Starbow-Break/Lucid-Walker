using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 수집 가능한 아이템
public interface ICollectable {
    public void Collect(GameObject owner); // 수집
}

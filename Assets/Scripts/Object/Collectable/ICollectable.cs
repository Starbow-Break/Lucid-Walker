using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable {
    public void Collect(GameObject owner); // 수집
}
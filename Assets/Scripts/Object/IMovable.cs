using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IMovable
{
    public bool pushable { get; set; }
    public float mass { get; set; }

    // 물체를 밀 때 밀리는 물체의 총 질량
    public void GetAllOfMoveObject(bool isRight, bool checkPushable, ref HashSet<GameObject> output);
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public interface IMovable
{
    public bool pushable { get; set; } // 밀기 가능 여부
    public float mass { get; set; } // 질량

    // 밀리는 물체를 output에 전달
    // 반환값은 물체들의 이동 가능 여부
    public bool GetAllOfMoveObject(bool isRight, bool checkPushable, ref HashSet<GameObject> output);
}

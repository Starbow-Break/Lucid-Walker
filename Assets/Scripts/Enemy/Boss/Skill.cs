using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Analytics;

// 스킬 클래스
public abstract class Skill : MonoBehaviour
{
    // 스킬을 사용하는데 필요한 SP
    [SerializeField, Min(0)] int _sp = 0;
    public int sp {
        get { return _sp; }
    }

    // 스킬 사용
    public void Cast() {
        StartCoroutine(SkillFlow());
    }

    // 스킬 동작 코루틴
    protected abstract IEnumerator SkillFlow();
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬 클래스
public abstract class Skill : MonoBehaviour
{
    // 스킬 사용
    public Coroutine Cast() {
        Coroutine coroutine = StartCoroutine(SkillFlow());
        return coroutine;
    }

    // 스킬 동작 코루틴
    protected abstract IEnumerator SkillFlow();
}
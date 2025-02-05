using System.Collections;
using UnityEngine;

// 스킬 클래스
public abstract class Skill : MonoBehaviour
{
    // 스킬을 사용하는데 필요한 SP
    [SerializeField, Min(0)] int _sp = 0;
    public int sp {
        get { return _sp; }
    }

    Coroutine coroutine;

    // 스킬 사용
    public void Cast() {
        coroutine = StartCoroutine(SkillFlow());
    }

    // 스킬 리셋 (public)
    public void SkillReset() {
        if(coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        DoReset();
    }

    // 스킬 리셋 (protected)
    protected abstract void DoReset();

    // 스킬 동작 코루틴
    protected abstract IEnumerator SkillFlow();
}
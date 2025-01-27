using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Analytics;

// 스킬 클래스
public abstract class Skill : MonoBehaviour
{
    [SerializeField, Min(0.0f)] float cooldown = 0.0f;

    public float coolDownRemain { get; private set; } = 0.0f;

    protected virtual void Update() {
        if(coolDownRemain > 0.0f) {
            coolDownRemain -= Time.deltaTime;
        }
    }

    // 스킬 사용
    public bool Cast() {
        if(coolDownRemain > 0.0f) {
            Debug.Log($"{Mathf.Ceil(coolDownRemain)}초 뒤에 사용할 수 있습니다.");
            return false;
        }

        Play();
        coolDownRemain = cooldown;
        return true;
    }

    // 스킬 재생
    protected abstract void Play();

    // 스킬 동작 코루틴
    protected abstract IEnumerator SkillFlow();
}
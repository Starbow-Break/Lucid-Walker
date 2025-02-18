using System.Collections;
using UnityEngine;

// 스킬 클래스
public abstract class Skill : MonoBehaviour
{
    // 스킬 쿨타임
    [SerializeField, Min(0.0f)] float coolDown = 0.0f;

    float coolDownRemain = 0.0f;
    
    // 스킬 준비 상태
    public bool isReady {
        get { return coolDownRemain <= 0.0f; }
    }

    protected virtual void Update() {
        coolDownRemain -= Time.deltaTime;
    }

    // 스킬 사용
    public void Cast() {
        if (isReady) {
            coolDownRemain = coolDown;
            StartCoroutine(SkillFlow());
        }
    }

    // 스킬 리셋
    public virtual void ResetSkill() {
        StopAllCoroutines();
        coolDownRemain = 0.0f;
    }

    // 실제 스킬 동작 부분
    protected abstract IEnumerator SkillFlow();
}
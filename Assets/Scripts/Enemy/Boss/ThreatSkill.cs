using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatSkill : Skill
{
    [SerializeField, Min(0.0f)] float duration; // 지속 시간
    [SerializeField] float velocity; // 보스의 이동 속도
    [SerializeField] PlatformScrolling platformScrolling; // 플랫폼 스크롤러

    Animator casterAnimator;

    void Start() {
        casterAnimator = GetComponent<Animator>();
    }

    protected override IEnumerator SkillFlow() {
        casterAnimator.SetBool("move", true);
        platformScrolling.scrollSpeed = velocity;

        yield return new WaitForSeconds(duration);

        casterAnimator.SetBool("move", false);
        platformScrolling.scrollSpeed = 0.0f;

        yield return null;
    }
}

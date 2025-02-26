using System.Collections;
using UnityEngine;

public class TongueSkill : Skill
{
    [SerializeField, Min(0)] int count = 3;     // 공격 횟수

    MaskBossPhase3 maskBoss;
    Animator casterAnimator;

    void Start()
    {
        maskBoss = GetComponent<MaskBossPhase3>();
        casterAnimator = GetComponent<Animator>();
    }

    protected override IEnumerator SkillFlow() {
        casterAnimator.SetBool("tongue_ready", true);
        yield return new WaitForSeconds(0.1f);

        for(int i = 1; i <= count + 1; i++) {
            Vector3 targetBodyPosition = i <= count ? new(0.0f, -3f + i, 0.0f) : new(0, 1, 0);

            while(maskBoss.bodyLocalPosition != targetBodyPosition) {
                Vector3 distance = targetBodyPosition - maskBoss.bodyLocalPosition;
                Debug.Log(distance);

                if(distance.magnitude <= 2f * Time.deltaTime) {
                    maskBoss.MoveBody(distance);
                }
                else {
                    maskBoss.MoveBody(2f * Time.deltaTime * distance.normalized);
                }

                yield return null;
            }

            if(i <= count) {
                casterAnimator.SetTrigger("tongue");
                yield return new WaitForSeconds(0.5f);
            }
        }

        casterAnimator.SetBool("tongue_ready", false);
    }
}

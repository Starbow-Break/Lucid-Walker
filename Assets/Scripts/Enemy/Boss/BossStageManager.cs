using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageManager : MonoBehaviour
{
    Coroutine coroutine = null;
    [Header("Player")]
    [SerializeField] PlayerController playerController;

    [Header("Boss")]
    [SerializeField] MaskBoss bossController;
    [SerializeField] Animator bossAnim;

    [Header("etc")]
    [SerializeField] Animator curtainAnim;
    

    void Start() {
        Phase1Start();
    }

    public void Phase1Start() {
        if(coroutine == null) {
            coroutine = StartCoroutine(Phase1StartFlow());
        }
    }

    IEnumerator Phase1StartFlow() {
        // 플레이어 조작 방지
        playerController.enabled = false;

        // 커튼이 열릴때까지 대기
        yield return new WaitUntil(() => curtainAnim.GetCurrentAnimatorStateInfo(0).IsName("Open"));

        // 살짝 대기
        yield return new WaitForSeconds(1.0f);

        // 보스가 인사
        bossAnim.SetTrigger("greet");
        yield return null;
        yield return new WaitWhile(() => bossAnim.GetCurrentAnimatorStateInfo(0).IsName("Greeting"));

        // 잠시 대기 (이 사이에 대사나 추가 연출 추가 예정)
        yield return new WaitForSeconds(3.0f);

        // 보스전 시작
        bossController.BattleStart();
        bossAnim.SetTrigger("battle_start");
        playerController.enabled = true;
    }
}

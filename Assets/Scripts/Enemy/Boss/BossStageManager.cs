using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Coffee.UIEffects;
using Unity.VisualScripting;
using UnityEngine;

public class BossStageManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] PlayerController playerController;

    [Header("Boss")]
    [SerializeField] List<MaskBoss> bossController;
    [SerializeField] List<Animator> bossAnim;

    [Header("Camera")]
    [SerializeField] CinemachineVirtualCamera theaterCamera;

    [Header("Objects")]
    [SerializeField] List<GameObject> phase1Objects;
    [SerializeField] List<GameObject> phase2Objects;
    [SerializeField] GameObject phase1to2TransitionCanvas;
    [SerializeField] UIEffect phase1to2Effect;

    [Header("etc")]
    [SerializeField] Animator curtainAnim;

    MaterialPropertyBlock mpb;
    CameraController cameraController;

    void Start() {
        mpb = new MaterialPropertyBlock();
        cameraController = theaterCamera.GetComponent<CameraController>();

        StartCoroutine(Test());
    }

    IEnumerator Test() {
        yield return Phase1StartFlow();
        yield return new WaitForSeconds(20.0f);
        bossController[0].Die();
        yield return PhaseChangeFrom1to2Flow();
    }

    public void Phase1Start() {
        StartCoroutine(Phase1StartFlow());
    }

    public void PhaseChangeFrom1to2() {
        StartCoroutine(PhaseChangeFrom1to2Flow());
    }

    IEnumerator Phase1StartFlow() {
        // 플레이어 조작 방지
        playerController.enabled = false;

        // 커튼이 열릴때까지 대기
        yield return new WaitUntil(() => curtainAnim.GetCurrentAnimatorStateInfo(0).IsName("Open"));

        // 살짝 대기
        yield return new WaitForSeconds(1.0f);  

        // 카메라 줌 인
        cameraController.CameraZoom(Vector3.up * 6.0f, 5.2f, 2.5f);
        yield return new WaitForSeconds(2.5f);

        // 보스가 인사
        bossAnim[0].SetTrigger("greet");
        yield return null;
        yield return new WaitWhile(() => bossAnim[0].GetCurrentAnimatorStateInfo(0).IsName("Greeting"));

        // 잠시 대기
        yield return new WaitForSeconds(1.0f);

        /* TODO : 대사나 추가 연출 추가 예정 */

        // 카메라 줌 아웃
        cameraController.CameraZoom(Vector3.up * 4.6f, 8.35f, 2.5f);
        yield return new WaitForSeconds(2.5f);

        // 보스전 시작 (페이즈 1)
        bossController[0].BattleStart();
        bossAnim[0].SetTrigger("battle_start");
        playerController.enabled = true;
    }

    IEnumerator PhaseChangeFrom1to2Flow() {
        // 플레이어 조작 방지
        playerController.enabled = false;

        // 페이즈 1 오브젝트들 비활성화
        foreach(GameObject obj in phase1Objects) {
            obj.SetActive(false);
        }

        // 배경 전환 캔버스 활성화
        phase1to2TransitionCanvas.SetActive(true);

        // 카메라 줌 인
        cameraController.CameraZoom(Vector3.up * 6.0f, 5.2f, 2.5f);
        yield return new WaitForSeconds(2.5f);

        /*
        // 보스 애니메이션 재생
        bossAnim.SetTrigger("");
        yield return new WaitUntil(() => curtainAnim.GetCurrentAnimatorStateInfo(0).IsName("Open"));
        */

        // 카메라 줌 아웃
        cameraController.CameraZoom(Vector3.up * 4.6f, 8.35f, 2.5f);
        yield return new WaitForSeconds(2.5f);

        // 배경 전환
        const float transitionTime = 4.0f;
        while(phase1to2Effect.transitionRate < 1.0f) {
            yield return null;
            phase1to2Effect.transitionRate += Time.deltaTime / transitionTime;
        }

        // 배경 전환 캔버스 비활성화
        phase1to2TransitionCanvas.SetActive(false);

        // 페이즈 2 오브젝트들 활성화
        foreach(GameObject obj in phase2Objects) {
            obj.SetActive(true);
        }

        // 보스전 시작 (페이즈 2)
        bossController[1].BattleStart();
        bossAnim[1].SetTrigger("battle_start");
        playerController.enabled = true;
    }
}

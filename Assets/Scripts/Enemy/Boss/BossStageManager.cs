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

    [Header("Objects")]
    [SerializeField] List<GameObject> phase1Objects;
    [SerializeField] List<GameObject> phase2Objects;

    [Header("Effect Canvases")]
    [SerializeField] GameObject phase1To2TransitionCanvas;
    [SerializeField] UIEffect phase1To2Effect;
    [SerializeField] GameObject phase2To3TransitionCanvas;
    [SerializeField] UIEffect phase2To3Effect;

    [Header("Cameras")]
    [SerializeField] List<CinemachineVirtualCamera> cameras;

    [Header("etc")]
    [SerializeField] Animator curtainAnim;
    [SerializeField] Transform phase3SpawnPoints;

    public static BossStageManager instance {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType<BossStageManager>();
            }

            return m_instance;
        }
    }
    private static BossStageManager m_instance;

    MaterialPropertyBlock mpb;

    int phase;

    void Awake()
    {
        if (instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        mpb = new MaterialPropertyBlock();
        phase = 0;

        foreach(CinemachineVirtualCamera camera in cameras) {
            CameraManager.Register(camera);
        }

        StartNextPhase();
    }

    public void StartNextPhase() {
        ++phase;
        switch(phase) {
            case 1:
                StartCoroutine(Phase1Start());
                break;
            case 2:
                StartCoroutine(Phase2Start());
                break;
            case 3:
                StartCoroutine(Phase3Start());
                break;
        }
    }

    // 1페이즈 시작
    IEnumerator Phase1Start() {
        // 플레이어 조작 방지
        playerController.enabled = false;

        // 커튼이 열릴때까지 대기
        yield return new WaitUntil(() => curtainAnim.GetCurrentAnimatorStateInfo(0).IsName("Open"));

        // 살짝 대기
        yield return new WaitForSeconds(1.0f);  

        // 카메라 줌 인
        CameraManager.SwitchCamera("Focusing Boss Cam");
        yield return new WaitForSeconds(2.5f);

        // 보스가 인사
        bossAnim[0].SetTrigger("greet");
        yield return null;
        yield return new WaitWhile(() => bossAnim[0].GetCurrentAnimatorStateInfo(0).IsName("Greeting"));

        // 잠시 대기
        yield return new WaitForSeconds(1.0f);

        /* TODO : 대사나 추가 연출 추가 예정 */

        // 카메라 줌 아웃
        CameraManager.SwitchCamera("Phase1 and 2 Cam");
        yield return new WaitForSeconds(2.5f);

        // 보스전 시작 (페이즈 1)
        bossController[0].BattleStart();
        bossAnim[0].SetTrigger("battle_start");
        playerController.enabled = true;
    }

    // 2페이즈 시작
    IEnumerator Phase2Start() {
        // 플레이어 조작 방지
        playerController.enabled = false;

        // 페이즈 1 오브젝트들 비활성화
        foreach(GameObject obj in phase1Objects) {
            obj.SetActive(false);
        }

        // 배경 전환 캔버스 활성화
        phase1To2TransitionCanvas.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        // 보스 애니메이션 재생
        Coroutine changeBossObjCo = StartCoroutine(ChangeBossObjectInPhase2Start());
        yield return new WaitForSeconds(1.5f);

        // 배경 전환
        const float transitionTime = 4.0f;
        while(phase1To2Effect.transitionRate < 1.0f) {
            yield return null;
            phase1To2Effect.transitionRate += Time.deltaTime / transitionTime;
        }

        // 배경 전환 캔버스 비활성화
        phase1To2TransitionCanvas.SetActive(false);

        // 페이즈 2 오브젝트들 활성화
        foreach(GameObject obj in phase2Objects) {
            obj.SetActive(true);
        }

        // 잠시 대기
        yield return changeBossObjCo;
        yield return new WaitForSeconds(2.0f);

        // 보스전 시작 (페이즈 2)
        bossController[1].BattleStart();
        bossAnim[1].SetTrigger("battle_start");
        playerController.enabled = true;
    }

    // 3페이즈 시작
    IEnumerator Phase3Start() {
        // 플레이어 컨트롤러 비활성화
        playerController.enabled = false;

        bossAnim[1].SetTrigger("phase_finish");
        yield return null;
        float animLength = bossAnim[1].GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength * 0.99f);

        phase2To3TransitionCanvas.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        // 플레이어를 페이즈 3 맵으로 순간이동
        playerController.transform.position = phase3SpawnPoints.position;
        CameraManager.SwitchCamera("Phase3 Ready Cam");

        // 이전 페이즈 보스 비활성화 
        bossController[1].gameObject.SetActive(false);

        // 배경 전환
        const float transitionTime = 4.0f;
        while(phase2To3Effect.transitionRate < 1.0f) {
            yield return null;
            phase2To3Effect.transitionRate += Time.deltaTime / transitionTime;
        }

        // 플레이어 컨트롤러 활성화
        playerController.enabled = true;
    }

    IEnumerator ChangeBossObjectInPhase2Start() {
        bossAnim[0].SetTrigger("phase_finish");
        yield return null;
        yield return new WaitWhile(() => bossAnim[0].GetCurrentAnimatorStateInfo(0).IsName("Next_Phase"));

        bossAnim[0].gameObject.SetActive(false);
        bossAnim[1].gameObject.SetActive(true);
    }
}

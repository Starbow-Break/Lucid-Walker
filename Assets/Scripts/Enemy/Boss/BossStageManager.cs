using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Coffee.UIEffects;
using UnityEngine;

public class BossStageManager : StageManager
{
    [Header("Player")]
    [SerializeField] PlayerController playerController;

    [Header("Boss")]
    [SerializeField] List<MaskBoss> bossController;
    [SerializeField] GameObject phase3Temp;
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

    [Header("SoundClips")]
    [SerializeField] AudioClip applauseClip;

    [Header("etc")]
    [SerializeField] Animator curtainAnim;
    [SerializeField] Transform phase3SpawnPoints;
    [SerializeField] Transform bossBattleFocusPoint;
    [SerializeField] BossShadow bossShadow;
    [SerializeField] List<Collider2D> phase3Colliders;

    protected override void Start() {
        base.Start();
        
        foreach(CinemachineVirtualCamera camera in cameras) {
            CameraManager.Register(camera);
        }

        //Phase1Start();
        //CameraManager.SwitchCamera("Phase1 and 2 Cam");
        CameraManager.SwitchCamera("Before Phase3 Cam");
        
        bossShadow.gameObject.SetActive(false);
    }

    public void Phase1Start() => StartCoroutine(Phase1StartFlow());
    public void Phase2Start() => StartCoroutine(Phase2StartFlow());
    public void Phase3Start() => StartCoroutine(Phase3StartFlow());
    public void TransitionPhase3() => StartCoroutine(TransitionPhase3Flow());
    public void BossDie() => StartCoroutine(BossDieFlow());

    // 1페이즈 시작
    IEnumerator Phase1StartFlow() {
        // 플레이어 조작 방지
        PlayerBlocked();

        // 커튼이 열릴때까지 대기
        yield return new WaitUntil(() => curtainAnim.GetCurrentAnimatorStateInfo(0).IsName("Open"));

        // PlaySound(AudioManager.Applause, applauseClip, 2.5f);
        AudioManager.Applause.PlayOneShot(applauseClip);
        // 살짝 대기
        yield return new WaitForSeconds(2.5f);

        // 보스가 인사
        bossAnim[0].SetTrigger("greet");
        yield return null;
        yield return new WaitWhile(() => bossAnim[0].GetCurrentAnimatorStateInfo(0).IsName("Greeting"));

        // 잠시 대기
        yield return new WaitForSeconds(1.0f);
        AudioManager.Applause.Stop();

        /* TODO : 대사나 추가 연출 추가 예정 */

        // 카메라 줌 아웃
        CameraManager.SwitchCamera("Phase1 and 2 Cam");
        yield return new WaitForSeconds(2.5f);

        // 보스전 시작 (페이즈 1)
        bossController[0].BattleStart();
        bossAnim[0].SetTrigger("battle_start");
        PlayerAwake();
    }

    // 2페이즈 시작
    IEnumerator Phase2StartFlow() {
        // 플레이어 컨트롤 방지
        PlayerBlocked();

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
        
        PlayerAwake();
    }

    // 3페이즈 시작
    IEnumerator TransitionPhase3Flow() {
        // 플레이어 컨트롤 방지
        PlayerBlocked();

        bossAnim[1].SetTrigger("phase_finish");
        yield return null;
        float animLength = bossAnim[1].GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength * 0.99f);

        phase2To3TransitionCanvas.SetActive(true);
        yield return new WaitForSeconds(2.0f);

        // 플레이어를 페이즈 3 맵으로 순간이동
        playerController.transform.position = phase3SpawnPoints.position;
        CameraManager.SwitchCamera("Spike Frame Phase Cam");

        // 이전 페이즈 보스 비활성화 
        bossController[1].gameObject.SetActive(false);

        // 배경 전환
        const float transitionTime = 4.0f;
        while(phase2To3Effect.transitionRate < 1.0f) {
            yield return null;
            phase2To3Effect.transitionRate += Time.deltaTime / transitionTime;
        }

        // 플레이어 컨트롤러 활성화
        PlayerAwake();
    }

    // 3페이즈 시작
    IEnumerator Phase3StartFlow() {
        // 플레이어 컨트롤 방지
        PlayerBlocked();
        
        // 카메라 초점 이동 및 Area Wall 이동
        CameraManager.ActiveCamera.Follow = bossBattleFocusPoint;

        // 3페이즈에서 맵 이동 및 이벤트 콜리전 비활성화
        foreach(Collider2D collider in phase3Colliders) {
            collider.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(1.0f);

        // 뒤 쪽에 보스 그림자 이동
        bossShadow.gameObject.SetActive(true);
        bossShadow.Move(2.5f);
        yield return new WaitForSeconds(3.5f);
        
        bossShadow.gameObject.SetActive(false);

        // TODO : 보스가 위에서 착지 함
        phase3Temp.SetActive(true);

        // TODO : 배틀 시작

        // 플레이어 컨트롤 재개
        PlayerAwake();
    }

    IEnumerator BossDieFlow()
    {
        // 플레이어 컨트롤 방지
        PlayerBlocked();

        bossAnim[2].SetTrigger("die");
        yield return null;
        yield return new WaitForSeconds(bossAnim[2].GetCurrentAnimatorClipInfo(0)[0].clip.length);

        CameraManager.SwitchCamera("After Phase3 Cam");

        // 플레이어 컨트롤 재개
        PlayerAwake();
    }

    IEnumerator ChangeBossObjectInPhase2Start() {
        bossAnim[0].SetTrigger("phase_finish");
        yield return null;
        yield return new WaitWhile(() => bossAnim[0].GetCurrentAnimatorStateInfo(0).IsName("Next_Phase"));

        bossAnim[0].gameObject.SetActive(false);
        bossAnim[1].gameObject.SetActive(true);
    }

    private void PlayerBlocked()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
            playerController.SetToIdleState();
            var rb = playerController.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.Sleep();
        }
    }

    private void PlayerAwake()
    {
        if (playerController != null)
        {
            playerController.enabled = true;
            var rb = playerController.GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }
    } 
}

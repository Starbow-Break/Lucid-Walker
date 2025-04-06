using System.Collections;
using UnityEngine;
using Cinemachine;

public class CutScene : MonoBehaviour
{
    public GameObject player;
    [Header("Camera Settings")]
    public CinemachineVirtualCamera cutsceneCamera;
    public CinemachineVirtualCamera originalCamera;
    [Header("Reset Settings")]
    // 플레이어를 복구할 고정 위치
    public Transform resetPoint;

    [Header("Animation Settings")]
    public Animator bossHandAnimator;
    public Animator walkingMonsterAnimator;
    public bool useAnimatorStateCheck = false;

    [Header("Dialogue Settings")]
    public DialogueData cutsceneDialogue;

    [Header("Pre-Cutscene Settings")]
    public GameObject ballonCanvas;
    public CharacterSwitchManager characterSwitch;
    public GameObject femaleCharacter;

    [Header("Post-Animation Settings")]
    [Tooltip("애니메이션이 끝난 후 활성화할 게임오브젝트")]
    public GameObject objectToActivateAfterAnimation;

    // 새로 추가: CircleWipe 전환 효과 참조
    public CircleWipe circleWipe;
    private bool cutsceneStarted = false;
    private CinemachineBasicMultiChannelPerlin cameraShake;
    private bool hasDialogueEnded = false;
    private Rigidbody2D rb = null;      // Rigidbody2D 변수 선언


    private void Start()
    {
        if (cutsceneCamera != null)
            cameraShake = cutsceneCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        DialogueManager.Instance.OnDialogueStep += HandleDialogueEvents;
        DialogueManager.Instance.OnDialogueFinished += HandleDialogueFinished;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!cutsceneStarted && other.CompareTag("Player"))
        {
            cutsceneStarted = true;
            StartCoroutine(StartCutScene());
        }
    }

    // 컷씬 재시작 및 플레이어 복구 함수 (Coroutine으로 변경)
    public IEnumerator ResetAndStartCutscene()
    {

        if (circleWipe != null)
        {
            yield return StartCoroutine(circleWipe.AnimateTransitionIn());
        }

        if (player != null && resetPoint != null)
        {
            player.transform.position = resetPoint.position;
        }

        ChargingMonster[] monsters = FindObjectsOfType<ChargingMonster>();
        foreach (ChargingMonster monster in monsters)
        {
            monster.ResetMonsterState();
            monster.gameObject.SetActive(false);
        }

        cutsceneStarted = true;
        StartCoroutine(StartCutScene(skipToStep: 3));  // <- skipToStep 추가

        if (circleWipe != null)
        {
            yield return StartCoroutine(circleWipe.AnimateTransitionOut());
        }
    }

    private void HandleDialogueEvents(int step)
    {
        switch (step)
        {
            case 1:
                // 필요시 카메라 흔들기 등 추가
                // StartCameraShake();
                break;
            case 2:
                if (bossHandAnimator != null)
                {
                    bossHandAnimator.SetTrigger("Carry");
                    StartCoroutine(HideFemaleCharacterAfterDelay(0.9f));
                }
                break;
            case 3:
                ExecuteCase3();
                break;
        }
    }


    private void ExecuteCase3()
    {
        // StartCameraShake();
        Debug.Log("캐이스 3");
        if (walkingMonsterAnimator != null)
        {
            walkingMonsterAnimator.gameObject.SetActive(true);
            StartCoroutine(WaitForAnimationAndFinishDialogue(walkingMonsterAnimator));
        }
        // StopCameraShake();
    }


    private IEnumerator StartCutScene(int skipToStep = 0)
    {
        if (ballonCanvas != null)
            ballonCanvas.SetActive(false);
        if (characterSwitch != null)
            characterSwitch.enabled = false;

        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
            controller.SetToIdleState();
            rb = player.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.Sleep();
        }

        cutsceneCamera.Priority = 50;
        originalCamera.Priority = 10;

        yield return new WaitForSeconds(0.5f);

        if (skipToStep > 0)
        {
            // 대사를 건너뛰고 특정 단계부터 바로 실행
            ExecuteCase3();  // case 3 실행
        }
        else
        {
            DialogueManager.Instance.StartDialogue(cutsceneDialogue);
        }
    }


    private IEnumerator HideFemaleCharacterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (femaleCharacter != null)
        {
            femaleCharacter.SetActive(false);
        }
    }

    // 애니메이션이 끝날 때까지 대기한 후, 대화 종료 및 후처리 실행
    private IEnumerator WaitForAnimationAndFinishDialogue(Animator animator)
    {
        if (animator != null)
        {
            // 애니메이션이 끝날 때까지 대기 (normalizedTime이 1이 될 때까지)
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                yield return null;
            }
        }
        animator.gameObject.SetActive(false);
        // 애니메이션 종료 후 특정 게임오브젝트 활성화
        if (objectToActivateAfterAnimation != null)
        {
            objectToActivateAfterAnimation.SetActive(true);
        }

        hasDialogueEnded = true;
        HandleDialogueFinished();
    }

    private void HandleDialogueFinished()
    {
        // 무조건 카메라 복귀
        cutsceneCamera.Priority = 10;
        originalCamera.Priority = 50;
        Debug.Log("대화 종료 후 원래 가상 카메라로 복귀 완료");

        // 몬스터 애니메이션이 아직 끝나지 않았다면 조작 복구하지 않음
        if (IsMonsterAnimating())
        {
            Debug.Log("아직 몬스터 애니메이션 중 - 조작 복구 보류");
            return;
        }

        // 이제 진짜 조작 복구
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = true;
            rb = player.GetComponent<Rigidbody2D>();
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }

        hasDialogueEnded = false;
    }


    private bool IsMonsterAnimating()
    {
        return walkingMonsterAnimator != null &&
               walkingMonsterAnimator.gameObject.activeInHierarchy &&
               walkingMonsterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
    }



    // private void StartCameraShake()
    // {
    //     // 예: 강도 2, 지속시간 2초로 카메라 흔들기
    //     CameraShake.instance.ShakeActiveCamera(2f, 2f);
    // }
    // private void StopCameraShake()
    // {
    //     CameraShake.instance.StopShakeActiveCamera();
    // }
}

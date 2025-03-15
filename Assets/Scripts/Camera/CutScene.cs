using System.Collections;
using UnityEngine;
using Cinemachine;

public class CutScene : MonoBehaviour
{
    [Header("Camera Settings")]
    public CinemachineVirtualCamera cutsceneCamera;
    public CinemachineVirtualCamera originalCamera;

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

    private bool cutsceneStarted = false;
    private CinemachineBasicMultiChannelPerlin cameraShake;
    private bool hasDialogueEnded = false;

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

    private IEnumerator StartCutScene()
    {
        if (ballonCanvas != null)
            ballonCanvas.SetActive(false);
        if (characterSwitch != null)
            characterSwitch.enabled = false;

        cutsceneCamera.Priority = 50;
        originalCamera.Priority = 10;

        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.StartDialogue(cutsceneDialogue);
    }

    private void HandleDialogueEvents(int step)
    {
        switch (step)
        {
            case 1:
                StartCameraShake();
                break;
            case 2:
                if (bossHandAnimator != null)
                {
                    bossHandAnimator.SetTrigger("Carry");
                    StartCoroutine(HideFemaleCharacterAfterDelay(0.9f));
                }
                break;
            case 3:
                StartCameraShake();
                Debug.Log("캐이스 3");
                if (walkingMonsterAnimator != null)
                {
                    walkingMonsterAnimator.gameObject.SetActive(true);
                    StartCoroutine(WaitForAnimationAndFinishDialogue(walkingMonsterAnimator));
                }
                StopCameraShake();
                break;
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
        if (hasDialogueEnded)
        {
            cutsceneCamera.Priority = 10;
            originalCamera.Priority = 50;
            Debug.Log("대화 종료 후 원래 가상 카메라로 복귀 완료");
        }
    }

    private void StartCameraShake()
    {
        if (cameraShake != null)
        {
            cameraShake.m_AmplitudeGain = 2f;
            cameraShake.m_FrequencyGain = 2f;
        }
    }

    private void StopCameraShake()
    {
        if (cameraShake != null)
        {
            cameraShake.m_AmplitudeGain = 0f;
            cameraShake.m_FrequencyGain = 0f;
        }
    }
}

using UnityEngine;
using System.Collections;

public class InfoDialogue : MonoBehaviour
{
    public DialogueData dialogueData;
    public Transform focusPoint; // 대화 상대 (카메라가 포커스할 대상)
    private CameraFollow cameraFollow;

    // public float cameraTargetSize; // 대화 중 카메라 목표 크기
    // public float originalCameraSize = 7.5f; // 대화 종료 후 복구될 카메라 크기
    // public float cameraLerpSpeed = 2f; // 카메라 크기 변경 속도

    private Transform detectedPlayer; // 감지된 플레이어
    private bool dialogueFinished = false; // 대화 종료 여부를 저장
    private bool isDialogueActive = false; // 대화 활성화 여부를 저장
    private bool isInteracting = false;


    // private void Start()
    // {
    //     cameraFollow = Camera.main.GetComponent<CameraFollow>();

    //     // 초기 카메라 크기 저장
    //     if (Camera.main != null)
    //     {
    //         originalCameraSize = Camera.main.orthographicSize;
    //     }
    // }


    private void Update()
    {
        // 대화 시작 상태
        if (isInteracting && !dialogueFinished && isDialogueActive && DialogueManager.Instance.IsDialogueFinished())
        {
            StartCoroutine(HandlePostDialogue());
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInteracting = true;
            detectedPlayer = other.transform; // 감지된 플레이어 저장
            if (isInteracting && !isDialogueActive)
            {
                StartDialogue();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        isInteracting = false;
    }


    private void StartDialogue()
    {
        Debug.Log("대화 시작 실행");

        isDialogueActive = true; // 대화 활성화
        dialogueFinished = false; // 대화 종료 상태 초기화

        // 카메라를 대화 대상에 포커스
        if (cameraFollow != null && focusPoint != null)
        {
            cameraFollow.SetDialogueFocus(focusPoint);
        }

        // 대화 시작
        DialogueManager.Instance.StartDialogue(dialogueData);

        // 카메라 크기 조정
        // StartCoroutine(LerpCameraSize(cameraTargetSize));
    }

    // private IEnumerator LerpCameraSize(float targetSize)
    // {
    //     Camera mainCamera = Camera.main;
    //     if (mainCamera != null)
    //     {
    //         float initialSize = mainCamera.orthographicSize;
    //         float elapsedTime = 0f;

    //         while (elapsedTime < cameraLerpSpeed)
    //         {
    //             elapsedTime += Time.deltaTime;
    //             mainCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, elapsedTime / cameraLerpSpeed);
    //             yield return null;
    //         }

    //         mainCamera.orthographicSize = targetSize; // 정확히 목표 크기에 도달하도록 설정
    //     }
    // }


    private IEnumerator HandlePostDialogue()
    {
        Debug.Log("HandlePost 실행");

        // 카메라를 먼저 플레이어로 포커싱
        if (cameraFollow != null && detectedPlayer != null)
        {
            cameraFollow.SetDialogueFocus(detectedPlayer);
            yield return new WaitForSeconds(0.2f); // 짧은 대기시간을 추가하여 자연스럽게 전환
        }

        // 대화 종료
        DialogueManager.Instance.EndDialogue();

        // 카메라 크기 복구
        // yield return StartCoroutine(LerpCameraSize(originalCameraSize));

        dialogueFinished = true; // 대화 종료 상태 설정
        isDialogueActive = false; // 대화 비활성화
    }
}

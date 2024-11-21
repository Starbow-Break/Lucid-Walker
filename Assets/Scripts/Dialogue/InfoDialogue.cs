using UnityEngine;
using System.Collections;

public class InfoDialogue : MonoBehaviour
{
    public DialogueData dialogueData;
    public Transform focusPoint; // 대화 상대 (카메라가 포커스할 대상)
    private CameraFollow cameraFollow;
    public Transform player; // 플레이어 Transform 참조

    public float cameraTargetSize; // 대화 중 카메라 목표 크기
    public float originalCameraSize = 7.5f; // 대화 종료 후 복구될 카메라 크기
    public float cameraLerpSpeed = 2f; // 카메라 크기 변경 속도

    private void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        // 초기 카메라 크기 저장
        if (Camera.main != null)
        {
            originalCameraSize = Camera.main.orthographicSize;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 카메라를 대화 대상에 포커스
            if (cameraFollow != null && focusPoint != null)
            {
                cameraFollow.SetDialogueFocus(focusPoint);
            }

            // 대화 시작
            DialogueManager.Instance.StartDialogue(dialogueData);

            // 카메라 크기 조정
            StartCoroutine(LerpCameraSize(cameraTargetSize));
        }
    }

    private void Update()
    {
        if (DialogueManager.Instance.IsDialogueFinished())
        {
            // 대화가 끝난 후 실행
            StartCoroutine(HandlePostDialogue());
        }
    }

    private IEnumerator LerpCameraSize(float targetSize)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            float initialSize = mainCamera.orthographicSize;
            float elapsedTime = 0f;

            while (Mathf.Abs(mainCamera.orthographicSize - targetSize) > 0.01f)
            {
                elapsedTime += Time.deltaTime * cameraLerpSpeed;
                mainCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, elapsedTime / cameraLerpSpeed);
                yield return null;
            }

            mainCamera.orthographicSize = targetSize; // 정확히 목표 크기에 도달하도록 설정
        }
    }

    private IEnumerator HandlePostDialogue()
    {
        // 대화 종료
        DialogueManager.Instance.EndDialogue();

        // 플레이어로 카메라 포커스 이동
        if (cameraFollow != null && player != null)
        {
            cameraFollow.SetDialogueFocus(player);
        }

        // 카메라 크기 복구
        yield return StartCoroutine(LerpCameraSize(originalCameraSize));
    }
}

using System.Collections;
using UnityEngine;

public class TicketDialogueTrigger : MonoBehaviour
{
    public DialogueData allTicketsCollectedDialogue; // 티켓 3개를 모았을 때 대화 데이터
    public DialogueData notEnoughTicketsDialogue; // 티켓이 부족할 때 대화 데이터
    public Transform focusPoint; // 대화 대상 (카메라 포커스)
    public GameObject key; // 생성할 키 프리팹
    public GameObject bridgeTilemap; // 활성화할 다리 Tilemap


    private CameraFollow cameraFollow;
    public Transform player; // 플레이어 Transform
    public float cameraTargetSize = 7.5f; // 대화 중 카메라 크기
    public float originalCameraSize = 7.5f; // 대화 후 복구될 카메라 크기
    public float cameraLerpSpeed = 2f; // 카메라 크기 변경 속도

    private bool hasTriggered = false; // 트리거 중복 실행 방지

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
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // 대화 트리거
            if (Ticket.collectedTicketCount >= 3)
            {
                StartCoroutine(HandleDialogue(allTicketsCollectedDialogue, true)); // 티켓 3개를 모았을 때
            }
            else
            {
                StartCoroutine(HandleDialogue(notEnoughTicketsDialogue, false)); // 티켓이 부족할 때
            }
        }
    }

    private IEnumerator HandleDialogue(DialogueData dialogue, bool allTicketsCollected)
    {
        // 카메라를 대화 대상에 포커스
        if (cameraFollow != null && focusPoint != null)
        {
            cameraFollow.SetDialogueFocus(focusPoint);
        }

        // 대화 시작
        DialogueManager.Instance.StartDialogue(dialogue);

        // 카메라 크기 조정
        yield return StartCoroutine(LerpCameraSize(cameraTargetSize));

        // 대화가 끝날 때까지 대기
        while (!DialogueManager.Instance.IsDialogueFinished())
        {
            yield return null;
        }

        // 카메라를 플레이어로 다시 포커스
        if (cameraFollow != null && player != null)
        {
            cameraFollow.SetDialogueFocus(player);
        }

        // 대화 종료 처리
        DialogueManager.Instance.EndDialogue();

        // 카메라 크기 복구
        yield return StartCoroutine(LerpCameraSize(originalCameraSize));


        if (allTicketsCollected)
        {
            // 키 생성
            SpawnKeyAndActivateBridge();
        }

        // 트리거 비활성화
        gameObject.SetActive(false);
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

            mainCamera.orthographicSize = targetSize; // 목표 크기 설정
        }
    }

    private void SpawnKeyAndActivateBridge()
    {
        // 기존 키 활성화
        if (key != null)
        {
            key.SetActive(true);
        }

        // 다리 활성화
        if (bridgeTilemap != null)
        {
            bridgeTilemap.SetActive(true);
        }
    }

}

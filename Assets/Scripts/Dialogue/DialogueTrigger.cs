using UnityEngine;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;
    public Transform focusPoint; // 대화 상대 (카메라가 포커스할 대상)
    private CameraFollow cameraFollow;

    public float cameraTargetSize; // 대화 중 카메라 목표 크기
    public float originalCameraSize = 7.5f; // 대화 종료 후 복구될 카메라 크기
    private bool hasTriggered = false; // 트리거가 한 번만 실행되도록 제어
    public float cameraLerpSpeed = 2f; // 카메라 크기 변경 속도
    public float focusPointMoveSpeed = 2f; // 포커스 포인트 이동 속도
    public float fadeOutSpeed = 2f; // 페이드아웃 속도

    private SpriteRenderer focusPointSprite; // focusPoint의 SpriteRenderer 참조

    // 참조할 pc,rb
    private PlayerController pc = null; // PlayerController 변수 선언
    private Rigidbody2D rb = null;      // Rigidbody2D 변수 선언


    private void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        if (Camera.main != null)
        {
            originalCameraSize = Camera.main.orthographicSize;
        }

        if (focusPoint != null)
        {
            focusPointSprite = focusPoint.GetComponent<SpriteRenderer>();
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return; // 이미 트리거가 활성화되었다면 실행 안 함

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            pc = other.GetComponent<PlayerController>();
            pc.isDialogueActive = true;
            pc.SetToIdleState();
            rb = other.GetComponent<Rigidbody2D>();
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;

            pc.enabled = false;

            // 카메라를 대화 대상에 포커스
            if (cameraFollow != null && focusPoint != null)
            {
                cameraFollow.SetDialogueFocus(focusPoint);
            }

            // 대화 시작
            DialogueManager.Instance.StartDialogue(dialogueData);

            // 카메라 크기 조정
            StartCoroutine(LerpCameraSize(cameraTargetSize, 1f));
        }
    }

    private void Update()
    {
        if (hasTriggered && DialogueManager.Instance.IsDialogueFinished())
        {
            hasTriggered = false;

            // 대화가 끝난 후 실행
            StartCoroutine(HandlePostDialogue());
        }
    }


    private IEnumerator LerpCameraSize(float targetSize, float minDuration)
    {
        Debug.Log("카메라 줌조절");
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
            // 최소 대기 시간 강제 적용
            float startTime = Time.time;
            while (Time.time - startTime < minDuration)
            {
                yield return null;
            }

            mainCamera.orthographicSize = targetSize;
        }
    }


    private IEnumerator HandlePostDialogue()
    {

        Debug.Log("handlepost함수 실행");
        // 플레이어로 카메라 포커스 이동
        if (cameraFollow != null)
        {
            cameraFollow.ClearDialogueFocus();
            Debug.Log("focus이동 실행");

        }

        // focusPoint 이동 및 페이드아웃
        if (focusPoint != null && focusPoint.tag == "Enemy")
        {
            StartCoroutine(MoveAndFadeOutFocusPoint());
            Debug.Log("focus이동 fade 실행");

        }


        Debug.Log("카메라 복구 실행전");
        // 카메라 크기 복구
        yield return LerpCameraSize(originalCameraSize, 1f);
        Debug.Log("카메라 복구 실행후");

        pc.isDialogueActive = false;

        // 움직임 정상화 
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        pc.enabled = true;
        pc = null;
        rb = null;

        // 대화 종료
        DialogueManager.Instance.EndDialogue();
        Debug.Log("대화종료호출");


        // 모든 작업이 완료된 후 GameObject 비활성화
        gameObject.SetActive(false);


    }

    private IEnumerator MoveAndFadeOutFocusPoint()
    {
        Vector3 moveDirection = new Vector3(1, 0, 0); // focusPoint가 왼쪽으로 이동
        float targetAlpha = 0f;

        if (focusPointSprite != null)
        {
            // 방향 전환 (Flip)
            focusPointSprite.flipX = false;

            // 이동 및 페이드아웃
            while (focusPointSprite.color.a > targetAlpha)
            {
                // 이동
                focusPoint.position += moveDirection * focusPointMoveSpeed * Time.deltaTime;

                // 페이드아웃
                Color color = focusPointSprite.color;
                color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeOutSpeed * Time.deltaTime);
                focusPointSprite.color = color;

                yield return null;
            }
        }

        // focusPoint 비활성화
        // focusPoint.gameObject.SetActive(false);
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class StageStartDialogueEvent : MonoBehaviour
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
    public Tilemap tilemap;
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
        DialogueManager.Instance.OnDialogueStep += HandleDialogueEvents;
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

            while (elapsedTime < cameraLerpSpeed)
            {
                elapsedTime += Time.deltaTime;
                mainCamera.orthographicSize = Mathf.Lerp(initialSize, targetSize, elapsedTime / cameraLerpSpeed);

                // 루프 중단 조건 추가 (안전장치)
                if (Mathf.Abs(mainCamera.orthographicSize - targetSize) < 0.01f)
                {
                    mainCamera.orthographicSize = targetSize; // 강제로 목표 크기로 설정
                    break;
                }

                yield return null;
            }

            // 최소 대기 시간 강제 적용
            float startTime = Time.time;
            while (Time.time - startTime < minDuration)
            {
                yield return null;
            }

            mainCamera.orthographicSize = targetSize; // 목표 크기로 최종 설정
        }
    }


    private void HandleDialogueEvents(int step)
    {
        switch (step)
        {
            case 2: // "어딜 가긴요..." 대사 이후
                focusPoint.GetComponent<Animator>().SetTrigger("Change");
                break;

            case 5: // "니 세상으로 돌아가야지" 대사 이후
                focusPoint.GetComponent<Animator>().SetTrigger("Disappear");

                break;
            case 6: // 마지막 대사 이후 4초간 대기
                StartCoroutine(HandleFocusChangeWithDelay());
                break;
        }
    }

    private IEnumerator HandleFocusChangeWithDelay()
    {
        StartCoroutine(LerpCameraSize(cameraTargetSize, 1f));

        // focus point 변경
        Transform newFocusPoint = GameObject.Find("ChangingCurtain").transform;
        if (newFocusPoint != null)
        {
            focusPoint = newFocusPoint;
            cameraFollow.SetDialogueFocus(focusPoint);

            StageChangeEffect stageEffect = focusPoint.GetComponent<StageChangeEffect>();
            StartCoroutine(stageEffect.StartSceneEffect());
        }

        // 대화 일시 정지
        DialogueManager.Instance.PauseDialogue();

        // 4초 동안 대기
        yield return new WaitForSeconds(4f);

        // 대화 다시 진행
        DialogueManager.Instance.ResumeDialogue();
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
        yield return LerpCameraSize(originalCameraSize, 0f);
        // 카메라 크기 복구
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
        tilemap.gameObject.SetActive(false);


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

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.OnDialogueStep -= HandleDialogueEvents;
    }
}

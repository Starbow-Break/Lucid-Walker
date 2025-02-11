using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private FixedJoint2D fixjoint;
    private bool isAttached = false;
    private GameObject currentRopeSegment; // 현재 플레이어가 붙어있는 밧줄 조각

    public float climbSpeed = 3f;     // 밧줄 올라가기 속도 (필요 시 사용)
    public float snapDistance = 0.1f;  // 위치 보정 거리
    public float jumpForce = 5f;       // 분리 후 점프에 적용할 힘
    private float originalGravityScale; // 원래 중력 값 저장

    // 분리 후 바로 재부착되는 문제를 방지하기 위해 추가된 변수와 쿨다운 시간
    private bool canAttach = true;
    public float attachCooldown = 0.5f;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        fixjoint = GetComponent<FixedJoint2D>();
        fixjoint.enabled = false; // 시작 시에는 비활성화
        originalGravityScale = playerRb.gravityScale; // 원래 중력 값 저장
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 재부착 허용 상태일 때만 밧줄에 붙도록 함
        if (canAttach && collision.CompareTag("Rope") && !isAttached)
        {
            AttachToRope(collision.gameObject);
        }
    }

    private void AttachToRope(GameObject ropeSegment)
    {
        if (isAttached) return;

        Rigidbody2D ropeRb = ropeSegment.GetComponent<Rigidbody2D>();
        fixjoint.enabled = true;
        fixjoint.connectedBody = ropeRb;

        currentRopeSegment = ropeSegment;
        isAttached = true;

        // 밧줄에 붙었을 때 중력 제거 및 속도 초기화
        playerRb.gravityScale = 0f;
        playerRb.velocity = Vector2.zero;

        // 부드럽게 밧줄 위치로 이동 (스냅)
        StartCoroutine(SmoothSnapToRope(currentRopeSegment.transform.position));

        // PlayerController의 상태도 업데이트 (밧줄 상태임을 알려줌)
        playerRb.GetComponent<PlayerController>().isOnRope = true;
    }

    private void Update()
    {
        if (isAttached)
        {
            HandleClimbing();

            // 밧줄에 붙어있는 동안 PlayerController에도 밧줄 상태를 전달
            playerRb.GetComponent<PlayerController>().isOnRope = true;

            // 스페이스바(또는 점프키) 입력 시 분리
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DetachFromRope();
            }
        }
    }

    private void HandleClimbing()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveToRopeSegment(-1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveToRopeSegment(1);
        }
    }

    private void MoveToRopeSegment(int direction)
    {
        if (currentRopeSegment == null) return;

        Transform parent = currentRopeSegment.transform.parent;
        int currentIndex = currentRopeSegment.transform.GetSiblingIndex();
        int targetIndex = currentIndex + direction;

        // 만약 타깃 인덱스가 유효하다면 해당 조각으로 이동
        if (targetIndex >= 0 && targetIndex < parent.childCount)
        {
            GameObject targetSegment = parent.GetChild(targetIndex).gameObject;

            // 현재 밧줄 연결 해제 후 새로운 조각으로 재부착
            DetachFromRope();
            AttachToRope(targetSegment);
        }
        else
        {
            // 타깃 인덱스가 범위를 벗어나면 로프의 끝에 도달한 것으로 판단하고 자동으로 분리
            DetachFromRope();
        }
    }

    private void DetachFromRope()
    {
        if (!isAttached) return;

        fixjoint.connectedBody = null;
        fixjoint.enabled = false;
        isAttached = false;
        currentRopeSegment = null;

        // 분리 시 PlayerController에도 밧줄 상태 해제 전달 및 중력 복원
        playerRb.GetComponent<PlayerController>().isOnRope = false;
        playerRb.gravityScale = originalGravityScale;
        StartCoroutine(ApplyJumpAfterDetach());

        // 재부착 방지를 위한 쿨다운 시작
        StartCoroutine(DetachCooldown());
    }

    private IEnumerator ApplyJumpAfterDetach()
    {
        yield return new WaitForSeconds(0.1f);
        // 밧줄 분리 후 점프 힘 적용
        playerRb.GetComponent<PlayerController>().isOnRope = false;
        playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
    }

    private IEnumerator SmoothSnapToRope(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < 0.2f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // 정확한 위치로 보정
    }

    private IEnumerator DetachCooldown()
    {
        canAttach = false;
        yield return new WaitForSeconds(attachCooldown);
        canAttach = true;
    }
}

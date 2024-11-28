using System.Collections;
using UnityEngine;

public class WalkingMonster : MonoBehaviour, IDamageable
{
    public int health = 3;
    public int damage = 2;
    public Transform groundChkFront;
    public Transform groundChkBack;
    public float chkDistance;

    public float detectionRange = 4f;  // 플레이어 탐지 거리
    public float moveSpeed = 2f;       // 이동 속도
    public float patrolDistance = 5f;  // 순찰 범위
    public float platformDetectionDistance = 0.5f; // 플랫폼 감지 거리
    private Rigidbody2D rb;
    private Animator anim;
    private bool isFacingRight = true; // 오른쪽을 보고 있는지 확인
    private float startPosX;           // 순찰을 시작한 위치

    private bool canAttack = true; // 공격 가능 여부 (쿨타임 상태 관리)
    public float attackCooldown = 2f; // 공격 후 쿨타임 시간 (2초)
    private bool isDead = false; // 몬스터가 죽었는지 확인

    public Vector2 attackCenter = new Vector2(0.5f, 0); // 공격 범위 중심
    public Vector2 attackSize = new Vector2(0.3f, 0.6f);  // 공격 범위 크기 (줄임)

    private Transform detectedPlayer; // 감지된 플레이어 참조

    public LayerMask platformLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosX = transform.position.x;
    }

    void Update()
    {
        bool ground_front = Physics2D.Raycast(groundChkFront.position, Vector2.down, chkDistance, platformLayer);
        bool ground_back = Physics2D.Raycast(groundChkBack.position, Vector2.down, chkDistance, platformLayer);

        if (isDead) return; // 죽었으면 로직 실행 중지

        DetectPlayer(); // 플레이어 감지

        // 플랫폼에서 벗어나려는 경우 또는 앞에 장애물이 있을 경우
        if (!ground_front || !ground_back || IsPlatformInFront())
        {
            Flip();
        }

        // 플레이어가 감지 범위 내에 있지 않으면 순찰
        if (detectedPlayer == null)
        {
            // 플레이어가 감지되지 않으면 순찰
            Patrol();
        }
        else
        {
            // 감지 범위 내에 있으면 쫓아감
            ChasePlayer();
        }

    }
    void DetectPlayer()
    {
        // 감지 범위 내에 있는 객체 중 태그가 "Player"인 대상 탐지
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
        if (playerCollider != null)
        {
            detectedPlayer = playerCollider.transform; // 탐지된 플레이어 설정
        }
        else
        {
            detectedPlayer = null; // 탐지되지 않으면 null로 초기화
        }
    }

    void Patrol()
    {
        // 좌우로 계속 걷는 행동
        if (isFacingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            if (transform.position.x >= startPosX + patrolDistance)
            {
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            if (transform.position.x <= startPosX - patrolDistance)
            {
                Flip();
            }
        }
    }

    void ChasePlayer()
    {
        // 발 밑 플랫폼 확인
        bool ground_front = Physics2D.Raycast(groundChkFront.position, Vector2.down, chkDistance, platformLayer);

        // 플레이어를 향해 이동
        Vector2 direction = ((Vector2)detectedPlayer.position - (Vector2)transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // 플랫폼 끝이나 장애물이 감지되면 Flip
        if (!ground_front || IsPlatformInFront())
        {
            Flip();
            return; // Flip한 후 더 이상 추적 로직 실행하지 않음
        }

        // 플레이어의 위치에 따라 방향을 수정
        if ((detectedPlayer.position.x > transform.position.x && !isFacingRight) ||
            (detectedPlayer.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }

        // 공격 범위 내 플레이어가 있는지 확인
        if (IsPlayerInAttackRange() && canAttack)
        {
            anim.SetTrigger("Attack"); // 공격 애니메이션 실행
            StartCoroutine(AttackCooldown());
        }
    }

    bool IsPlayerInAttackRange()
    {
        // 공격 범위 안에 플레이어가 있는지 확인
        Vector2 boxCenter = (Vector2)transform.position + attackCenter * (isFacingRight ? 1 : -1);
        Collider2D playerInRange = Physics2D.OverlapBox(
            boxCenter,
            attackSize,
            0f,
            LayerMask.GetMask("Player")
        );

        return playerInRange != null;
    }

    // 애니메이션 이벤트에서 호출될 함수
    public void PerformAttack()
    {
        Vector2 boxCenter = (Vector2)transform.position + attackCenter * (isFacingRight ? 1 : -1);

        RaycastHit2D hit = Physics2D.BoxCast(
            boxCenter,
            attackSize,
            0f,
            Vector2.zero,
            0f,
            LayerMask.GetMask("Player")
        );

        if (hit.collider != null)
        {
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, transform); // 실제 공격 로직 실행
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // 플랫폼이 앞에 있는지 감지하는 함수
    bool IsPlatformInFront()
    {
        // NPC의 진행 방향에 따라 Raycast를 발사
        Vector2 rayDirection = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, platformDetectionDistance, platformLayer);

        // Raycast가 지정된 플랫폼 레이어에 충돌하면 true 반환
        return hit.collider != null;
    }


    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead) return;

        health -= damage; // 체력 감소
        anim.SetTrigger("Hurt"); // 피격 애니메이션

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("Die"); // 죽는 애니메이션
    }

    void Flip()
    {
        // 캐릭터의 방향을 반전 (x 스케일 반전)
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        // 공격 범위를 시각적으로 확인하기 위한 Raycast 표시
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, isFacingRight ? Vector2.right : Vector2.left * platformDetectionDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundChkFront.position, groundChkFront.position + Vector3.down * chkDistance);
        Gizmos.DrawLine(groundChkBack.position, groundChkBack.position + Vector3.down * chkDistance);

    }

}

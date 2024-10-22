using System.Collections;
using UnityEngine;

public class MonsterNpc : MonoBehaviour
{
    public int health = 2;
    public int damage = 2;
    public Transform player;           // 플레이어의 위치
    public float detectionRange = 5f;  // 플레이어 탐지 거리
    public float moveSpeed = 2f;       // 이동 속도
    public float patrolDistance = 5f;  // 순찰 범위
    public float platformDetectionDistance = 0.5f; // 플랫폼 감지 거리
    private Rigidbody2D rb;
    private Animator anim;
    private bool isFacingRight = true; // 오른쪽을 보고 있는지 확인
    private float startPosX;           // 순찰을 시작한 위치

    private bool canAttack = true; // 공격 가능 여부 (쿨타임 상태 관리)
    public float attackCooldown = 2f; // 공격 후 쿨타임 시간 (2초)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosX = transform.position.x;
    }

    void Update()
    {
        // 플랫폼 앞에 있는지 감지하고 방향 전환
        if (IsPlatformInFront())
        {
            Flip();
        }

        // 플레이어가 감지 범위 내에 있지 않으면 순찰
        if (Vector2.Distance(player.position, transform.position) > detectionRange)
        {
            Patrol();
        }
        else
        {
            // 감지 범위 내에 있으면 쫓아감
            ChasePlayer();
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
        // 플레이어를 쫓는 행동
        Vector2 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // 플레이어의 방향에 따라 NPC 방향 전환
        if ((player.position.x > transform.position.x && !isFacingRight) ||
            (player.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }
    }

    // 플랫폼이 앞에 있는지 감지하는 함수
    bool IsPlatformInFront()
    {
        // NPC의 진행 방향에 따라 Raycast를 발사
        Vector2 rayDirection = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, platformDetectionDistance);

        // 레이캐스트가 플랫폼에 충돌하면 true 반환
        if (hit.collider != null && hit.collider.CompareTag("Platform"))
        {
            return true;
        }

        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌한 객체가 플레이어일 경우
        if (collision.gameObject.CompareTag("Player"))
        {
            // 공격할 수 있을 때 공격
            if (canAttack)
            {
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    Attack(damageable);
                }
            }
        }
    }

    void Attack(IDamageable damageable)
    {
        // 플레이어에게 데미지를 입힘
        damageable.TakeDamage(damage, transform);

        // 공격 쿨타임 적용
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown); // 쿨타임 대기
        canAttack = true;
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
        // 감지 범위를 시각적으로 확인하기 위한 Raycast 표시
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, isFacingRight ? Vector2.right : Vector2.left * platformDetectionDistance);
    }
}

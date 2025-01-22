using System.Collections;
using UnityEngine;

public class WalkingMonster : MonoBehaviour, IDamageable
{
    public int health = 3;
    public int damage = 2;

    [Header("Ground Check")]
    [SerializeField] private Transform groundChkFront;
    [SerializeField] private Transform groundChkBack;
    [SerializeField] private float chkDistance;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask WaterLayer;

    [Header("Movement and Detection")]
    [SerializeField] private float detectionRange = 2f;  // 플레이어 탐지 거리
    [SerializeField] private float moveSpeed = 2f;       // 이동 속도
    [SerializeField] private float patrolDistance = 5f;  // 순찰 범위
    [SerializeField] private float platformDetectionDistance = 0.5f; // 플랫폼 감지 거리

    [Header("Attack")]
    [SerializeField] private Vector2 attackCenter = new Vector2(0.5f, 0); // 공격 범위 중심
    [SerializeField] private Vector2 attackSize = new Vector2(0.3f, 0.6f);  // 공격 범위 크기
    [SerializeField] private float attackCooldown = 2f; // 공격 쿨타임 시간

    private Rigidbody2D rb;
    private Animator anim;
    public bool isFacingRight { get; private set; }= true; // 오른쪽을 보고 있는지 확인
    private float startPosX;           // 순찰을 시작한 위치
    private Transform detectedPlayer;  // 감지된 플레이어 참조
    private bool canAttack = true;     // 공격 가능 여부
    private bool isDead = false;       // 몬스터가 죽었는지 확인

    // Debugging Variables
    [Header("Debugging States")]
    [SerializeField] private bool isGroundFront;  // 앞쪽 Ground 감지 상태
    [SerializeField] private bool isGroundBack;   // 뒤쪽 Ground 감지 상태
    [SerializeField] private bool isInWater;      // 물에 있는지 상태
    [SerializeField] private bool isPlatformInFront; // 앞쪽 플랫폼 상태

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosX = transform.position.x;
    }

    void Update()
    {
        // Check ground and platform states
        isGroundFront = Physics2D.Raycast(groundChkFront.position, Vector2.down, chkDistance, platformLayer);
        isGroundBack = Physics2D.Raycast(groundChkBack.position, Vector2.down, chkDistance, platformLayer);
        isInWater = Physics2D.Raycast(groundChkFront.position, Vector2.down, chkDistance, WaterLayer);
        isPlatformInFront = IsPlatformInFront();

        if (isDead) return; // 죽었으면 로직 실행 중지

        DetectPlayer(); // 플레이어 감지

        if (!isInWater)
        {
            // 플랫폼 감지에 따라 방향 전환
            if (!isGroundFront || !isGroundBack || isPlatformInFront)
            {
                Flip();
            }
        }
        else
        {
            if (isPlatformInFront)
            {
                Flip();
            }
        }

        // 플레이어가 감지되지 않으면 순찰
        if (detectedPlayer == null)
        {
            Patrol();
        }
        else
        {
            ChasePlayer();
        }
    }

    void DetectPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
        if (playerCollider != null)
        {
            detectedPlayer = playerCollider.transform;
        }
        else
        {
            detectedPlayer = null;
        }
    }

    void Patrol()
    {
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
        bool ground_front = Physics2D.Raycast(groundChkFront.position, Vector2.down, chkDistance, platformLayer);

        Vector2 direction = ((Vector2)detectedPlayer.position - (Vector2)transform.position).normalized;

        if (!ground_front || isPlatformInFront)
        {
            Flip();
            return; // Flip한 후 더 이상 이동하지 않음
        }

        transform.Translate(direction * moveSpeed * Time.deltaTime);



        if ((detectedPlayer.position.x > transform.position.x && !isFacingRight) ||
            (detectedPlayer.position.x < transform.position.x && isFacingRight))
        {
            Flip();
        }

        if (IsPlayerInAttackRange() && canAttack)
        {
            anim.SetTrigger("Attack");
            StartCoroutine(AttackCooldown());
        }
    }

    bool IsPlayerInAttackRange()
    {
        Vector2 boxCenter = (Vector2)transform.position + attackCenter * (isFacingRight ? 1 : -1);
        Collider2D playerInRange = Physics2D.OverlapBox(
            boxCenter,
            attackSize,
            0f,
            LayerMask.GetMask("Player")
        );

        return playerInRange != null;
    }

    public virtual void PerformAttack()
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
                damageable.TakeDamage(damage, transform);
            }
        }
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    bool IsPlatformInFront()
    {
        Vector2 rayDirection = isFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, platformDetectionDistance, platformLayer);
        return hit.collider != null;
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead) return;

        health -= damage;
        anim.SetTrigger("Hurt");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetTrigger("Die");
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + attackCenter * (isFacingRight ? 1 : -1), attackSize);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundChkFront.position, groundChkFront.position + Vector3.down * chkDistance);
        Gizmos.DrawLine(groundChkBack.position, groundChkBack.position + Vector3.down * chkDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, isFacingRight ? Vector2.right * platformDetectionDistance : Vector2.left * platformDetectionDistance);
    }
}

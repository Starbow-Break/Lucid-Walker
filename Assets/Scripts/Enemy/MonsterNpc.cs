using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MonsterNpc : MonoBehaviour
{
    public Transform player;           // 플레이어의 위치
    public float detectionRange = 5f;  // 플레이어 탐지 거리
    public float moveSpeed = 2f;       // 이동 속도
    public float patrolDistance = 5f;  // 순찰 범위
    private Rigidbody2D rb;
    private Animator anim;
    private bool isFacingRight = true; // 오른쪽을 보고 있는지 확인
    private float startPosX;           // 순찰을 시작한 위치
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startPosX = transform.position.x;
    }
    void Update()
    {
        // 기본적으로 계속 순찰
        Patrol();
        // 플레이어 탐지
        DetectPlayer();
    }
    void Patrol()
    {
        // 좌우로 계속 걷는 행동
        if (isFacingRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            if (transform.position.x >= startPosX + patrolDistance)
            {
                Flip();
            }
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            if (transform.position.x <= startPosX - patrolDistance)
            {
                Flip();
            }
        }
    }
    void DetectPlayer()
    {
        // Raycast로 플레이어가 감지 범위 내에 있는지 확인
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * (isFacingRight ? 1 : -1), detectionRange);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // 플레이어가 감지되면 그 방향으로 이동
            Vector2 targetPosition = new Vector2(player.position.x, rb.position.y);
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);
            rb.MovePosition(newPosition);
            // 플레이어의 방향에 따라 몬스터 방향 전환
            if ((player.position.x > transform.position.x && !isFacingRight) ||
                (player.position.x < transform.position.x && isFacingRight))
            {
                Flip();
            }
        }
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
        // 감지 범위를 시각적으로 확인하기 위한 Raycast 표시
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.right * (isFacingRight ? 1 : -1) * detectionRange);
    }
}
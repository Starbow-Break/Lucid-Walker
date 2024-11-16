using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class MaskMonster : MonoBehaviour, IDamageable
{   
    [SerializeField] bool rightStart = true;

    [Header("Animation")]
    [SerializeField] Animator anim;
    [SerializeField] Floating floating;

    [Header("Stats")]
    [SerializeField, Min(0.0f)] float moveDistance = 10.0f; // 이동 거리
    [SerializeField, Min(0.0f)] float moveSpeed = 1.0f; // 이동 속도
    [SerializeField, Min(0.0f)] float waitTime = 2.0f; // 정지 시간
    [SerializeField, Min(0.0f)] float hp = 10.0f; // 체력
    [SerializeField, Min(0.0f)] float attackCoolTime = 1.0f; // 공격 쿨타임
    [SerializeField, Min(0.0f)] float attackDist = 1.0f; // 공격 범위

    [Header("Particle")]
    [SerializeField] ParticleSystem shadowParticle;
    ParticleSystem.VelocityOverLifetimeModule volm;

    Vector3 routeLeft, routeRight; // 경로 제일 왼쪽, 오른쪽
    public int isRight { get; private set; } // 오른쪽을 바라보면 1, 아니면 -1
    bool isMove = false; // 움직임 여부
    bool isTurn = false; // 방향전환 여부
    bool isDie = false; // 사망 여부
    bool isAttack = false; // 공격 여부
    float currentWaitTime = 0.0f; // 지금 페이즈에 대기한 시간
    float remainAttackCoolTime = 0.0f; // 남은 공격 쿨타임

    // 이동 종료 판정
    bool moveFinished => isRight == 1 
            ? transform.position.x >= routeRight.x
            : transform.position.x <= routeLeft.x;

    // 공격 판정
    bool checkAttack => Physics2D.Raycast(
            transform.position, 
            isRight * Vector3.right,
            attackDist, 
            LayerMask.GetMask("Player")).collider != null;

    void OnValidate() {
        if(moveDistance >= 0.0f) {
            if(rightStart) {
                routeLeft = transform.position;
                routeRight = transform.position + Vector3.right * moveDistance;
            }
            else {
                routeLeft = transform.position + Vector3.left * moveDistance;
                routeRight = transform.position;
            }
        }
    }

    void Awake()
    {
        isRight = rightStart ? 1 : -1;
        transform.localScale = new(isRight, transform.localScale.y, transform.localScale.z);
        volm = shadowParticle.velocityOverLifetime;
        volm.xMultiplier *= isRight;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAttack && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            isAttack = false;
        }

        if (remainAttackCoolTime <= 0.0f)
        {
            if(checkAttack) {
                remainAttackCoolTime = attackCoolTime;
                isAttack = false;
                anim.SetTrigger("attack");
            }
        }
        else {
            remainAttackCoolTime -= Time.deltaTime;
        }

        if (!isDie && !isAttack)
        {
            if (isMove) // 이동 중인 상태라면
            {
                Move(); // 이동
                if(moveFinished) { // 이동이 종료 됐다면 잠시 정지
                    transform.position = isRight == 1 ? routeRight : routeLeft;
                    isMove = false;
                }
            }
            else if (isTurn) { // 방향 전환 상태라면
                if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Turn")) {
                    Flip();
                    shadowParticle.Play();
                    isTurn = false;
                    isMove = true;
                }
            }
            else // 정지중인 상태라면
            {
                currentWaitTime += Time.deltaTime;
                if(currentWaitTime >= waitTime) { // 정지 시간이 끝나면 방향 전환
                    currentWaitTime = 0.0f;

                    if(moveFinished) { // 이동을 끝까지 해서 멈춘거라면 방향 전환
                        isTurn = true;
                        anim.SetTrigger("turn");
                        volm.xMultiplier *= -1;
                        shadowParticle.Stop();
                    }
                    else { // 아니라면 바로 이동
                        isMove = true;
                    }
                }
            }
        }
    }

    // 이동
    void Move()
    {
        transform.position += isRight * moveSpeed * Time.deltaTime * Vector3.right;
    }

    // 사망
    void Die()
    {
        isDie = true;
        anim.SetBool("die", true);
    }

    // 방향 전환
    void Flip()
    {
        isRight *= -1;
        transform.localScale = new(isRight, transform.localScale.y, transform.localScale.z);
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        hp -= 1.0f; // 체력 감소
        if(hp <= 0.0f) { // 체력이 전부 깎이면 사망
            Die();
        }
    }

    private void OnDrawGizmos() {
        // 이동 범위 확인
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(routeLeft, routeRight);

        // 플레이어 감지 범위 확인
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, attackDist * isRight * Vector2.right);
    }
}

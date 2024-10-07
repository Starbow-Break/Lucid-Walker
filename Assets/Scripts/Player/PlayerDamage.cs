using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int health = 100;  // 플레이어의 초기 체력
    private Animator anim;     // Animator 참조
    private Rigidbody2D rb;    // Rigidbody2D 참조
    private bool isInvulnerable = false;  // 무적 상태 확인 변수
    public float invulnerabilityDuration = 1.5f;  // 무적 지속 시간
    public float knockbackForce = 5f;     // 튕기는 힘

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        // 무적 상태라면 데미지를 받지 않음
        if (isInvulnerable) return;

        health -= damage;
        Debug.Log("플레이어가 " + damage + " 데미지를 입었습니다. 현재 체력: " + health);

        // Hurt 애니메이션 트리거 활성화
        anim.SetTrigger("hurt");

        // 데미지를 받았을 때 방향에 따라 반대 방향으로 튕기게 처리
        Knockback(attacker);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // 무적 상태로 설정하고 일정 시간 후에 해제
            StartCoroutine(ActivateInvulnerability());
        }
    }

    void Knockback(Transform attacker)
    {
        // 플레이어가 공격을 받은 방향에 따라 반대 방향으로 힘을 가함
        Vector2 knockbackDirection = (transform.position - attacker.position).normalized;  // 반대 방향으로 벡터 계산
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);  // AddForce로 튕기게 함
    }

    void Die()
    {
        Debug.Log("플레이어가 죽었습니다.");
        // 플레이어가 죽었을 때 처리 (애니메이션, 게임 오버 로직 등 추가 가능).
    }

    private IEnumerator ActivateInvulnerability()
    {
        isInvulnerable = true; // 무적 상태 활성화
        yield return new WaitForSeconds(invulnerabilityDuration); // 무적 지속 시간 대기
        isInvulnerable = false; // 무적 상태 해제
    }


    public void OnHurtAnimationEnd()
    {
        anim.SetBool("isGround", true); // Idle 상태로 돌아가기 위한 조건 설정
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ChairMonster의 공격 콜라이더와 충돌 시 데미지 받기
        if (collision.CompareTag("MonsterAttack"))
        {
            ChairMonster chairMonster = collision.GetComponentInParent<ChairMonster>();
            if (chairMonster != null)
            {
                // 기본 공격 콜라이더와 큰 공격 콜라이더에 따라 데미지 분기
                if (collision == chairMonster.normalAttackBox)
                {
                    TakeDamage(chairMonster.damage, chairMonster.transform);  // 기본 데미지 적용 및 튕김 처리
                }
                else if (collision == chairMonster.bigAttackBox)
                {
                    TakeDamage(chairMonster.bigDamage, chairMonster.transform);  // 큰 데미지 적용 및 튕김 처리
                }
            }
        }
    }
}

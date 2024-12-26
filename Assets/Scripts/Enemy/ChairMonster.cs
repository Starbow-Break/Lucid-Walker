using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairMonster : MonoBehaviour
{
    public Transform pos; // 감지 범위 중심
    public Animator anim; // 애니메이터
    public int damage; // 공격 데미지
    public BoxCollider2D attackBox; // 공격 범위 콜라이더

    public float coolTime; // 공격 쿨타임


    void Start()
    {
        anim = GetComponent<Animator>();
        attackBox.enabled = false; // 초기에는 공격 콜라이더 비활성화
    }


    // 애니메이션 이벤트로 호출: 공격 범위 콜라이더 활성화
    public void EnableAttackCollider()
    {
        attackBox.enabled = true;
    }

    // 애니메이션 이벤트로 호출: 공격 범위 콜라이더 비활성화
    public void DisableAttackCollider()
    {
        attackBox.enabled = false;
    }

    // 공격 범위 내 플레이어 데미지 처리
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, transform);
            }
        }
    }


    void OnDrawGizmos()
    {

        // 공격 범위 Gizmo (파란색)
        Gizmos.color = Color.blue;
        if (attackBox != null)
        {
            Gizmos.DrawWireCube(attackBox.bounds.center, attackBox.bounds.size);
        }
    }
}

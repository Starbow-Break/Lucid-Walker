using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairMonster : MonoBehaviour
{
    public Transform pos;
    public Animator anim;
    public int damage;
    public int bigDamage;
    public BoxCollider2D normalAttackBox;
    public BoxCollider2D bigAttackBox;
    public float coolTime;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Collider2D collider = Physics2D.OverlapBox(pos.position, new Vector2(2f, 2f), 0);


        // 쿨타임이 끝났고 플레이어가 범위에 있을 때만 공격 시작
        if (collider != null && collider.CompareTag("Player"))
        {
            Debug.Log("adf");

            anim.SetBool("Attack", true);  // Attack 애니메이션 시작
            Debug.Log("adf------");
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (normalAttackBox.enabled)
                {
                    damageable.TakeDamage(damage, transform);
                    Debug.Log("작은Collider 호출");
                }
                else if (bigAttackBox.enabled)
                {
                    damageable.TakeDamage(bigDamage, transform);
                    Debug.Log("큰Collider 호출");
                }
            }
        }

        // 애니메이션 상태가 Idle로 돌아왔을 때 Attack을 false로 설정
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("ChairMonster_Attack") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            anim.SetBool("Attack", false);  // 애니메이션 끝났을 때 Attack을 false로
        }
    }

    // 기본 공격 콜라이더 활성화
    public void enbox()
    {
        normalAttackBox.enabled = true;
    }

    // 기본 공격 콜라이더 비활성화
    public void debox()
    {
        normalAttackBox.enabled = false;
    }

    // 큰 공격 콜라이더 활성화
    public void bigAttackOn()
    {
        bigAttackBox.enabled = true;
    }

    // 큰 공격 콜라이더 비활성화
    public void bigAttackOff()
    {
        bigAttackBox.enabled = false;
    }

    void OnDrawGizmos()
    {
        // Gizmos 색상 설정 (빨간색으로 설정)
        Gizmos.color = Color.red;

        // 감지할 박스의 크기와 위치 설정
        Vector2 boxSize = new Vector2(2f, 2f);
        Vector3 boxPosition = pos != null ? pos.position : transform.position; // pos가 null이 아니면 pos 사용, 그렇지 않으면 기본 transform 위치 사용

        Gizmos.DrawWireCube(boxPosition, boxSize);
    }

}

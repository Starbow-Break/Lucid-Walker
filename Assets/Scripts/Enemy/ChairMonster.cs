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
    private float currentTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentTime = 0;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        Collider2D collider = Physics2D.OverlapBox(pos.position, new Vector2(1f, 1f), 1);


        // 쿨타임이 끝났고 플레이어가 범위에 있을 때만 공격 시작
        if (collider != null && collider.CompareTag("Player") && currentTime <= 0)
        {
            anim.SetBool("Attack", true);  // Attack 애니메이션 시작
            currentTime = coolTime;

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (normalAttackBox.enabled)
                {
                    damageable.TakeDamage(damage, transform);
                }
                else if (bigAttackBox.enabled)
                {
                    damageable.TakeDamage(bigDamage, transform);
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
}

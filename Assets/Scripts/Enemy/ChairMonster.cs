using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairMonster : MonoBehaviour
{
    public Transform pos;
    public Animator anim;
    public int damage;
    public int bigDamage; // 더 강한 데미지
    public BoxCollider2D normalAttackBox; // 기본 공격 콜라이더
    public BoxCollider2D bigAttackBox; // 입 벌릴 때 사용될 큰 공격 콜라이더
    public float coolTime;
    private float currentTime;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D collider = Physics2D.OverlapBox(pos.position, new Vector2(1f, 1f), 1);

        if (collider != null && collider.CompareTag("Player"))
        {
            if (currentTime <= 0)
            {
                anim.SetTrigger("Attack");
                currentTime = coolTime;
            }
        }
        currentTime -= Time.deltaTime;
    }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bullet; // 총알 프리팹
    public Transform pos; // 총알 생성 위치
    public float cooltime; // 쿨타임
    private float curtime; // 현재 쿨타임

    public Animator anim; // 플레이어 애니메이터

    void Start()
    {
        anim = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    void Update()
    {
        if (curtime <= 0)
        {
            if (Input.GetKey(KeyCode.X))
            {
                // "Shoot" 트리거 실행
                if (anim != null)
                {
                    anim.SetTrigger("Shoot");
                }

                // 쿨타임 초기화
                curtime = cooltime;
            }
        }

        // 쿨타임 감소
        curtime -= Time.deltaTime;
    }

    // 애니메이션 이벤트로 호출될 메서드
    public void FireBullet()
    {
        if (bullet != null && pos != null)
        {
            Instantiate(bullet, pos.position, transform.rotation); // 총알 생성
        }
    }
}

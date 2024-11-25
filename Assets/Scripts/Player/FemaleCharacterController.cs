using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FemaleCharacterController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D capsuleCollider;


    public Transform groundChkFront;
    public Transform groundChkBack;
    public Transform wallChkUp;
    public LayerMask p_Layer;
    public float wallJumpPower;

    public float walkSpeed = 2f; // 걷기 속도
    public float runSpeed = 4f;  // 달리기 속도

    public float isRight = 1;    // 바라보는 방향 1 = 오른쪽, -1 = 왼쪽

    float input_x;
    public bool isGround;

    public float chkDistance;
    public float jumpPower = 4;  // 점프 파워
    public float jumpBoost = 2;  // 점프 초기 가속도
    public float fallMultiplier = 2.5f;  // 빠르게 떨어지기 위한 가속도
    public float lowJumpMultiplier = 2f; // 낮은 점프 속도
    bool isRunning;  // 달리기 상태 확인


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>(); // CapsuleCollider2D 컴포넌트 가져오기
    }

    private void Update()
    {
        input_x = Input.GetAxis("Horizontal");
        isRunning = Input.GetKey(KeyCode.LeftShift);  // Shift로 달리기

        // 캐릭터의 앞쪽과 뒤쪽의 바닥 체크
        bool ground_front = Physics2D.Raycast(groundChkFront.position, Vector2.down, chkDistance, p_Layer);
        bool ground_back = Physics2D.Raycast(groundChkBack.position, Vector2.down, chkDistance, p_Layer);

        // 점프 상태에서 앞 또는 뒤쪽에 바닥이 감지되면 바닥에 붙어서 이동하게 변경
        if (!isGround && (ground_back || ground_front))
            rb.velocity = new Vector2(rb.velocity.x, 0);

        // 앞 또는 뒤쪽의 바닥이 감지되면 isGround 변수를 참으로
        if (ground_back || ground_front)
            isGround = true;
        else
            isGround = false;

        anim.SetBool("isGround", isGround);

        // 달리기 상태일 때 run 애니메이션
        if (isGround)
        {
            if (input_x != 0)
            {
                anim.SetBool("walk", !isRunning);  // 걷기 상태 (Shift를 누르지 않았을 때)
                anim.SetBool("run", isRunning);    // 달리기 상태 (Shift를 누를 때)
            }
            else
            {
                anim.SetBool("walk", false);
                anim.SetBool("run", false);
            }
        }

        // 점프 애니메이션 트리거
        if (Input.GetAxis("Jump") != 0)
        {
            anim.SetTrigger("jump");
        }

        // 캐릭터 방향 전환
        if ((input_x > 0 && isRight < 0) || (input_x < 0 && isRight > 0))
        {
            FlipPlayer();
        }
    }

    private void FixedUpdate()
    {

        // 캐릭터 점프
        if (isGround && Input.GetAxis("Jump") != 0)
        {
            // 초기 점프 속도 증가를 위해 jumpBoost를 더해줌
            rb.velocity = Vector2.up * (jumpPower + jumpBoost);
        }

    }

    void FlipPlayer()
    {
        // 방향 전환
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        isRight = isRight == 1 ? -1 : 1;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;

    public Transform groundChkFront;
    public Transform groundChkBack;
    public Transform wallChk;
    public float wallchkDistance;
    public LayerMask w_Layer;
    public bool isWall;
    public float slidingSpeed;
    public float wallJumpPower;
    public bool isWallJump;

    public float runSpeed; // 이동속도
    public float isRight; // 바라보는 방향 1 = 오른쪽, -1 = 왼쪽

    float input_x;
    bool isGround;
    public float chkDistance;
    public float jumpPower = 1;
    public LayerMask g_layer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        input_x = Input.GetAxis("Horizontal");

        // 캐릭터의 앞쪽과 뒤쪽의 바닥 체크
        bool ground_front = Physics2D.Raycast(groundChkFront.position, Vector2.down, chkDistance, g_layer);
        bool ground_back = Physics2D.Raycast(groundChkBack.position, Vector2.down, chkDistance, g_layer);

        // 점프 상태에서 앞 또는 뒤쪽에 바닥이 감지되면 바닥에 붙어서 이동하게 변경
        if (!isGround && (ground_back || ground_front))
            rb.velocity = new Vector2(rb.velocity.x, 0);

        // 앞 또는 뒤쪽의 바닥이 감지되면 isGround 변수를 참으로
        if (ground_back || ground_front)
            isGround = true;
        else
            isGround = false;

        anim.SetBool("isGround", isGround);

        isWall = Physics2D.Raycast(wallChk.position, Vector2.right * isRight, wallchkDistance, w_Layer);
        anim.SetBool("isSliding", isWall);

        // 스페이스 바가 눌리면 점프 애니메이션
        if (Input.GetAxis("Jump") != 0)
        {
            anim.SetTrigger("jump");
        }

        // 방향키가 눌리는 방향과 캐릭터가 바라보는 방향이 다르다면 캐릭터의 방향 전환
        if (!isWallJump)
        {
            if ((input_x > 0 && isRight < 0) || (input_x < 0 && isRight > 0))
            {
                FlipPlayer();
                anim.SetBool("run", true);
            }
            else if (input_x == 0)
            {
                anim.SetBool("run", false);
            }
        }
    }

    private void FixedUpdate()
    {
        // 캐릭터 이동
        if (!isWallJump)
            rb.velocity = (new Vector2((input_x) * runSpeed, rb.velocity.y));
        if (isGround == true)
        {
            // 캐릭터 점프
            if (Input.GetAxis("Jump") != 0)
            {
                rb.velocity = Vector2.up * jumpPower;
            }
        }
        if (isWall)
        {
            isWallJump = false;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * slidingSpeed);

            if (Input.GetAxis("Jump") != 0)
            {
                isWallJump = true;
                Invoke("FreezeX", 0.3f);
                rb.velocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower);
                FlipPlayer();
            }
        }
    }

    void FreezeX()
    {
        isWallJump = false;
    }
    void FlipPlayer()
    {
        // 방향 전환
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        isRight = isRight * -1;
    }

    private void onDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundChkFront.position, Vector2.down * chkDistance);
        Gizmos.DrawRay(groundChkBack.position, Vector2.down * chkDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(wallChk.position, Vector2.right * isRight * wallchkDistance);
    }
}

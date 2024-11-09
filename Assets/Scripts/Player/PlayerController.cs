using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D capsuleCollider;


    public Transform groundChkFront;
    public Transform groundChkBack;
    public Transform wallChk;
    public float wallchkDistance;
    public LayerMask p_Layer;
    public LayerMask w_Layer;
    public bool isWall;
    public float slidingSpeed;
    public float wallJumpPower;
    public bool isWallJump;

    public float walkSpeed = 2f; // 걷기 속도
    public float runSpeed = 4f;  // 달리기 속도
    public float crouchWalkSpeed = 2f;

    public float isRight = 1;    // 바라보는 방향 1 = 오른쪽, -1 = 왼쪽

    float input_x;
    public bool isGround;
    bool isCrouching;
    bool isCrouchWalking;

    public float chkDistance;
    public float jumpPower = 4;  // 점프 파워
    public float jumpBoost = 2;  // 점프 초기 가속도
    public float fallMultiplier = 2.5f;  // 빠르게 떨어지기 위한 가속도
    public float lowJumpMultiplier = 2f; // 낮은 점프 속도
    bool isRunning;  // 달리기 상태 확인
    private Vector2 originalColliderSize;
    private Vector2 targetCrouchSize = new Vector2(1.0f, 0.9f); // 목표 crouch size
    private float crouchTransitionSpeed = 3f; // 크기 변환 속도


    #region Movable
    [SerializeField] Transform movableChk; // movable 체크하는 위치
    [SerializeField] float movableChkDistance; // 체크 거리
    [SerializeField] float maximumPushMass; // 밀수 있는 최대 중량
    #endregion

    HashSet<GameObject> pushMovable = new(); // 밀고있는 물체 상태 확인

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>(); // CapsuleCollider2D 컴포넌트 가져오기
        originalColliderSize = capsuleCollider.size; // 원래 size 값 저장

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
        isCrouching = Input.GetKey(KeyCode.DownArrow); // 아래 방향키로 crouch 상태
        isCrouchWalking = isCrouching && input_x != 0; // crouch 상태에서 좌우 이동 시 crouchWalk 상태

        anim.SetBool("isCrouching", isCrouching); // crouching 애니메이션
        anim.SetBool("isCrouchWalking", isCrouchWalking); // crouchWalk 애니메이션

        // CapsuleCollider2D의 size 조정
        if (isCrouching && !isCrouchWalking)
        {
            // crouching 상태에서만 Lerp로 점진적으로 변경
            capsuleCollider.size = Vector2.Lerp(capsuleCollider.size, targetCrouchSize, Time.deltaTime * crouchTransitionSpeed);
        }
        if (isCrouching && isCrouchWalking)
        {
            // crouching 상태에서만 Lerp로 점진적으로 변경
            capsuleCollider.size = new Vector2(capsuleCollider.size.x, 0.9f); // crouching 상태에서 y 크기를 1로 설정
        }
        else if (!isCrouching)
        {
            // crouching이 해제되면 원래 크기로 복구
            capsuleCollider.size = Vector2.Lerp(capsuleCollider.size, originalColliderSize, Time.deltaTime * crouchTransitionSpeed * 2);
        }

        isWall = Physics2D.Raycast(wallChk.position, Vector2.right * isRight, wallchkDistance, w_Layer);
        anim.SetBool("isSliding", !isGround && isWall);

        // 달리기 상태일 때 run 애니메이션
        if (!isWallJump && isGround)
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
        if (Input.GetAxis("Jump") != 0 && !isWallJump)
        {
            anim.SetTrigger("jump");
        }
        if (Input.GetAxis("Jump") != 0 && isWallJump)
        {
            anim.SetTrigger("wallJump");
        }


        // 캐릭터 방향 전환
        if (!isWallJump)
            if ((input_x > 0 && isRight < 0) || (input_x < 0 && isRight > 0))
            {
                FlipPlayer();
            }

    }

    private void FixedUpdate()
    {
        // 이전 프레임에서 밀었던 물체들의 상태와 정보를 초기화 한다.
        foreach (GameObject obj in pushMovable)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            anim.SetFloat("movable_mess", 0.0f);
            obj.GetComponent<SpriteRenderer>().color = Color.white; // Debug
        }
        pushMovable.Clear();

        // 캐릭터 이동
        if (!isWallJump && !isCrouchWalking)
        {
            float moveSpeed = isRunning ? runSpeed : walkSpeed;
            Vector2 velocity = new(input_x * moveSpeed, rb.velocity.y);

            // 땅에 서 있고 수평 방향으로 이동을 시도 한다면 밀고 있는 물체있는지 확인 후 속도에 반영
            if (isGround && velocity.x != 0.0f)
            {
                LayerMask movableLayer = LayerMask.GetMask("Movable");
                RaycastHit2D movableHit = Physics2D.Raycast(movableChk.position, Vector2.right * isRight, movableChkDistance, movableLayer);
                IMovable movable = movableHit.collider != null ? movableHit.collider.gameObject.GetComponent<IMovable>() : null;

                float totalMass = 0.0f; // 미는 물체들의 총 질량
                bool isMove = true; // 힘이 충분히 클 때 이동 기능 여부

                if (movable != null && movable.pushable)
                {
                    isMove = movable.GetAllOfMoveObject(isRight == 1, true, ref pushMovable);

                    foreach (GameObject obj in pushMovable)
                    {
                        IMovable movableObj = obj.GetComponent<IMovable>();
                        totalMass += movableObj != null ? movableObj.mass : 0.0f;
                    }
                }

                if (isMove)
                { // 힘이 충분히 클 때 미는게 가능하다면
                    // 밀고있는 물체의 중량에 따라 속도 감소
                    // 만약에 밀 수 있는 중량을 넘어서면 움직이지 않는다.
                    float weight = totalMass > maximumPushMass ? 0.0f : 1.0f / (1.0f + totalMass);
                    velocity = new(velocity.x * weight, velocity.y);
                    anim.SetFloat("movable_mess", totalMass / maximumPushMass);
                }
                else
                { // 불가능 하다면
                    velocity = new(0.0f, velocity.y);
                }

                foreach (GameObject obj in pushMovable)
                {
                    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                    rb.velocity = velocity.x * Vector2.right;

                    // Debug
                    if (isMove)
                    {
                        obj.GetComponent<SpriteRenderer>().color = Color.magenta;
                    }
                }
            }

            rb.velocity = velocity;
        }

        // crouchWalk 속도 적용
        if (isCrouchWalking)
        {
            rb.velocity = new Vector2(input_x * crouchWalkSpeed, rb.velocity.y);
        }

        // 캐릭터 점프
        if (isGround && Input.GetAxis("Jump") != 0)
        {
            // 초기 점프 속도 증가를 위해 jumpBoost를 더해줌
            rb.velocity = Vector2.up * (jumpPower + jumpBoost);
        }

        // 벽 슬라이드 및 점프
        if (!isGround && isWall)
        {
            isWallJump = false;

            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * slidingSpeed);
            anim.SetBool("isSliding", true);


            if (Input.GetAxis("Jump") != 0)
            {
                isWallJump = true;
                Invoke("FreezeX", 0.3f);
                rb.velocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower);
                FlipPlayer();  // 방향키에 맞춰 캐릭터 방향을 전환

            }

            // 벽 점프 후 캐릭터 방향 전환
            // if ((isRight < 0 && input_x > 0) || (isRight > 0 && input_x < 0))
            // {

            // }
        }

        // 중력 가속도 적용
        ApplyGravityModifiers();
    }

    void ApplyGravityModifiers()
    {
        // 점프 후 천천히 떨어지도록 중력 가속도를 증가시킴
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Space 키를 떼면 천천히 떨어지는 속도를 높임
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void FreezeX()
    {
        isWallJump = false;

        rb.velocity = new Vector2(0, rb.velocity.y);

    }

    void FlipPlayer()
    {
        // 방향 전환
        transform.eulerAngles = new Vector3(0, Mathf.Abs(transform.eulerAngles.y - 180), 0);
        isRight = isRight == 1 ? -1 : 1;
    }

    public void SetZiplineAnim(bool value)
    {
        anim.SetBool("zipline", value);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundChkFront.position, Vector2.down * chkDistance);
        Gizmos.DrawRay(groundChkBack.position, Vector2.down * chkDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(wallChk.position, Vector2.right * isRight * wallchkDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(movableChk.position, Vector2.right * isRight * movableChkDistance);
    }
}

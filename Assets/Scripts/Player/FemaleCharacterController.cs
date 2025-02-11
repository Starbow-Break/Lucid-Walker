using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FemaleCharacterController : MonoBehaviour
{
    public bool isActive = false; // 활성화 상태 플래그
    public PlayerData Data;
    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D capsuleCollider;

    private SpriteRenderer sr;

    [Header("2.5D Switch Layers")]

    [SerializeField] private string frontLayerName = "PlayerFront";
    [SerializeField] private string backLayerName = "PlayerBack";
    [SerializeField] private int frontSortingOrder = 10;
    [SerializeField] private int backSortingOrder = 5;

    // [레이캐스트 방식] 뒤 플랫폼 확인용
    [SerializeField] private float backPlatformCheckDistance = 3f;  // 뒤 플랫폼 감지 거리
    [SerializeField] private LayerMask backPlatformLayerMask;

    public LayerMask p_Layer;
    public LayerMask w_Layer;
    public LayerMask water_Layer;
    public bool IsFacingRight { get; private set; }
    public bool isRunning { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    //Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    #region INPUT PARAMETERS
    public Vector2 _moveInput;

    public float LastPressedJumpTime { get; private set; }
    // public float LastPressedDashTime { get; private set; }
    #endregion

    public bool isGround;

    public float chkDistance;

    #region CHECK PARAMETERS
    //Set all of these up in the inspector
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    #endregion
    private bool recentlySwitchedToBack = false;
    private float backLayerCooldown = 0.3f;
    private float backLayerTimer = 0f;

    public bool isDialogueActive = false; // 대화 중 상태 플래그 추가

    [Header("Jump Cooldown")]
    public float jumpCooldown = 0.5f; // 점프 후 쿨타임 (초)
    private float jumpCooldownTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
        capsuleCollider = GetComponent<CapsuleCollider2D>(); // CapsuleCollider2D 컴포넌트 가져오기

    }

    private void Update()
    {
        if (!isActive)
        {
            // 비활성화 상태일 때 Idle 상태 강제 설정
            SetToIdleState();
            return; // 입력 및 동작 처리 중지
        }

        // 쿨타임 타이머 업데이트
        if (jumpCooldownTimer > 0)
            jumpCooldownTimer -= Time.deltaTime;

        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;
        #endregion


        #region INPUT HANDLER
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true; // 달리기 상태를 활성화
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false; // 달리기 상태를 비활성화
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpUpInput();
        }

        #endregion

        #region COLLISION CHECKS
        if (!IsJumping)
        {
            //Ground Check
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, p_Layer)) //checks if set box overlaps with ground
            {
                if (LastOnGroundTime < -0.1f)
                {
                    isGround = true;
                }

                LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
            }

            // //Right Wall Check
            // if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, w_Layer) && IsFacingRight)
            //         || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, w_Layer) && !IsFacingRight)) && !IsWallJumping)
            //     LastOnWallRightTime = Data.coyoteTime;

            // //Right Wall Check
            // if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, w_Layer) && !IsFacingRight)
            //     || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, w_Layer) && IsFacingRight)) && !IsWallJumping)
            //     LastOnWallLeftTime = Data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
        #endregion

        #region 점프 도중에도 뒤 플랫폼 감지 (플랫폼 백 체크)
        if (!isGround && IsJumping) // 공중에 있을 때도 뒤 플랫폼 확인
        {
            Vector2 checkOrigin = transform.position;
            float checkDistance = backPlatformCheckDistance;

            // 캐릭터가 바라보는 방향과 반대 방향 + 아래쪽으로 Raycast
            Vector2 checkDir = (IsFacingRight ? Vector2.left : Vector2.right) + Vector2.down;

            // Raycast 실행 (뒤 플랫폼이 있는지 확인)
            RaycastHit2D hit = Physics2D.Raycast(checkOrigin, checkDir, checkDistance, backPlatformLayerMask);

            if (hit.collider != null)
            {
                // 감지되면 PlayerBack으로 변경
                gameObject.layer = LayerMask.NameToLayer(backLayerName);
                sr.sortingOrder = backSortingOrder;
                Debug.Log("점프 중 뒤 플랫폼 감지됨! PlayerBack으로 변경");

                recentlySwitchedToBack = true;
                backLayerTimer = backLayerCooldown;
            }
        }
        #endregion

        #region 애니메이션 상태 업데이트
        if (isGround) // 땅에 있을 때만 걷기, 달리기, 멈춤 상태 업데이트
        {

            if (Mathf.Abs(_moveInput.x) > 0.1f) // 이동 중
            {
                anim.SetBool("walk", !isRunning); // Shift가 눌리지 않으면 걷기
                anim.SetBool("run", isRunning);  // Shift가 눌리면 달리기
            }
            else // 이동하지 않음
            {
                anim.SetBool("walk", false);
                anim.SetBool("run", false);
            }
        }

        anim.SetBool("isGround", isGround); // 현재 Ground 상태 업데이트
        #endregion


        #region [레이캐스트 방식 추가]
        // 땅에 서 있으면서 "위 방향 + 스페이스" = 뒤 플랫폼 점프 시도
        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            // 1) 뒤쪽(캐릭터가 바라보는 반대 방향)으로 Raycast
            Vector2 checkOrigin = transform.position;
            float checkDistance = backPlatformCheckDistance;

            // 기존: "캐릭터가 보고 있는 방향의 반대쪽"
            Vector2 checkDir = IsFacingRight ? Vector2.left : Vector2.right;

            // 2) 경사가 있는 경우 대각선 방향도 체크
            Vector2 diagonalCheckDir = (IsFacingRight ? Vector2.left : Vector2.right) + Vector2.down;

            // 3) 실제 Raycast 검사 (수평 + 대각선)
            RaycastHit2D hitHorizontal = Physics2D.Raycast(checkOrigin, checkDir, checkDistance, backPlatformLayerMask);
            RaycastHit2D hitDiagonal = Physics2D.Raycast(checkOrigin, diagonalCheckDir, checkDistance, backPlatformLayerMask);

            if (hitHorizontal.collider != null || hitDiagonal.collider != null)
            {
                // 뒤 플랫폼 감지됨 → 레이어 전환 + 점프
                gameObject.layer = LayerMask.NameToLayer(backLayerName);
                sr.sortingOrder = backSortingOrder;
                Debug.Log("뒤 플랫폼 감지됨! 점프");

                recentlySwitchedToBack = true;
                backLayerTimer = backLayerCooldown;

                // 점프
                OnJumpInput();
            }
            else
            {
                // 2') 뒤 플랫폼이 근처에 없다면 → 그냥 일반 점프
                OnJumpInput();
            }
        }

        // // 그 외 점프 (예: 아무 입력 없이 Space만 눌렀을 때)
        // else if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     OnJumpInput();
        // }
        #endregion

        #region JUMP CHECKS
        if (IsJumping && rb.velocity.y < 0)
        {
            IsJumping = false;

            _isJumpFalling = true;
            isGround = false;

        }

        if (LastOnGroundTime > 0 && !IsJumping)
        {
            _isJumpCut = false;

            _isJumpFalling = false;

        }

        //Jump
        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;
            isGround = false;

            Jump();

            anim.SetTrigger("jump");
        }
        #endregion
        #region GRAVITY
        //Higher gravity if we've released the jump input or are falling

        if (rb.velocity.y < 0 && _moveInput.y < 0)
        {
            //Much higher gravity if holding down
            SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
        }
        else if (_isJumpCut)
        {
            //Higher gravity if jump button released
            SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
        }
        else if ((IsJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
        }
        else if (rb.velocity.y < 0)
        {
            //Higher gravity if falling
            SetGravityScale(Data.gravityScale * Data.fallGravityMult);
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
        }
        else
        {
            //Default gravity if standing on a platform or moving upwards
            SetGravityScale(Data.gravityScale);
        }
        #endregion

        if (recentlySwitchedToBack)
        {
            backLayerTimer -= Time.deltaTime;
            if (backLayerTimer <= 0f)
            {
                recentlySwitchedToBack = false;
            }
        }

        TrySwitchBackToFrontIfPlatformFound();


    }

    private void TrySwitchBackToFrontIfPlatformFound()
    {
        // 1) 아직 쿨타임 중이면 아예 검사 안 함
        if (recentlySwitchedToBack)
            return;

        // 1) 현재 레이어가 PlayerBack인지 확인
        if (gameObject.layer == LayerMask.NameToLayer(backLayerName))
        {
            // 2) 아래 방향에 앞 플랫폼이 있는지 OverlapBox로 확인
            //    (p_Layer 대신 "PlatformFront" 마스크를 직접 구해서)
            Collider2D frontHit = Physics2D.OverlapBox(
                _groundCheckPoint.position,
                _groundCheckSize,
                0f,
                LayerMask.GetMask("PlatformFront")
            );

            if (frontHit != null)
            {
                // 3) 발견되면 → PlayerFront로 복귀
                gameObject.layer = LayerMask.NameToLayer(frontLayerName);
                sr.sortingOrder = frontSortingOrder;
                Debug.Log("앞 플랫폼 발견 → PlayerFront로 전환");
            }
        }
    }
    private void FixedUpdate()
    {
        if (!isActive) return; // 비활성화 상태일 때 물리 처리 금지

        // 캐릭터 이동
        if (!IsWallJumping)
        {
            // 캐릭터의 이동 속도 계산 (걷기와 달리기 구분)
            float moveSpeed = isRunning ? Data.runMaxSpeed : Data.runMaxSpeed * 0.5f; // Shift 누르면 달리기 속도
            Vector2 velocity = new(_moveInput.x * moveSpeed, rb.velocity.y);

            // 실제 이동 속도 적용
            rb.velocity = velocity;
        }

        //Handle Run
        if (IsWallJumping)
            Run(Data.wallJumpRunLerp);
        else
            Run(1);

    }


    public void SetToIdleState()
    {
        rb.velocity = Vector2.zero; // 이동 중지
        anim.SetBool("walk", false);
        anim.SetBool("run", false);
        anim.SetBool("isGround", true);
        anim.Play("Idle"); // 강제로 Idle 애니메이션 재생
    }

    #region INPUT CALLBACKS

    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }
    public void OnJumpUpInput()
    {
        if (CanJumpCut())
            _isJumpCut = true;
    }
    #endregion


    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        //nameof() notation means we don't need to input a string directly.
        //Removes chance of spelling mistakes and will improve error messages if any
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1;
    }
    #endregion

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - rb.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
         * For those interested here is what AddForce() will do
         * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
         * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
        */
    }

    private void Turn()
    {
        //stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }
    #endregion


    #region JUMP METHODS
    private void Jump()
    {
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        jumpCooldownTimer = jumpCooldown;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = Data.jumpForce;
        if (rb.velocity.y < 0)
            force -= rb.velocity.y;

        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping && jumpCooldownTimer <= 0;
    }

    private bool CanJumpCut()
    {
        return IsJumping && rb.velocity.y > 0;
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool isActive = true; // 활성화 상태 플래그

    public PlayerData Data;
    Rigidbody2D rb;
    Animator anim;
    CapsuleCollider2D capsuleCollider;
    public LayerMask p_Layer;
    public LayerMask w_Layer;
    public LayerMask water_Layer;

    public bool IsFacingRight { get; private set; }
    public bool isRunning { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsSliding { get; private set; }

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;
    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;


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
    bool isCrouching;
    bool isCrouchWalking;
    [SerializeField] Transform waterChk; // water 체크하는 위치
    [SerializeField] Transform wallChkUp; // wall 위에 있는지 체크하는 위치

    public float chkDistance;
    public bool isInWater;

    #region CHECK PARAMETERS
    //Set all of these up in the inspector
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    private Vector2 originalColliderSize;
    private Vector2 targetCrouchSize = new Vector2(0.92f, 0.8f); // 목표 crouch size
    private float crouchTransitionSpeed = 5f; // 크기 변환 속도
    public float crouchWalkSpeed = 2f;
    public bool isDialogueActive = false; // 대화 중 상태 플래그 추가

    #region Movable
    [SerializeField] Transform movableChk; // movable 체크하는 위치
    [SerializeField] float movableChkDistance; // 체크 거리
    bool needUpdateMovableAnim = false; // 애니메이션 업데이트 필요 여부
    float anim_movable_mess_weight = 0.0f; // movable_mess 값
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;
        capsuleCollider = GetComponent<CapsuleCollider2D>(); // CapsuleCollider2D 컴포넌트 가져오기
        originalColliderSize = capsuleCollider.size; // 원래 size 값 저장
    }

    private void Update()
    {
        if (!isActive)
        {
            // 비활성화 상태일 때 Idle 상태 강제 설정
            SetToIdleState();
            return; // 입력 및 동작 처리 중지
        }
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
            if (isInWater)
            {
                SwimUpward(); // Swim upwards
            }
            else
            {
                OnJumpInput();
            }
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

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, w_Layer) && IsFacingRight)
                    || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, w_Layer) && !IsFacingRight)) && !IsWallJumping)
                LastOnWallRightTime = Data.coyoteTime;

            //Right Wall Check
            if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, w_Layer) && !IsFacingRight)
                || (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, w_Layer) && IsFacingRight)) && !IsWallJumping)
                LastOnWallLeftTime = Data.coyoteTime;

            //Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
        }
        #endregion


        isInWater = Physics2D.Raycast(waterChk.position, Vector2.down, chkDistance, water_Layer); // 로컬 변수가 아닌 클래스 변수 업데이트
        bool wall_up = Physics2D.Raycast(wallChkUp.position, Vector2.up, chkDistance, w_Layer);

        #region 애니메이션 상태 업데이트
        if (isGround) // 땅에 있을 때만 걷기, 달리기, 멈춤 상태 업데이트
        {

            anim.SetBool("isSwim", false);

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
        anim.SetBool("isSliding", IsSliding); // 슬라이딩 상태 업데이트

        #region SWIMMING LOGIC
        if (isInWater)
        {
            isGround = false;
            anim.SetBool("isSwim", true); // Set swim animation state

            // Allow upward movement
            if (_moveInput.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(Data.swimSpeed, rb.velocity.y + Data.swimAcceleration * Time.deltaTime));
            }
            else if (_moveInput.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(-Data.swimSpeed * 0.5f, rb.velocity.y - Data.swimAcceleration * Time.deltaTime)); // Slower descent
            }
            else
            {
                // 자연스러운 부력 적용 (속도 점진적으로 감소)
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Lerp(rb.velocity.y, 0, Time.deltaTime * 2));
            }

            // Adjust sprite rotation for direction
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            if (_moveInput.x > 0) // Moving right
            {
                if (_moveInput.y > 0)
                    targetRotation = Quaternion.Euler(0, 0, 20);
                else if (_moveInput.y < 0)
                    targetRotation = Quaternion.Euler(0, 0, -20);
            }
            else if (_moveInput.x < 0) // Moving left
            {
                if (_moveInput.y > 0)
                    targetRotation = Quaternion.Euler(0, 0, -20);
                else if (_moveInput.y < 0)
                    targetRotation = Quaternion.Euler(0, 0, 20);
            }

            // Smoothly rotate towards target
            float rotationSpeed = 5f; // Adjust rotation speed here
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            // anim.SetBool("isSwim", false); // Exit swim animation state
            // float rotationSpeed = 5f; // Adjust rotation speed here
            // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotationSpeed);
        }
        #endregion

        #endregion

        isCrouching = Input.GetKey(KeyCode.DownArrow);  // 아래 방향키로 crouch 상태
        isCrouchWalking = isCrouching && Mathf.Abs(_moveInput.x) > 0.1f;         // 옆으로 이동 중에 아래키를 누르면 즉시 crouchWalking으로 전환

        anim.SetBool("isCrouching", isCrouching); // crouching 애니메이션
        anim.SetBool("isCrouchWalking", isCrouchWalking); // crouchWalk 애니메이션

        // CapsuleCollider2D의 size 조정
        if (isCrouching || isCrouchWalking || isInWater)
        {
            capsuleCollider.size = Vector2.Lerp(capsuleCollider.size, targetCrouchSize, Time.deltaTime * crouchTransitionSpeed);
            Physics2D.SyncTransforms(); // 즉시 Physics 업데이트
        }
        else if (!isCrouching && !isInWater)
        {

            if (!wall_up)
            {
                // 머리 위에 공간이 있으면 Collider 크기 복구
                capsuleCollider.size = Vector2.Lerp(capsuleCollider.size, originalColliderSize, Time.deltaTime * crouchTransitionSpeed * 2);
                Physics2D.SyncTransforms(); // 즉시 Physics 업데이트
            }
            else
            {
                // 공간이 없으면 crouching 상태 유지
                isCrouching = true;
                anim.SetBool("isCrouching", true); // 애니메이션 유지
            }
        }

        #region JUMP CHECKS
        if (IsJumping && rb.velocity.y < 0)
        {
            IsJumping = false;

            _isJumpFalling = true;
            isGround = false;

        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
            isGround = false;

        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;

            _isJumpFalling = false;

        }


        //Jump
        if (CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            IsWallJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            isGround = false;

            Jump();

            anim.SetTrigger("jump");
        }
        //WALL JUMP
        else if (CanWallJump() && LastPressedJumpTime > 0)
        {
            IsWallJumping = true;
            IsJumping = false;
            _isJumpCut = false;
            _isJumpFalling = false;
            isGround = false;


            _wallJumpStartTime = Time.time;
            _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

            WallJump(_lastWallJumpDir);
            anim.SetTrigger("wallJump");
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
        {
            IsSliding = true;
            anim.SetBool("isSliding", true); // 애니메이션 상태 업데이트
        }
        else
        {
            IsSliding = false;
            anim.SetBool("isSliding", true); // 애니메이션 상태 업데이트
        }
        #endregion

        #region GRAVITY
        //Higher gravity if we've released the jump input or are falling
        if (IsSliding)
        {
            SetGravityScale(Data.slideGravityScale); // 슬라이딩 전용 낮은 중력 값
        }
        if (isInWater)
        {
            SetGravityScale(Data.slideGravityScale * 0.1f); // 수영 전용 낮은 중력 값
        }
        else if (rb.velocity.y < 0 && _moveInput.y < 0 && !isInWater)
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
        else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(rb.velocity.y) < Data.jumpHangTimeThreshold)
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

        #region MOVABLE_ANIMATION
        if (needUpdateMovableAnim)
        {
            anim.SetFloat("movable_mess", anim_movable_mess_weight);
            needUpdateMovableAnim = false;
        }
        #endregion

        // isWall = Physics2D.Raycast(wallChk.position, IsFacingRight ? Vector2.right : Vector2.left, wallchkDistance, w_Layer);
        anim.SetBool("isSliding", IsSliding);
    }

    private void FixedUpdate()
    {
        if (!isActive) return; // 비활성화 상태일 때 물리 처리 금

        // 캐릭터 이동
        if (!IsWallJumping)
        {
            // 캐릭터의 이동 속도 계산 (걷기와 달리기 구분)
            float moveSpeed = isRunning ? Data.runMaxSpeed : Data.runMaxSpeed * 0.5f; // Shift 누르면 달리기 속도
            Vector2 velocity = new(_moveInput.x * moveSpeed, rb.velocity.y);

            // 땅에 서 있고, 수평 방향으로 이동 중이면 밀고 있는 물체 체크
            if (isGround && Mathf.Abs(velocity.x) > 0.01f)
            {
                LayerMask movableLayer = LayerMask.GetMask("Movable");
                RaycastHit2D movableHit = Physics2D.Raycast(movableChk.position, IsFacingRight ? Vector2.right : Vector2.left, movableChkDistance, movableLayer);

                if (movableHit.collider != null)
                {
                    // 물체를 밀고 있을 때 애니메이션 상태 업데이트
                    float new_movable_mess = Mathf.Max(0.0f, 1 - Mathf.Abs(rb.velocity.x / velocity.x));
                    if (new_movable_mess != anim_movable_mess_weight)
                    {
                        anim_movable_mess_weight = new_movable_mess;
                        needUpdateMovableAnim = true;
                    }
                }
                else
                {
                    if (anim_movable_mess_weight != 0.0f)
                    {
                        anim_movable_mess_weight = 0.0f;
                        needUpdateMovableAnim = true;
                    }
                }
            }
            else
            {
                if (anim_movable_mess_weight != 0.0f)
                {
                    anim_movable_mess_weight = 0.0f;
                    needUpdateMovableAnim = true;
                }
            }

            // 실제 이동 속도 적용
            rb.velocity = velocity;
        }

        //Handle Run
        if (IsWallJumping)
            Run(Data.wallJumpRunLerp);
        else
            Run(1);

        //Handle Slide
        if (IsSliding)
            Slide();

        // // 물 속 이동 속도 제한
        // float waterMoveSpeed = isInWater ? walkSpeed * 0.5f : (isRunning ? runSpeed : walkSpeed); // 이름 변경
        // Vector2 waterVelocity = new Vector2(input_x * waterMoveSpeed, rb.velocity.y); // 이름 변경

        // if (isInWater)
        // {
        //     // 물 속에서의 부드러운 이동
        //     rb.velocity = new Vector2(waterVelocity.x * 0.8f, rb.velocity.y * 0.9f); // 이름 변경
        // }
        // else
        // {
        //     // 물 밖에서는 일반 이동
        //     rb.velocity = waterVelocity;
        // }

        // crouchWalk 속도 적용
        if (isCrouchWalking)
        {
            rb.velocity = new Vector2(_moveInput.x * crouchWalkSpeed, rb.velocity.y);
        }
    }

    private void SetToIdleState()
    {
        rb.velocity = Vector2.zero; // 이동 중지
        anim.SetBool("walk", false);
        anim.SetBool("run", false);
        anim.SetBool("isGround", true);
        anim.SetBool("isSliding", false);
        anim.SetBool("isCrouching", false);
        anim.SetBool("isCrouchWalking", false);
        anim.Play("Idle"); // 강제로 Idle 애니메이션 재생
    }

    #region INPUT CALLBACKS

    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }


    private void SwimUpward()
    {
        rb.velocity = new Vector2(rb.velocity.x, Data.swimSpeed); // Apply upward velocity
    }
    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }

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

    public void Turn()
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

    private void WallJump(int dir)
    {
        //Ensures we can't call Wall Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        #region Perform Wall Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= rb.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        rb.AddForce(force, ForceMode2D.Impulse);

        // 애니메이터에 wallJump 트리거 전달
        // AnimHandler.startedJumping = true;
        anim.SetTrigger("wallJump");
        #endregion
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        //We remove the remaining upwards Impulse to prevent upwards sliding
        if (rb.velocity.y > 0)
        {
            rb.AddForce(-rb.velocity.y * Vector2.up, ForceMode2D.Impulse);
        }

        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = Data.slideSpeed - rb.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        rb.AddForce(movement * Vector2.up);
    }
    #endregion

    public void SetZiplineAnim(bool value)
    {
        anim.SetBool("zipline", value);
    }

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
             (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && rb.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && rb.velocity.y > 0;
    }


    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion
}
#endregion
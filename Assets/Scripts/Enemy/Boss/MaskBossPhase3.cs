using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MaskBossPhase3 : MaskBoss
{
    [SerializeField] LayerMask groundLayer;

    [Header("Bones")]
    [SerializeField] Transform body;
    [SerializeField] Transform frontHand;
    [SerializeField] Transform backHand;

    [Header("Camera Shake")]
    [SerializeField] float shakeIntensity = 5.0f;
    [SerializeField] float shakeTime = 0.1f;

    [SerializeField] private List<Transform> groundCheckTransforms;
    [SerializeField] private Transform filpPivot;
    
    public bool isGround;

    public Vector3 bodyLocalPosition {
        get { return body.localPosition; }
    }

    Rigidbody2D rb;
    Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<MaskBossStats>();

        tongueSkill = GetComponent<TongueSkill>();
        handAttackSkill = GetComponent<HandAttackSkill>();
        spitSkill = GetComponent<SpitSkill>();
        anim = GetComponent<Animator>();
    }

    protected override void Start() {
        base.Start();

        isGround = false;
        gameObject.SetActive(false);
        attackCoolDownRemain = stats.attackBatTime;
    } 

    void Update()
    {
        // 손 방향을 올바른 방향으로 조정
        frontHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
        backHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
    
        // if(Input.GetKeyDown(KeyCode.Alpha1)) {
        //     tongueSkill.Cast();
        // }
        // if(Input.GetKeyDown(KeyCode.Alpha2)) {
        //     handAttackSkill.Cast();
        // }
        // if(Input.GetKeyDown(KeyCode.Alpha3)) {
        //     spitSkill.Cast();
        // }

        // 싸우는 상태가 아니거나 사망 상태이면 아무 행동도 하지 않는다.
        if(!battle || isDead) return;

        attackCoolDownRemain -= Time.deltaTime;

        if(attackCoolDownRemain <= 0.0f) {
            attackCoolDownRemain = stats.attackBatTime;
            Think();
        }
        
        anim.SetBool("ground", isGround);
    }

    void FixedUpdate()
    {
        if (!isGround && rb.velocity.y <= 0.0f && CheckGround())
        {
            Shake();
            isGround = true;
            if(!battle) {
                BattleStart();
            }
        }
    }

    #region AI
    // 스킬의 우선순위가 높은 순서대로 정렬된 리스트
    LinkedList<int> skillProrityList = new LinkedList<int>(new List<int>{0, 1, 2});
    
    // 생각
    void Think() {
        LinkedListNode<int> node = skillProrityList.First;

        while(node != null) {
            if(TryCastSkill(node.Value)) {
                skillProrityList.Remove(node);
                skillProrityList.AddLast(node);
                break;
            }
            node = node.Next;
        }
    }

    // 스킬 사용 시도, 스킬 시전 여부를 반환
    bool TryCastSkill(int skillId) {
        Skill skill = GetSkillWithId(skillId);
        if(skill.isReady) {
            skill.Cast();
            return true;
        }
        return false;
    }
    #endregion

    #region SKILL
    TongueSkill tongueSkill;
    HandAttackSkill handAttackSkill;
    SpitSkill spitSkill;

    // 스킬 아이디로부터 스킬 획득
    Skill GetSkillWithId(int skillId) {
        return skillId switch
        {
            0 => handAttackSkill,
            1 => tongueSkill,
            2 => spitSkill,
            _ => null,
        };
    }
    #endregion

    // body를 dir만큼 움직인다,
    public void MoveBody(Vector3 dir) {
        body.localPosition += dir;
    }

    public void Flip() {
        filpPivot.transform.localScale = new (
            filpPivot.transform.localScale.x * -1,
            filpPivot.transform.localScale.y,
            filpPivot.transform.localScale.z
        );
    }

    public void TriggerJump()
    {
        anim.SetTrigger("jump");
    }

    private void Jump()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector2.up * 70.0f, ForceMode2D.Impulse);
        isGround = false;
    }

    private bool CheckGround()
    {
        bool result = true;
        
        foreach (Transform groundCheckTransform in groundCheckTransforms)
        {
            if (!Physics2D.Raycast(groundCheckTransform.position, Vector2.down, 0.3f, groundLayer))
            {
                result = false;
                break;
            }
        }
    
        return result;
    }
    
    public void Shake() {
        CameraShake.instance.ShakeActiveCamera(shakeIntensity, shakeTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform groundCheckTransform in groundCheckTransforms)
        {
            Gizmos.DrawRay(groundCheckTransform.position, Vector2.down * 0.3f);
        }
    }
}

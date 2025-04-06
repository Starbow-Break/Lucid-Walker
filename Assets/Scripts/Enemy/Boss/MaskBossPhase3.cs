using System.Collections.Generic;
using UnityEngine;

public class MaskBossPhase3 : MonoBehaviour
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
    TongueSkill tongueSkill;
    HandAttackSkill handAttackSkill;
    SpitSkill spitSkill;
    Animator anim;

    void Start() {
        rb = GetComponent<Rigidbody2D>();

        tongueSkill = GetComponent<TongueSkill>();
        handAttackSkill = GetComponent<HandAttackSkill>();
        spitSkill = GetComponent<SpitSkill>();
        anim = GetComponent<Animator>();

        isGround = false;
        gameObject.SetActive(false);
    } 

    void Update()
    {
        // 손 방향을 올바른 방향으로 조정
        frontHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
        backHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
    
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            tongueSkill.Cast();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            handAttackSkill.Cast();
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)) {
            spitSkill.Cast();
        }
        
        anim.SetBool("ground", isGround);
    }

    void FixedUpdate()
    {
        if (!isGround && rb.velocity.y <= 0.0f && CheckGround())
        {
            Shake();
            isGround = true;
        }
    }

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

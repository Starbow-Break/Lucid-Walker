using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class MaskBossPhase3 : MonoBehaviour
{
    [Header("Bones")]
    [SerializeField] Transform body;
    [SerializeField] Transform frontHand;
    [SerializeField] Transform backHand;

    [SerializeField] private List<Transform> groundCheckTransforms;

    private Vector3 landingFrontArmSolverPosition;
    private Vector3 landingBackArmSolverPosition;

    public bool isGround;

    public Vector3 bodyLocalPosition {
        get { return body.localPosition; }
    }

    Rigidbody2D rb;
    TongueSkill tongueSkill;
    ThreatSkill threatSkill;
    Animator anim;
    Coroutine idleCoroutine;

    void Start() {
        rb = GetComponent<Rigidbody2D>();

        tongueSkill = GetComponent<TongueSkill>();
        threatSkill = GetComponent<ThreatSkill>();
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
            threatSkill.Cast();
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerJump();
        }
        
        anim.SetBool("ground", isGround);
    }

    void FixedUpdate()
    {
        if (!isGround && rb.velocity.y < 0.0f && CheckGround())
        {
            isGround = true;
        }
    }

    // body를 dir만큼 움직인다,
    public void MoveBody(Vector3 dir) {
        body.localPosition += dir;
    }

    public void TriggerJump()
    {
        anim.SetTrigger("jump");
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * 60.0f, ForceMode2D.Impulse);
        isGround = false;
    }

    private bool CheckGround()
    {
        bool result = true;
        
        foreach (Transform groundCheckTransform in groundCheckTransforms)
        {
            if (!Physics2D.Raycast(groundCheckTransform.position, Vector2.down, 0.2f, LayerMask.GetMask("Platform")))
            {
                result = false;
                break;
            }
        }
    
        Debug.Log(result);
        return result;
    }
    
    private void Shake() {
        CameraShake.instance.ShakeActiveCamera(5.0f, 0.1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Transform groundCheckTransform in groundCheckTransforms)
        {
            Gizmos.DrawRay(groundCheckTransform.position, Vector2.down * 0.2f);
        }
    }
}

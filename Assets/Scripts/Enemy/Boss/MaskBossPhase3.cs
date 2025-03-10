using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class MaskBossPhase3 : MonoBehaviour
{
    [Header("Bones")]
    [SerializeField] Transform body;
    [SerializeField] Transform frontHand;
    [SerializeField] Transform backHand;
    
    [Header("Solver Targets")]
    [SerializeField] Transform frontArmSolverTarget;
    [SerializeField] Transform backArmSolverTarget;

    private Vector3 landingFrontArmSolverPosition;
    private Vector3 landingBackArmSolverPosition;

    public bool idle {
        get {
            return anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        }
    }

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

        gameObject.SetActive(false);
    } 

    void Update()
    {
        // 손 방향을 올바른 방향으로 조정
        frontHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
        backHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);

        // 땅 위에 있고 공격하지 않는 상태면 Idle 상태이다.
        if (idle) {
            idleCoroutine ??= StartCoroutine(IdleActionFlow());
        }
        else {
            if (idleCoroutine != null) {
                StopCoroutine(idleCoroutine);
                idleCoroutine = null;
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            tongueSkill.Cast();
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)) {
            threatSkill.Cast();
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            Jump();
        }
    }

    // body를 dir만큼 움직인다,
    public void MoveBody(Vector3 dir) {
        body.localPosition += dir;
    }

    public void Jump()
    {
        StartCoroutine(JumpFlow());
    }

    IEnumerator IdleActionFlow() {
        float currentTime = 0.0f;
        float period = 2.0f;

        while(true) {
            yield return null;
            currentTime = (currentTime + Time.deltaTime) % period;
            body.localPosition = Vector3.Lerp(0.7f * Vector3.up, 1.0f * Vector3.up, Mathf.Cos(currentTime / period * 2.0f * Mathf.PI) / 2 + 0.5f);
        }
    }
    
    IEnumerator JumpFlow()
    {
        anim.SetBool("air", true);
        yield return JumpReadyFlow();
        yield return JumpingFlow();
        
    }
    
    // 상체를 살짝 내리면서 점프 준비
    IEnumerator JumpReadyFlow()
    {
        float duration = 0.3f;
        float time = 0.0f;
        float startY = transform.position.y;
        float targetY = -40.2f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float nextY = Mathf.Lerp(startY, targetY, time / duration);
            transform.position = new(transform.position.x, nextY, transform.position.z);
            frontArmSolverTarget.position = landingFrontArmSolverPosition;
            backArmSolverTarget.position = landingBackArmSolverPosition;
            yield return null;
        }
    }
    
    // 점프 중
    IEnumerator JumpingFlow()
    {
        Vector3 initVelocity = Vector3.up * 40.0f;
        Vector3 gravity = Vector3.down * 10.0f;
        
        Vector3 velocity = initVelocity;
        
        float duration = 1.0f;
        float time = 0.0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            velocity += gravity * Time.deltaTime;
            transform.Translate(velocity * Time.deltaTime, Space.World);
            if (transform.position.y - landingFrontArmSolverPosition.y < 8.0f)
            {
                frontArmSolverTarget.position = landingFrontArmSolverPosition;
                backArmSolverTarget.position = landingBackArmSolverPosition;
            }
            yield return null;
        }
    }

    void Shake() {
        CameraShake.instance.ShakeActiveCamera(5.0f, 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Platform"))
        {
            if(anim.GetBool("air"))
            {
                anim.SetBool("air", false);
                CameraShake.instance.ShakeActiveCamera(5.0f, 0.1f);
                rb.isKinematic = true;
                landingFrontArmSolverPosition = frontArmSolverTarget.position;
                landingBackArmSolverPosition = backArmSolverTarget.position;
            }
        }
    }
}

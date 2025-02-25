using System.Collections;
using UnityEngine;

public class MaskBossPhase3 : MonoBehaviour
{
    [Header("Bones")]
    [SerializeField] Transform body;
    [SerializeField] Transform frontHand;
    [SerializeField] Transform backHand;

    public bool isGround { get; private set; }
    public bool idle {
        get {
            return anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        }
    }

    public Vector3 bodyLocalPosition {
        get { return body.localPosition; }
    }

    TongueSkill tongueSkill;
    Animator anim;
    Coroutine idleCoroutine;

    void Start() {
        tongueSkill = GetComponent<TongueSkill>();
        anim = GetComponent<Animator>();
        isGround = true;
    } 

    void Update()
    {
        Debug.Log(idle);
        // 현재 땅 위에 있다면 손 방향을 올바른 방향으로 조정
        if (isGround) {
            frontHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
            backHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
        }

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
    }

    // body를 dir만큼 움직인다,
    public void MoveBody(Vector3 dir) {
        body.localPosition += dir;
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
}

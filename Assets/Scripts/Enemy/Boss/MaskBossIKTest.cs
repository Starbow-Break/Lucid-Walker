using System.Collections;
using UnityEngine;

public class MaskBossIKTest : MonoBehaviour
{
    [Header("Bones")]
    [SerializeField] Transform body;
    [SerializeField] Transform frontHand;
    [SerializeField] Transform backHand;

    public Vector3 bodyLocalPosition {
        get { return body.localPosition; }
    }

    TongueSkill tongueSkill;
    Animator anim;

    void Start() {
        tongueSkill = GetComponent<TongueSkill>();
    } 

    void Update()
    {
        frontHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
        backHand.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);

        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            tongueSkill.Cast();
        }
    }

    // body를 dir만큼 움직인다,
    public void MoveBody(Vector3 dir) {
        body.localPosition += dir;
    }
}

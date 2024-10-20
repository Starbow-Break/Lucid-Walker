using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Case : MonoBehaviour, IMovable
{
    [Tooltip("True로 설정 시 케이스를 이동시킬 수 있다.")]
    [SerializeField] bool _pushable = false;

    [Tooltip("질량")]
    [SerializeField] float _mass = 1.0f;

    [Tooltip("물체를 밀 수 있다면 밀리는 상태를 체크할 최대 거리")]
    [SerializeField] float pushCheckDistance;
    
    public bool pushable { get; set; }
    public float mass { get; set; }

    Rigidbody2D rb; // RigidBody
    SpriteRenderer sr;
    // 물체의 크기
    float caseWidth;
    float caseHeight;

    void Awake()
    {
        pushable = _pushable;
        mass = _mass;
        
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        sr = GetComponent<SpriteRenderer>();
        caseWidth = sr.bounds.max.x - sr.bounds.min.x;
        caseHeight = sr.bounds.max.y - sr.bounds.min.y;
    }

    void FixedUpdate()
    {
        if(rb.totalForce == Vector2.zero)
        {
            rb.velocity = new(0.0f, rb.velocity.y);
        }
    }

    // 밀리는 물체를 output에 전달
    // 반환값은 물체들의 이동 가능 여부
    public bool GetAllOfMoveObject(bool isRight, bool checkPushable, ref HashSet<GameObject> output) {
        output.Add(gameObject);

        // pushable을 체크 하는데 pushable이 아닌 오브젝트가 있다면 밀기 불가능
        if(checkPushable && !pushable) return false;

        LayerMask movableLayer = LayerMask.GetMask("Movable");

        // 수평 방향에 Movable이 있는지 확인
        RaycastHit2D hitHorizontal = Physics2D.BoxCast(
            (Vector2)transform.position + (caseWidth / 2 + pushCheckDistance / 2) * (isRight ? Vector2.right : Vector2.left) + caseHeight / 2 * Vector2.up,
            new(pushCheckDistance / 2, caseHeight / 2 * 0.9f),
            0.0f,
            isRight ? Vector2.right : Vector2.left,
            0.0f,
            movableLayer
        );

        // 있다면 해당 오브젝트로 이동하여 계속 추출
        // 단, pushable을 체크하고 있었다면 다음 오브젝트에서도 그대로 체크한다.
        IMovable movableHorizontal = hitHorizontal.collider != null ? hitHorizontal.collider.gameObject.GetComponent<IMovable>() : null;
        if(movableHorizontal != null) {
            MonoBehaviour movableMono = movableHorizontal as MonoBehaviour;
            if(movableMono != null && !output.Contains(movableMono.gameObject)) {
                if(!movableHorizontal.GetAllOfMoveObject(isRight, checkPushable, ref output)) {
                    return false;
                }
            }
        }

        // 윗 방향에 Movable이 있는지 확인
        RaycastHit2D hitTop = Physics2D.BoxCast(
            (Vector2)transform.position + (caseHeight + pushCheckDistance / 2) * Vector2.up,
            new(caseWidth / 2 * 0.9f, pushCheckDistance / 2),
            0.0f,
            isRight ? Vector2.right : Vector2.left,
            0.0f,
            movableLayer
        );

        // 있다면 해당 오브젝트로 이동하여 계속 추출
        // 윗방향에 있는 오브젝트는 pushable을 체크할 필요가 없다. 하지만 물체의 크기가 일정하지 않으므로 수평 방향부터 확인
        IMovable movableTop = hitTop.collider != null ? hitTop.collider.gameObject.GetComponent<IMovable>() : null;
        if(movableTop != null) {
            MonoBehaviour movableMono = movableTop as MonoBehaviour;
            if(movableMono != null && !output.Contains(movableMono.gameObject)) {
                movableTop.GetAllOfMoveObject(isRight, false, ref output);
            }
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        SpriteRenderer _sr = GetComponent<SpriteRenderer>();
        float w = _sr.bounds.max.x - _sr.bounds.min.x;
        float h = _sr.bounds.max.y - _sr.bounds.min.y;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(
            transform.position + (w / 2 + pushCheckDistance / 2) * Vector3.right + h / 2 * Vector3.up,
            new Vector3(pushCheckDistance, h * 0.9f, 0.5f)
        );

        Gizmos.DrawWireCube(
            transform.position + (w / 2 + pushCheckDistance / 2) * Vector3.left + h / 2 * Vector3.up,
            new Vector3(pushCheckDistance, h * 0.9f, 0.5f)
        );

        Gizmos.DrawWireCube(
            transform.position + (h + pushCheckDistance / 2) * Vector3.up,
            new Vector3(w * 0.9f, pushCheckDistance, 0.5f)
        );
        
    }
}

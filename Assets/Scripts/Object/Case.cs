using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Case : MonoBehaviour, IMovable
{
    [Tooltip("True로 설정 시 케이스를 이동시킬 수 있다.")]
    [SerializeField] bool _pushable = false;
    
    public bool pushable { get; set; }

    Rigidbody2D rb; // RigidBody
    SpriteRenderer sr;
    // 물체의 크기
    float caseWidth;
    float caseHeight;

    void Awake()
    {
        pushable = _pushable;
        
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = !pushable;

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

    // 밀리는 모든 오브젝트들을 반환
    // output에 전부 들어간다.
    public void GetAllOfMoveObject(bool isRight, bool checkPushable, ref HashSet<GameObject> output) {
        output.Add(gameObject);

        // 수평 방향에 Movable이 있는지 확인
        RaycastHit2D hitHorizontal = Physics2D.BoxCast(
            (Vector2)transform.position + (caseWidth / 2 + 0.1f) * (isRight ? Vector2.right : Vector2.left) + caseHeight / 2 * Vector2.up,
            new(0.1f, caseHeight / 2 - 0.1f),
            0.0f,
            isRight ? Vector2.right : Vector2.left,
            0.0f,
            LayerMask.GetMask("Platform")
        );

        // 있다면 해당 오브젝트로 이동하여 계속 추출
        // 단, pushable을 체크하고 있었다면 다음 오브젝트에서도 그대로 체크한다.
        IMovable movableHorizontal = hitHorizontal.collider != null ? hitHorizontal.collider.gameObject.GetComponent<IMovable>() : null;
        if(movableHorizontal != null && (!checkPushable || pushable)) {
            MonoBehaviour movableMono = movableHorizontal as MonoBehaviour;
            if(movableMono != null && !output.Contains(movableMono.gameObject)) {
                movableHorizontal.GetAllOfMoveObject(isRight, checkPushable, ref output);
            }
        }

        // 윗 방향에 Movable이 있는지 확인
        RaycastHit2D hitTop = Physics2D.BoxCast(
            (Vector2)transform.position + (caseHeight + 0.1f) * Vector2.up,
            new(caseWidth / 2 - 0.1f, 0.1f),
            0.0f,
            isRight ? Vector2.right : Vector2.left,
            0.0f,
            LayerMask.GetMask("Platform")
        );

        // 있다면 해당 오브젝트로 이동하여 계속 추출
        // 윗방향에 있는 오브젝트는 pushable을 체크할 필요가 없다. 하지만 물체의 크기가 일정하지 않으므로 수평 방향부터 확인
        IMovable movableTop = hitTop.collider != null ? hitTop.collider.gameObject.GetComponent<IMovable>() : null;
        if(movableTop != null) {
            MonoBehaviour movableMono = movableTop as MonoBehaviour;
            if(movableMono != null && !output.Contains(movableMono.gameObject)) {
                movableTop.GetAllOfMoveObject(isRight, checkPushable, ref output);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(
            transform.position + caseWidth / 2 * Vector3.right + caseHeight / 2 * Vector3.up,
            new Vector3(0.2f, caseHeight, 0.5f)
        );

        Gizmos.DrawWireCube(
            transform.position + caseWidth / 2 * Vector3.left + caseHeight / 2 * Vector3.up,
            new Vector3(0.2f, caseHeight, 0.5f)
        );

        Gizmos.DrawWireCube(
            transform.position + caseHeight * Vector3.up,
            new Vector3(caseWidth, 0.2f, 0.5f)
        );
        
    }
}

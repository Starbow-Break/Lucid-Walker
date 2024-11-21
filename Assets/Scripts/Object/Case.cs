using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

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

    SpriteRenderer sr;
    Rigidbody2D rb;

    // 물체의 크기
    float caseWidth;
    float caseHeight;

    [SerializeField] LayerMask platformLayer;
    LayerMask movableLayer;
    

    void Awake()
    {
        pushable = _pushable;
        mass = _mass;

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        caseWidth = sr.bounds.max.x - sr.bounds.min.x;
        caseHeight = sr.bounds.max.y - sr.bounds.min.y;

        movableLayer = LayerMask.GetMask("Movable");
    }

    void Update()
    {
        // 상자 바닥에 물체가 있는지 확인
        RaycastHit2D hitBottom = Physics2D.BoxCast(
            (Vector2)transform.position + 0.01f * Vector2.down,
            new(caseWidth / 2, 0.01f),
            0.0f,
            Vector2.down,
            20.0f,
            movableLayer | platformLayer
        );

        if(hitBottom.collider == null) {
            rb.velocity += Time.deltaTime * (Vector2)Physics.gravity;
        }
        else {
            if(hitBottom.point.y > transform.position.y + Time.deltaTime * rb.velocity.y) {
                transform.Translate(hitBottom.point - (Vector2)transform.position);
                rb.velocity = new(rb.velocity.x, 0.0f);
            }
            else {
                rb.velocity += Time.deltaTime * (Vector2)Physics.gravity;
            }
            
        }
    }

    // 밀리는 물체를 output에 전달
    // 반환값은 물체들의 이동 가능 여부
    public bool GetAllOfMoveObject(bool isRight, bool checkPushable, ref HashSet<GameObject> output) {
        output.Add(gameObject);

        // pushable을 체크 하는데 pushable이 아닌 오브젝트가 있다면 밀기 불가능
        if(checkPushable && !pushable) return false;

        // 수평 방향에 Movable이나 미는걸 가로막는 오브젝트가 있는지 확인
        RaycastHit2D hitHorizontal = Physics2D.BoxCast(
            (Vector2)transform.position + (caseWidth / 2 + pushCheckDistance / 2) * (isRight ? Vector2.right : Vector2.left) + caseHeight / 2 * Vector2.up,
            new(pushCheckDistance / 2, caseHeight / 2 * 0.9f),
            0.0f,
            isRight ? Vector2.right : Vector2.left,
            0.0f,
            movableLayer | platformLayer
        );

        // 감지된게 있다면
        Collider2D horizontalCollider = hitHorizontal.collider;
        if(horizontalCollider != null) {
            Debug.Log((1 << horizontalCollider.gameObject.layer) & platformLayer);
            // Movable이 아닌경우 이동 불가
            if(((1 << horizontalCollider.gameObject.layer) & platformLayer) != 0) {
                return false;
            }

            // Movable이면 추가 탐색
            IMovable movableHorizontal = horizontalCollider.gameObject.GetComponent<IMovable>();
            if(movableHorizontal != null) {
                MonoBehaviour movableMono = movableHorizontal as MonoBehaviour;
                if(movableMono != null && !output.Contains(movableMono.gameObject)) {
                    if(!movableHorizontal.GetAllOfMoveObject(isRight, true, ref output)) {
                        return false;
                    }
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
                return movableTop.GetAllOfMoveObject(isRight, false, ref output);
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

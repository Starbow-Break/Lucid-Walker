using UnityEngine;

public class ChargingMonster : MonoBehaviour
{
    private Collider2D hitboxCollider; // 데미지용 트리거
    private Collider2D bodyCollider;   // 물리 충돌 감지용

    [Header("Monster Settings")]
    public float moveSpeed = 3f;
    public int damage = 2;
    private bool isFacingRight = true;

    [Header("Slope Alignment Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 1f;
    [SerializeField, Range(0f, 1f)] private float rotationLerpSpeed = 0.1f; // 회전 보간 속도

    [Header("Collision Settings")]
    [SerializeField] private LayerMask playerLayer;          // 플레이어 감지 레이어
    [SerializeField] private Vector2 attackSize = new Vector2(0.5f, 0.6f);
    [Header("Visual Settings")]
    [SerializeField] private Transform spriteTransform;      // 몬스터의 시각적 표현(자식 오브젝트)

    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col.isTrigger) hitboxCollider = col;  // 트리거 콜라이더 찾기
            else bodyCollider = col;  // 일반 충돌 콜라이더 찾기
        }
    }

    void Update()
    {
        MoveForward();
        AlignVisual();
    }

    void MoveForward()
    {
        // 오브젝트의 로컬 스케일에 따라 방향 결정 (양수: 오른쪽, 음수: 왼쪽)
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        Vector2 origin = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            // 경사면의 법선 벡터 가져오기
            Vector2 groundNormal = hit.normal;
            // 접선 벡터 계산 (경사면을 따라가는 방향)
            Vector2 tangent = new Vector2(groundNormal.y, -groundNormal.x);
            // 로컬 스케일의 방향을 곱하여 이동 방향 결정
            Vector2 moveDirection = direction > 0 ? tangent : -tangent;
            rb.velocity = moveDirection.normalized * moveSpeed;
        }
        else
        {
            // 경사면이 아닌 경우, 로컬 스케일 기반으로 기본 이동 적용
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
        }
    }


    void AlignVisual()
    {
        Vector2 origin = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            float angle = Vector2.SignedAngle(Vector2.up, hit.normal);
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            if (spriteTransform != null)
            {
                spriteTransform.rotation = Quaternion.Lerp(spriteTransform.rotation, targetRotation, rotationLerpSpeed);
            }
        }
        else
        {
            if (spriteTransform != null)
            {
                spriteTransform.rotation = Quaternion.Lerp(spriteTransform.rotation, Quaternion.identity, rotationLerpSpeed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 boxCenter = (Vector2)transform.position + (isFacingRight ? Vector2.right * 0.5f : Vector2.left * 0.5f);
        Gizmos.DrawWireCube(boxCenter, attackSize);

        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}

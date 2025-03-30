using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Animator anim;
    public float speed = 10f;         // 총알 속도
    public float distance = 1f;       // Raycast 거리
    public LayerMask isLayer;         // 충돌 레이어

    // 총알의 공격력 (일반 총알은 1, 큰 총알은 2 이상의 값으로 Inspector에서 설정)
    public int damage = 1;

    private bool hasHit = false;      // 충돌 여부

    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("DestroyBullet", 2f);  // 2초 후 파괴
    }

    void Update()
    {
        if (hasHit) return;  // 충돌 후에는 이동 멈춤

        // Raycast로 충돌 체크
        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, distance, isLayer);
        if (ray.collider != null)
        {
            anim.SetTrigger("Hit"); // 히트 애니메이션 실행
            HandleHit(ray.collider); // 충돌 처리
        }

        // 플레이어 방향에 따라 총알 이동
        transform.Translate(Vector3.right * transform.localScale.x * speed * Time.deltaTime);
    }

    void HandleHit(Collider2D collider)
    {
        hasHit = true;

        // 적 태그인 경우 데미지 적용
        if (collider.CompareTag("Enemy"))
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, transform); // damage 값을 사용
                Debug.Log($"Bullet hit {collider.name} for {damage} damage.");
            }
        }

        // 애니메이션 길이에 따라 총알 파괴 지연
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        Invoke("DestroyBullet", animLength / 2);
    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}

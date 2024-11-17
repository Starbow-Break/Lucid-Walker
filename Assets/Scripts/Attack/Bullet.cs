using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Animator anim;
    public float speed;
    public float distance;
    public LayerMask isLayer;

    private bool hasHit = false; // 이미 명중했는지 여부 확인

    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("DestroyBullet", 2f); // 기본적으로 2초 후 파괴
    }

    void Update()
    {
        if (hasHit) return; // 명중한 경우 이동 중지

        RaycastHit2D ray = Physics2D.Raycast(transform.position, transform.right, distance, isLayer);
        if (ray.collider != null)
        {
            anim.SetTrigger("Hit"); // 명중 애니메이션 실행
            HandleHit(ray.collider); // 명중 처리
        }

        // 총알 이동
        float directionMultiplier = transform.rotation.y == 0 ? 1 : -1;
        transform.Translate(transform.right * directionMultiplier * speed * Time.deltaTime);
    }

    void HandleHit(Collider2D collider)
    {
        hasHit = true; // 명중 상태 설정

        // IDamageable 인터페이스가 구현된 대상에게 데미지 전달
        if (collider.CompareTag("Enemy"))
        {
            Debug.Log("명중!");
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1, transform); // 데미지 값 1
                Debug.Log($"Bullet hit {collider.name}");
            }
        }

        // 애니메이션이 끝난 후 Destroy 호출
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length; // 현재 애니메이션 길이
        Invoke("DestroyBullet", animLength / 2); // 애니메이션이 끝날 때까지 기다림
    }

    void DestroyBullet()
    {
        Destroy(gameObject); // 총알 오브젝트 삭제
    }
}

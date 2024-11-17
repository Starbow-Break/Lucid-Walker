using System.Collections;
using UnityEngine;

public class BlackObject : MonoBehaviour
{
    const int BLACKOBJ_DAMAGE = 1;
    [SerializeField] private float fadeOutSpeed = 1f; // 서서히 사라지는 속도
    [SerializeField] private LayerMask waterLayer;    // 물 레이어

    private SpriteRenderer spriteRenderer; // SpriteRenderer 컴포넌트
    private bool isFading = false; // 페이드 중인지 여부

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer가 없습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Collision Detected with: {other.gameObject.name}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        // LayerMask를 사용하여 물과의 충돌 감지
        if (((1 << other.gameObject.layer) & waterLayer) != 0 && !isFading)
        {
            Debug.Log("Water Layer detected. Starting fade out.");
            StartCoroutine(FadeOutAndDestroy());
        }

        // 플레이어와 충돌 시 데미지 처리
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(BLACKOBJ_DAMAGE, transform);
            }
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        Color color = spriteRenderer.color;

        // Alpha 값이 0이 될 때까지 점진적으로 감소
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime * fadeOutSpeed;
            spriteRenderer.color = color;
            yield return null;
        }

        // 완전히 투명해지면 오브젝트 삭제
        Destroy(gameObject);
    }
}

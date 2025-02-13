using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour, IDamageable
{
    // 기존의 maxHealth, currentHealth는 삭제 (공유 체력은 HealthManager가 관리)
    public bool isHurt;
    SpriteRenderer sr;
    Color halfA = new Color(1, 1, 1, 0.5f);
    Color fullA = new Color(1, 1, 1, 1);
    public float speed;
    private Animator anim;
    private Rigidbody2D rb;
    public bool isKnockBack = false;

    // 사망 시 개별적으로 실행할 애니메이션 등은 그대로 사용
    [SerializeField] private GameObject failUI;
    [SerializeField] private Animator failUIAnimator;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        // UI 초기화는 HealthManager에서 처리하므로 여기서는 별도 작업 불필요
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (!isHurt)
        {
            isHurt = true;
            // 공유 체력에 데미지를 적용 (두 캐릭터 모두 동일한 체력)
            HealthManager.Instance.TakeDamage(damage);

            if (HealthManager.Instance.currentHealth <= 0)
            {
                StartCoroutine(HandleDeath());
            }
            else
            {
                StartCoroutine(DamageRoutine(attacker));
            }
        }
    }

    IEnumerator DamageRoutine(Transform attacker)
    {
        // Knockback 처리
        isKnockBack = true;
        float knockbackDuration = 0.2f;
        float elapsedTime = 0f;

        anim.SetTrigger("hurt");
        while (elapsedTime < knockbackDuration)
        {
            // 예시: 간단한 넉백 (공격자 방향에 따라 실제 계산 수정 가능)
            transform.Translate(Vector2.left * 2 * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isKnockBack = false;

        // Alpha Blink 처리
        float blinkDuration = 2f;
        float blinkTime = 0f;
        while (blinkTime < blinkDuration)
        {
            sr.color = halfA;
            yield return new WaitForSeconds(0.1f);
            sr.color = fullA;
            yield return new WaitForSeconds(0.1f);
            blinkTime += 0.2f;
        }
        isHurt = false;
    }

    private IEnumerator HandleDeath()
    {
        // 사망 애니메이션 재생
        anim.SetTrigger("hurt");
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(1.5f);

        // Fail UI 활성화 (HealthManager에서도 사망 처리를 진행하므로 중복되지 않도록 주의)
        if (failUI != null)
        {
            failUI.SetActive(true);
            failUIAnimator.SetTrigger("Bounce");
            yield return StartCoroutine(FadeInFailUI());
        }
    }

    private IEnumerator FadeInFailUI()
    {
        float duration = 1f;
        float startAlpha = 0.5f;
        float endAlpha = 1f;
        float elapsedTime = 0f;

        Image failUIImage = failUI.GetComponent<Image>();
        Color color = failUIImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            failUIImage.color = color;
            yield return null;
        }
        color.a = endAlpha;
        failUIImage.color = color;
    }
}

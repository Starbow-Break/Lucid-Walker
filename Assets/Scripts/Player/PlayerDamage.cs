using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image를 사용하려면 추가

public class PlayerDamage : MonoBehaviour, IDamageable
{
    public int maxHealth = 3; // 최대 체력
    public int currentHealth; // 현재 체력
    public bool isHurt;
    SpriteRenderer sr;
    Color halfA = new Color(1, 1, 1, 0.5f);
    Color fullA = new Color(1, 1, 1, 1);
    public float speed;
    private Animator anim;     // Animator 참조
    private Rigidbody2D rb;    // Rigidbody2D 참조
    public bool isKnockBack = false;
    [SerializeField] private HealthUI healthUI; // HealthUI 참조
    [SerializeField] private GameObject failUI; // Fail UI
    [SerializeField] private Animator failUIAnimator; // FailUI의 Animator

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // 체력 초기화
        currentHealth = maxHealth;

        // UI 초기화
        healthUI.InitializeHealthUI(maxHealth);
        healthUI.UpdateHealthUI(currentHealth);

        failUI.SetActive(false); // Fail UI 비활성화
    }

    public void TakeDamage(int damage, Transform attacker)
    {
        if (!isHurt)
        {
            isHurt = true;
            currentHealth -= damage;

            healthUI.UpdateHealthUI(currentHealth);

            if (currentHealth <= 0)
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
        float elapsedTime = 0;

        anim.SetTrigger("hurt");
        while (elapsedTime < knockbackDuration)
        {
            transform.Translate(Vector2.left * 2 * Time.deltaTime); // Knockback 예제
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isKnockBack = false;

        // Alpha Blink 처리
        float blinkDuration = 2f;  // HurtRoutine에서 대기할 시간
        float blinkTime = 0;
        while (blinkTime < blinkDuration)
        {
            sr.color = halfA;
            yield return new WaitForSeconds(0.1f);
            sr.color = fullA;
            yield return new WaitForSeconds(0.1f);

            blinkTime += 0.2f; // 두 번의 대기 시간 더함
        }

        // HurtRoutine 종료 처리
        isHurt = false;
    }

    private IEnumerator HandleDeath()
    {
        // 사망 애니메이션
        anim.SetTrigger("hurt");
        anim.SetTrigger("Die");
        yield return new WaitForSeconds(1.5f);

        // Fail UI 활성화
        failUI.SetActive(true);
        failUIAnimator.SetTrigger("Bounce");

        // Fail UI 알파값 그라데이션
        yield return StartCoroutine(FadeInFailUI());

        yield return new WaitForSeconds(4f); // 2초 대기

        // Start 화면으로 전환
        LevelManager.Instance.LoadScene("Start", "CircleWipe");
    }

    private IEnumerator FadeInFailUI()
    {
        float duration = 1f; // 페이드 시간
        float startAlpha = 0.5f; // 시작 알파값
        float endAlpha = 1f; // 끝 알파값
        float elapsedTime = 0f;

        // GameObject에서 Image 컴포넌트 가져오기
        Image failUIImage = failUI.GetComponent<Image>();

        Color color = failUIImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            failUIImage.color = color;
            yield return null;
        }

        color.a = endAlpha; // 알파값 최종 설정
        failUIImage.color = color;
    }
}

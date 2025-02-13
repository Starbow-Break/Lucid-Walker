using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    public int maxHealth = 3;
    [HideInInspector]
    public int currentHealth;

    [SerializeField] private HealthUI healthUI;      // 하트 UI를 관리하는 스크립트
    [SerializeField] private GameObject failUI;        // 사망 시 표시할 UI
    [SerializeField] private Animator failUIAnimator;  // Fail UI의 Animator

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 유지하려면
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthUI.InitializeHealthUI(maxHealth);
        healthUI.UpdateHealthUI(currentHealth);
        if (failUI != null)
            failUI.SetActive(false);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHealthUI(currentHealth);

        if (currentHealth <= 0)
        {
            // 사망 처리 (필요시 추가 로직)
            Instance.StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(1.5f);

        if (failUI != null)
        {
            failUI.SetActive(true);
            failUIAnimator.SetTrigger("Bounce");
            yield return StartCoroutine(FadeInFailUI());
        }

        yield return new WaitForSeconds(4f);
        // 예시: 시작 씬으로 전환 (LevelManager에 따라 변경)
        LevelManager.Instance.LoadScene("Start", "CrossFade");
    }

    private IEnumerator FadeInFailUI()
    {
        float duration = 1f; // 페이드 시간
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

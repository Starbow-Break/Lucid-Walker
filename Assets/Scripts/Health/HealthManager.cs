using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    // maxHealth와 currentHealth를 직접 설정하지 않고, PlayerStats에서 가져옵니다.
    [HideInInspector]
    public int maxHealth;
    [HideInInspector]
    public int currentHealth;

    [SerializeField] private HealthUI healthUI;      // 하트 UI를 관리하는 스크립트
    [SerializeField] private GameObject failUI;        // 사망 시 표시할 UI
    [SerializeField] private Animator failUIAnimator;  // Fail UI의 Animator
    [SerializeField] private int stageNumber;


    private PlayerStats playerStats;

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
        // PlayerStats를 찾아서 maxHealth 값을 할당
        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            maxHealth = playerStats.MaxHearts;
        }
        else
        {
            Debug.LogWarning("PlayerStats를 찾을 수 없습니다. 기본값 3을 사용합니다.");
            maxHealth = 3;
        }

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
        var data = DataPersistenceManager.instance.GetCurrentGameData();

        // 에피소드 데이터 저장
        int currentEpisode = 1;
        foreach (var episode in data.episodesData)
        {
            if (episode.GetStageProgress(stageNumber) != null)
            {
                currentEpisode = episode.episodeNumber;
                break;
            }
        }

        data.lastPlayedEpisode = currentEpisode;

        var ep = data.GetEpisodeData(currentEpisode);
        var sp = ep.GetStageProgress(stageNumber);
        sp.isCleared = true;
        // sp.gotTreasure = true;

        // currentStage 업데이트도 가능
        if (ep.currentStage < stageNumber)
            ep.currentStage = stageNumber + 1;

        data.returnFromStage = true;
        Debug.Log($"✅ 저장 전 returnFromStage = {data.returnFromStage}");

        DataPersistenceManager.instance.SaveGame();
        PlayerPrefs.Save();
        yield return null;

        yield return new WaitForSeconds(3f);
        // 예시: 시작 씬으로 전환 (LevelManager에 따라 변경)
        LevelManager.Instance.LoadScene("Main", "CrossFade");
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

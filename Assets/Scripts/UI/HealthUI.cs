using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Data;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Transform heartsContainer; // 하트들이 배치될 부모 오브젝트
    [SerializeField] private GameObject heartPrefab; // 하트 Prefab
    [SerializeField] private Sprite heartFull; // 꽉 찬 하트 이미지
    [SerializeField] private Sprite heartEmpty; // 빈 하트 이미지
    [SerializeField] private GameObject treasureIconPrefab; // 보물 아이콘 Prefab
    [SerializeField] private Sprite inactiveTreasureIcon;
    [SerializeField] private Sprite activeTreasureIcon;


    private GameObject treasureIcon; // 보물 아이콘 참조

    private List<Animator> heartAnimators = new List<Animator>(); // 하트 Animator 리스트

    private List<Image> hearts = new List<Image>(); // 동적 하트 리스트
    private PlayerStats playerStats;

    Image treasureIconImage;

    private void Awake()
    {
        // 같은 씬에 PlayerStats가 붙어있다고 가정
        playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Start()
    {
        // DataPersistenceManager에서 LoadData()가 끝난 이후에
        // 아래처럼 InitializeHealthUI 호출 (씬 로드 순서에 따라 달라질 수 있음)
        InitializeHealthUI(playerStats.MaxHearts);
    }

    private void OnEnable()
    {
        StageManager.Instance.OnChangedTreasure += UpdateTreasureIcon;
    }

    private void OnDisable()
    {
        StageManager.Instance.OnChangedTreasure -= UpdateTreasureIcon;
    }

    // 초기 하트 UI 설정
    public void InitializeHealthUI(int maxHealth)
    {
        Debug.Log("InitializeHealthUI called with maxHealth: " + maxHealth);
        foreach (Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }
        hearts.Clear();
        heartAnimators.Clear();


        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            Image heartImage = heart.GetComponent<Image>();
            Animator heartAnimator = heart.GetComponent<Animator>();

            heartImage.sprite = heartFull; // 기본은 꽉 찬 하트
            hearts.Add(heartImage);
            heartAnimators.Add(heartAnimator);
        }
        AddTreasureIcon();
    }

    private void AddTreasureIcon()
    {
        int episode = StageManager.Instance.episode;
        int stage = StageManager.Instance.stage;
        GameData gameData = DataPersistenceManager.instance.GetCurrentGameData();
        var episodeData = gameData.GetEpisodeData(episode);
        var stageProgress = episodeData.GetStageProgress(stage);

        // 보물 아이콘 생성
        treasureIcon = Instantiate(treasureIconPrefab, heartsContainer);
        RectTransform rectTransform = treasureIcon.GetComponent<RectTransform>();

        treasureIconImage = treasureIcon.GetComponent<Image>();
        UpdateTreasureIcon(stageProgress.gotTreasure);
    }
    // 체력 UI 업데이트
    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i >= currentHealth)
            {
                Animator animator = heartAnimators[i];
                Image image = hearts[i];

                if (animator != null && animator.gameObject != null)
                {
                    animator.SetTrigger("Broke");
                    StartCoroutine(UpdateHeartImageAfterAnimation(animator, image));
                }
                else
                {
                    Debug.LogWarning($"⚠️ Heart Animator #{i}는 이미 Destroy된 상태입니다.");
                }
            }
        }
    }

    private IEnumerator UpdateHeartImageAfterAnimation(Animator animator, Image heartImage)
    {
        if (animator == null || animator.gameObject == null) yield break;

        // 애니메이션 상태가 끝날 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Heart_Idle"))
        {
            if (animator == null || animator.gameObject == null) yield break;
            yield return null;
        }

        // 깨진 하트 이미지로 변경
        if (heartImage != null)  // 여기도 방어적 체크
            heartImage.sprite = heartEmpty;
    }

    public void UpdateTreasureIcon(bool value)
    {
        treasureIconImage.sprite = value ? activeTreasureIcon : inactiveTreasureIcon;
    }
}

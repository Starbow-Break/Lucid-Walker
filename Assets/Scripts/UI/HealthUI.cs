using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Transform heartsContainer; // 하트들이 배치될 부모 오브젝트
    [SerializeField] private GameObject heartPrefab; // 하트 Prefab
    [SerializeField] private Sprite heartFull; // 꽉 찬 하트 이미지
    [SerializeField] private Sprite heartEmpty; // 빈 하트 이미지
    [SerializeField] private GameObject treasureIconPrefab; // 보물 아이콘 Prefab


    private GameObject treasureIcon; // 보물 아이콘 참조

    private List<Animator> heartAnimators = new List<Animator>(); // 하트 Animator 리스트

    private List<Image> hearts = new List<Image>(); // 동적 하트 리스트
    private PlayerStats playerStats;

    private void Awake()
    {
        // 같은 씬에 PlayerStats가 붙어있다고 가정
        playerStats = FindObjectOfType<PlayerStats>();
    }

    private void Start()
    {
        // DataPersistenceManager에서 LoadData()가 끝난 이후에
        // 아래처럼 InitializeHealthUI 호출 (씬 로드 순서에 따라 달라질 수 있음)
        InitializeHealthUI(playerStats.GetMaxHearts());
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

        // 보물 아이콘 생성
        treasureIcon = Instantiate(treasureIconPrefab, heartsContainer);
        RectTransform rectTransform = treasureIcon.GetComponent<RectTransform>();
    }
    // 체력 UI 업데이트
    public void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i >= currentHealth)
            {
                // 깨지는 애니메이션 트리거
                heartAnimators[i].SetTrigger("Broke");
                // 애니메이션이 끝나면 빈 하트 이미지로 변경
                StartCoroutine(UpdateHeartImageAfterAnimation(heartAnimators[i], hearts[i]));
            }
        }
    }
    private IEnumerator UpdateHeartImageAfterAnimation(Animator animator, Image heartImage)
    {
        // 애니메이션 상태가 끝날 때까지 대기
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Heart_Idle"))
        {
            yield return null;
        }

        // 깨진 하트 이미지로 변경
        heartImage.sprite = heartEmpty;
    }
}

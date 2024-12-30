using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EpisodeSelector : MonoBehaviour
{
    [System.Serializable]
    public class Episode
    {
        public Sprite episodeNameImage; // 에피소드 이름 (이미지)
        public Sprite icon;             // 에피소드 아이콘
        public Sprite characterFace;    // 주인공 얼굴 이미지
        public string description;      // 에피소드 설명
    }

    public Episode[] episodes;          // 에피소드 데이터 배열
    public Image episodeNameImage;      // 에피소드 이름을 표시할 UI 이미지
    public Image episodeIcon;           // 에피소드 아이콘
    public Image characterFaceImage;    // 주인공 얼굴을 표시할 UI 이미지
    public TextMeshProUGUI episodeDescriptionText; // 에피소드 설명
    public int currentEpisodeIndex = 0; // 현재 에피소드 인덱스

    // 버튼 클릭 시 호출
    public void NextEpisode()
    {
        currentEpisodeIndex++;
        if (currentEpisodeIndex >= episodes.Length)
            currentEpisodeIndex = 0; // 마지막 에피소드 다음은 첫 번째 에피소드
        UpdateUI();
    }

    public void PreviousEpisode()
    {
        currentEpisodeIndex--;
        if (currentEpisodeIndex < 0)
            currentEpisodeIndex = episodes.Length - 1; // 첫 번째 에피소드 이전은 마지막 에피소드
        UpdateUI();
    }

    private void UpdateUI()
    {
        // UI 요소 업데이트
        episodeNameImage.sprite = episodes[currentEpisodeIndex].episodeNameImage; // 에피소드 이름 이미지
        episodeIcon.sprite = episodes[currentEpisodeIndex].icon;                  // 에피소드 아이콘
        characterFaceImage.sprite = episodes[currentEpisodeIndex].characterFace;  // 주인공 얼굴 이미지
        episodeDescriptionText.text = episodes[currentEpisodeIndex].description;  // 에피소드 설명
    }

    private void Start()
    {
        UpdateUI(); // 초기 UI 설정
    }
}

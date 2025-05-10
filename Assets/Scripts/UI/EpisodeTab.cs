using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EpisodeTab : MonoBehaviour
{

    [Header("Footer / 상단 텍스트")]
    [SerializeField] private Image footerCharacter;
    [SerializeField] private List<Sprite> footerSprites;
    [SerializeField] private TextMeshProUGUI episodeText;

    [Header("탭 스프라이트")]
    [SerializeField] private Image tab1Image;
    [SerializeField] private Image tab2Image;
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite unselectedSprite;

    [Header("Reward UI")]
    [SerializeField] private EpisodeRewardUI ep1RewardUI;
    [SerializeField] private EpisodeRewardUI ep2RewardUI;

    private int selectedEpisode = 1;

    private void Start()
    {
        SelectEpisode(1); // 시작은 EP1
    }

    public void SelectEpisode(int episode)
    {
        selectedEpisode = episode;

        // 탭 스프라이트 변경
        tab1Image.sprite = (episode == 1) ? selectedSprite : unselectedSprite;
        tab2Image.sprite = (episode == 2) ? selectedSprite : unselectedSprite;

        // 상단 텍스트 갱신
        episodeText.text = $"EP {episode}";

        // Footer 이미지 교체
        footerCharacter.sprite = footerSprites[episode - 1];

        // Reward UI Init
        var gameData = DataPersistenceManager.instance.GetCurrentGameData();
        var epData = gameData.GetEpisodeData(episode);

        bool allCollected = epData.stageProgresses.FindAll(p => p.stageNumber <= 6).TrueForAll(p => p.gotTreasure);
        bool alreadyClaimed = gameData.IsEpisodeRewardClaimed(episode);

        if (episode == 1)
            ep1RewardUI.Init(1, allCollected, alreadyClaimed);
        else
            ep2RewardUI.Init(2, allCollected, alreadyClaimed);
    }
}

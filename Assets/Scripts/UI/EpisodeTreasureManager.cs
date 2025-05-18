using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct StageTreasureEntry
{
    public int stageNumber;
    public Image slotImage;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
}

public class EpisodeTreasureManager : MonoBehaviour
{
    public int episode;  // 해당 에피소드 번호
    public List<StageTreasureEntry> treasureEntries;

    [Header("Reward UI")]
    [SerializeField] private GameObject rewardStamp; // 인장
    [SerializeField] private Button claimRewardButton;
    [SerializeField] private GameObject rewardEffect;
    [SerializeField] private Image claimButtonImage;
    [SerializeField] private Sprite activeButtonSprite;   // 색 있는 버튼
    [SerializeField] private Sprite inactiveButtonSprite; // 회색 버튼

    private Dictionary<int, StageTreasureEntry> _treasureMap;

    private void Awake()
    {
        _treasureMap = new Dictionary<int, StageTreasureEntry>();
        foreach (var entry in treasureEntries)
        {
            if (_treasureMap.ContainsKey(entry.stageNumber))
            {
                Debug.LogWarning($"중복된 Treasure Entry: Stage {entry.stageNumber}");
                continue;
            }
            _treasureMap.Add(entry.stageNumber, entry);
        }

        if (claimRewardButton != null)
            claimRewardButton.onClick.AddListener(ClaimReward);
    }

    public void ApplyTreasureState(EpisodeData episodeData)
    {
        foreach (var kvp in _treasureMap)
        {
            int stage = kvp.Key;
            var entry = kvp.Value;

            var progress = episodeData.GetStageProgress(stage);
            entry.slotImage.sprite = progress.gotTreasure ? entry.inactiveSprite : entry.activeSprite;
        }

        bool allCollected = episodeData.stageProgresses.FindAll(p => p.stageNumber <= 6).TrueForAll(p => p.gotTreasure);
        bool alreadyClaimed = DataPersistenceManager.instance.GetCurrentGameData().IsEpisodeRewardClaimed(episode);

        // 인장 표시
        if (rewardStamp != null) rewardStamp.SetActive(allCollected && alreadyClaimed);

        // 버튼 상태 제어
        if (claimRewardButton != null)
        {
            bool canClaim = allCollected && !alreadyClaimed;
            claimRewardButton.interactable = canClaim;
            claimRewardButton.gameObject.SetActive(true); // 항상 보이되, interactable로 조절

            if (claimButtonImage != null)
                claimButtonImage.sprite = canClaim ? activeButtonSprite : inactiveButtonSprite;

            if (rewardEffect != null)
                rewardEffect.SetActive(canClaim); // 반짝이 효과 ON/OFF
        }
    }

    private void ClaimReward()
    {
        if (!claimRewardButton.interactable) return; // 못 누르는 상태면 무시

        var data = DataPersistenceManager.instance.GetCurrentGameData();
        data.MarkEpisodeRewardClaimed(episode);
        Debug.Log($"EP{episode} 보상 수령 완료!");

        if (rewardStamp != null) rewardStamp.SetActive(true);
        if (claimRewardButton != null)
        {
            claimRewardButton.interactable = false;
            claimRewardButton.gameObject.SetActive(false); // 혹은 남겨두되 비활성화
        }

        if (rewardEffect != null)
            rewardEffect.SetActive(false); // 효과 종료
    }

}
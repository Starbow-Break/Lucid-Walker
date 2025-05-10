using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EpisodeRewardUI : MonoBehaviour
{
    [SerializeField] private GameObject rewardStamp; // 인장 색칠됨
    [SerializeField] private Button claimRewardButton;

    private int episode;
    private bool isClaimed;

    public void Init(int episode, bool allCollected, bool alreadyClaimed)
    {
        this.episode = episode;
        isClaimed = alreadyClaimed;

        rewardStamp.SetActive(allCollected && alreadyClaimed);
        claimRewardButton.gameObject.SetActive(allCollected && !alreadyClaimed);

        claimRewardButton.onClick.RemoveAllListeners();
        claimRewardButton.onClick.AddListener(ClaimReward);
    }

    private void ClaimReward()
    {
        Debug.Log($"EP{episode} 보상 수령!");

        var data = DataPersistenceManager.instance.GetCurrentGameData();
        data.MarkEpisodeRewardClaimed(episode);

        rewardStamp.SetActive(true);
    }
}

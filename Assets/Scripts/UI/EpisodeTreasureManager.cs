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
    }

    public void ApplyTreasureState(EpisodeData episodeData)
    {
        foreach (var kvp in _treasureMap)
        {
            int stage = kvp.Key;
            var entry = kvp.Value;

            var progress = episodeData.GetStageProgress(stage);
            entry.slotImage.sprite = progress.gotTreasure ? entry.activeSprite : entry.inactiveSprite;
        }
    }
}
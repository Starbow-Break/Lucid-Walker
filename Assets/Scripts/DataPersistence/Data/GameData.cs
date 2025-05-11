using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialData
{
    // 튜토리얼 진행 여부 (예: 완료 여부)
    public bool isCompleted;

    public TutorialData()
    {
        isCompleted = false;
    }
}

public enum CartoonSceneTriggerTime
{
    None,
    BeforeStage,
    AfterStage
}

[System.Serializable]
public class StageProgress
{
    public int stageNumber;
    public bool isCleared;
    public bool gotTreasure;

    public bool hasCartoonScene;
    public CartoonSceneTriggerTime cartoonSceneTriggerTime;

    public bool cartoonScenePlayed;

    public StageProgress(int stageNumber, CartoonSceneTriggerTime cartoonSceneTriggerTime = CartoonSceneTriggerTime.None)
    {
        this.stageNumber = stageNumber;
        this.isCleared = false;
        this.gotTreasure = false;

        this.cartoonSceneTriggerTime = cartoonSceneTriggerTime;
        this.hasCartoonScene = cartoonSceneTriggerTime != CartoonSceneTriggerTime.None;
        this.cartoonScenePlayed = false;
    }
}

[System.Serializable]
public class EpisodeData
{
    // 에피소드 번호 (1, 2, 3, ...)
    public int episodeNumber;
    // 현재 진행중인 스테이지 번호 (1,2,3,..., 보스는 마지막 번호)
    public int currentStage;
    // 해당 에피소드의 총 스테이지 수 (튜토리얼은 별도이므로 에피소드 1은 스테이지1~6와 보스로 총 7개)
    public int totalStages;
    public List<StageProgress> stageProgresses;


    public EpisodeData(int episodeNumber, int totalStages)
    {
        this.episodeNumber = episodeNumber;
        this.totalStages = totalStages;
        // 에피소드 시작 시, 첫 스테이지로 시작
        this.currentStage = 1;

        stageProgresses = new List<StageProgress>();
        for (int i = 1; i <= totalStages; i++)
        {
            CartoonSceneTriggerTime timing = CartoonSceneTriggerTime.None;

            if (i == 1 || i == 3 || i == 5)
                timing = CartoonSceneTriggerTime.BeforeStage;
            else if (i == 7)
                timing = CartoonSceneTriggerTime.AfterStage;

            stageProgresses.Add(new StageProgress(i, timing));
        }

    }
    public StageProgress GetStageProgress(int stageNumber)
    {
        return stageProgresses.Find(sp => sp.stageNumber == stageNumber);
    }
}


[System.Serializable]
public class CutscenePlayRecord
{
    public int episodeNumber;
    public bool hasPlayed;
}

[System.Serializable]
public class GameData
{
    // 튜토리얼 진행 데이터
    public TutorialData tutorialData;
    public List<CutscenePlayRecord> cutscenePlayRecords;
    // 에피소드 진행 데이터 
    public List<EpisodeData> episodesData;
    public int lastPlayedEpisode = 1;
    public bool returnFromStage = false;
    public bool stageFailed = false;

    // 플레이어의 현재 하트 개수 (스탯 기본 하트 :3)
    public int heartCount;
    public int gold;

    // 구매된 업그레이드의 ID를 저장하는 리스트
    public List<string> purchasedUpgradeIDs;

    public float maxEnergy;
    public float currentEnergy;
    public float energyRegenRate;
    public float energyDrainRate;
    public float attackDamage;
    public int luck;

    // 보물 스탬프 리워드
    public List<int> claimedEpisodeRewards = new List<int>(); // 수령된 보상 에피소드

    public GameData()
    {
        // 튜토리얼 데이터 초기화
        tutorialData = new TutorialData();

        // 기본 에피소드 1 추가 (튜토리얼과는 별도, 에피소드 1은 스테이지 1~6와 보스로 총 7단계라고 가정)
        episodesData = new List<EpisodeData> { new EpisodeData(1, 7) };
        cutscenePlayRecords = new List<CutscenePlayRecord>();

        // 기본 하트 개수 설정 (예: 3)
        heartCount = 3;
        gold = 100;

        // 리스트 초기화
        purchasedUpgradeIDs = new List<string>();
        maxEnergy = 100f;
        currentEnergy = 100f;
        energyRegenRate = 5f;
        energyDrainRate = 10f;

        attackDamage = 10f;  // 기본 공격력
        luck = 0;            // 기본 행운 
    }

    // 특정 에피소드 데이터를 반환 (없는 경우 null)
    public EpisodeData GetEpisodeData(int episodeNumber)
    {
        return episodesData.Find(ep => ep.episodeNumber == episodeNumber);
    }

    // 새로운 에피소드 추가 (이미 존재하면 추가하지 않음)
    public void AddEpisodeData(int episodeNumber, int totalStages)
    {
        if (GetEpisodeData(episodeNumber) == null)
        {
            episodesData.Add(new EpisodeData(episodeNumber, totalStages));
        }
    }

    public bool HasCutscenePlayed(int episodeNumber)
    {
        return cutscenePlayRecords.Exists(e => e.episodeNumber == episodeNumber && e.hasPlayed);
    }

    public void SetCutscenePlayed(int episodeNumber)
    {
        var record = cutscenePlayRecords.Find(e => e.episodeNumber == episodeNumber);
        if (record != null)
        {
            record.hasPlayed = true;
        }
        else
        {
            cutscenePlayRecords.Add(new CutscenePlayRecord
            {
                episodeNumber = episodeNumber,
                hasPlayed = true
            });
        }
    }

    public bool IsEpisodeRewardClaimed(int episodeNumber)
    {
        return claimedEpisodeRewards.Contains(episodeNumber);
    }

    public void MarkEpisodeRewardClaimed(int episodeNumber)
    {
        if (!claimedEpisodeRewards.Contains(episodeNumber))
        {
            claimedEpisodeRewards.Add(episodeNumber);
        }
    }
}

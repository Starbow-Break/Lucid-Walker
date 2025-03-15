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

[System.Serializable]
public class EpisodeData
{
    // 에피소드 번호 (1, 2, 3, ...)
    public int episodeNumber;
    // 현재 진행중인 스테이지 번호 (1,2,3,..., 보스는 마지막 번호)
    public int currentStage;
    // 해당 에피소드의 총 스테이지 수 (튜토리얼은 별도이므로 에피소드 1은 스테이지1~6와 보스로 총 7개)
    public int totalStages;

    public EpisodeData(int episodeNumber, int totalStages)
    {
        this.episodeNumber = episodeNumber;
        this.totalStages = totalStages;
        // 에피소드 시작 시, 첫 스테이지로 시작
        this.currentStage = 1;
    }
}

[System.Serializable]
public class GameData
{
    // 튜토리얼 진행 데이터
    public TutorialData tutorialData;
    // 에피소드 진행 데이터 
    public List<EpisodeData> episodesData;
    // 플레이어의 현재 하트 개수 (스탯 기본 하트 :3)
    public int heartCount;
    public int gold;

    // 구매된 업그레이드의 ID를 저장하는 리스트
    public List<string> purchasedUpgradeIDs;

    public GameData()
    {
        // 튜토리얼 데이터 초기화
        tutorialData = new TutorialData();

        // 에피소드 데이터 리스트 초기화
        episodesData = new List<EpisodeData>();
        // 기본 에피소드 1 추가 (튜토리얼과는 별도, 에피소드 1은 스테이지 1~6와 보스로 총 7단계라고 가정)
        episodesData.Add(new EpisodeData(1, 7));

        // 기본 하트 개수 설정 (예: 3)
        heartCount = 3;
        gold = 100;

        // 리스트 초기화
        purchasedUpgradeIDs = new List<string>();
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
}

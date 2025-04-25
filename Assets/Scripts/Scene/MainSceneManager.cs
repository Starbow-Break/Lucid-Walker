using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform chairPosition;
    [SerializeField] private MainChairconnect chairSystem;
    [SerializeField] private EpisodeSelector episodeSelector;
    void OnEnable()
    {
        var data = DataPersistenceManager.instance.GetCurrentGameData();

        if (data.returnFromStage)
        {
            // 플레이어 위치를 앉은 상태로 Teleport
            player.transform.position = chairPosition.position;
            player.SetActive(false);

            // 2.의자 자동 연출 실행
            chairSystem.StartAutoSequence();

            if (episodeSelector != null)
            {
                episodeSelector.currentEpisodeIndex = data.lastPlayedEpisode - 1;
                episodeSelector.SendMessage("UpdateUI"); // UpdateUI() 호출
            }

            // data.returnFromStage = false;
            DataPersistenceManager.instance.SaveGame();
            Debug.Log($"▶ mainscenemanager() 호출됨, player active: {player.activeSelf}");

        }
    }

}

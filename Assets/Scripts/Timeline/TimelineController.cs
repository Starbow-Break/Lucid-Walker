using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class TimelineController : MonoBehaviour
{
    public PlayableDirector ep1Timeline;
    public GameObject ep1TimelineDummyPlayer;
    public GameObject actualPlayer;
    public int currentEpisode = 1;

    private void Start()
    {
        var data = DataPersistenceManager.instance.GetCurrentGameData();

        if (!data.HasCutscenePlayed(currentEpisode))
        {
            // 컷씬 재생
            ep1Timeline.gameObject.SetActive(true);
            ep1TimelineDummyPlayer.SetActive(true);
            actualPlayer.SetActive(false);

            ep1Timeline.Play();
        }
        else
        {
            // 컷씬 스킵
            ep1Timeline.gameObject.SetActive(false);
            ep1TimelineDummyPlayer.SetActive(false);
            actualPlayer.SetActive(true);
        }
    }

    // Timeline 끝에서 SignalEmitter로 호출될 메서드
    public void OnEp1CutsceneEnd()
    {
        var data = DataPersistenceManager.instance.GetCurrentGameData();
        data.SetCutscenePlayed(currentEpisode);
        DataPersistenceManager.instance.SaveGame();

        ep1TimelineDummyPlayer.SetActive(false);
        actualPlayer.SetActive(true);

        Debug.Log("✅ Ep1 컷씬 종료 → 실제 플레이 시작");
    }
}

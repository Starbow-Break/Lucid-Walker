using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;



public class TimelineController : MonoBehaviour, IDataPersistence
{
    public PlayableDirector ep1Timeline;
    public GameObject ep1TimelineDummyPlayer;
    public GameObject actualPlayer;
    public int currentEpisode = 1;

    private bool shouldPlayCutscene = false;
    public PolygonCollider2D mainConfinerShape; // 거실 confiner 바운드
    public CinemachineVirtualCamera mainCamera; // timeline이랑 동일한 카메라일 수도 있음
    public GameObject knockSprite;
    public GameObject womancharacterDome;
    public GameObject womancharacterSprite;
    public GameObject dome;


    public void LoadData(GameData data)
    {
        shouldPlayCutscene = !data.HasCutscenePlayed(currentEpisode);
    }

    public void SaveData(ref GameData data)
    {
        // 아무것도 안 해도 됨
    }

    private void Start()
    {
        if (shouldPlayCutscene)
        {
            ep1Timeline.gameObject.SetActive(true);
            ep1TimelineDummyPlayer.SetActive(true);
            actualPlayer.SetActive(false);
            ep1Timeline.Play();
        }
        else
        {
            ep1Timeline.gameObject.SetActive(false);
            ep1TimelineDummyPlayer.SetActive(false);
            actualPlayer.SetActive(true);

            OnEp1CutsceneEnd();
        }
    }

    public void OnEp1CutsceneEnd()
    {
        var data = DataPersistenceManager.instance.GetCurrentGameData();
        data.SetCutscenePlayed(currentEpisode);
        DataPersistenceManager.instance.SaveGame();

        ep1TimelineDummyPlayer.SetActive(false);
        actualPlayer.SetActive(true);
        actualPlayer.GetComponent<PlayerController>().CheckDirectionToFace(true);
        if (mainCamera != null && mainConfinerShape != null)
        {
            var confiner = mainCamera.GetComponent<CinemachineConfiner2D>();
            confiner.m_BoundingShape2D = mainConfinerShape;
            confiner.InvalidateCache();
        }
        knockSprite.SetActive(false);
        womancharacterDome.SetActive(true);
        womancharacterSprite.SetActive(false);
        dome.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
struct SequentialSpotlightData {
    public Spotlight spotlight; // 스포트라이트
    public List<int> lightOnTime; // 켜지는 시간
}

public class SequentialSpotlightGroup : MonoBehaviour
{
    [SerializeField] int period; // 주기
    [SerializeField] float timePerUnit; // 단위당 시간
    [SerializeField] List<SequentialSpotlightData> spotlightsData;

    public bool isOn = true;

    int curTimeUnit = -1; // 현재 시간
    Dictionary<Spotlight, HashSet<int>> spotlightsDict; // 각 시간마다 목표 스포트라이트
    Coroutine coroutine = null;

    void Start()
    {
        InitializeTargetSpotlight();
        if(isOn) {
            coroutine = StartCoroutine(SequentialSpotlightFlow());
        }
    }

    // 초기화
    void ResetGroup()
    {
        curTimeUnit = -1; // 시간 초기화

        // 모든 스포트라이트를 끈다.
        foreach(SequentialSpotlightData ssd in spotlightsData) {
            ssd.spotlight.TurnOff();
        }
    }

    void InitializeTargetSpotlight()
    {
        spotlightsDict = new Dictionary<Spotlight, HashSet<int>>();

        foreach(SequentialSpotlightData ssd in spotlightsData) {
            ssd.spotlight.TurnOff();
            spotlightsDict.Add(ssd.spotlight, new HashSet<int>());
            foreach(int time in ssd.lightOnTime) {
                spotlightsDict[ssd.spotlight].Add(time);
            }
        }

        ResetGroup();
    }

    IEnumerator SequentialSpotlightFlow()
    {
        while(true) {
            // 모든 스포트라이트 상태 갱신
            curTimeUnit = (curTimeUnit + 1) % period;
            foreach(SequentialSpotlightData ssd in spotlightsData) {
                if (spotlightsDict[ssd.spotlight].Contains(curTimeUnit)) {
                    ssd.spotlight.TurnOn();
                }
                else {
                    ssd.spotlight.TurnOff();
                }
            }

            yield return new WaitForSeconds(timePerUnit);
        }
    }
    
    void TurnOff()
    {
        isOn = false;
        if(coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        ResetGroup();
    }

    void TurnOn()
    {
        isOn = true;
        coroutine ??= StartCoroutine(SequentialSpotlightFlow());
    }

    public void Switch()
    {
        if(isOn) TurnOff(); else TurnOn();
    }
}

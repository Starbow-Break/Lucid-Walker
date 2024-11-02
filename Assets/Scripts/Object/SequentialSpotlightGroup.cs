using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    int curTimeUnit = -1; // 현재 시간
    Dictionary<int, List<Spotlight>> targetSpotlight; // 각 시간마다 목표 스포트라이트

    void Start()
    {
        InitializeTargetSpotlight();
        StartCoroutine(SequentialSpotlightFlow());
    }

    void InitializeTargetSpotlight()
    {
        targetSpotlight = new Dictionary<int, List<Spotlight>>();

        foreach(SequentialSpotlightData ssd in spotlightsData) {
            foreach(int t in ssd.lightOnTime) {
                if(!targetSpotlight.ContainsKey(t)) {
                    targetSpotlight.Add(t, new List<Spotlight>());
                }

                targetSpotlight[t].Add(ssd.spotlight);
            }
        }
    }

    IEnumerator SequentialSpotlightFlow()
    {
        while(true) {
            // 기존 시간대에 켜진 스포트라이트들을 전부 끈다.
            if(targetSpotlight.ContainsKey(curTimeUnit)) {
                foreach(Spotlight sl in targetSpotlight[curTimeUnit]) {
                    sl.TurnOff();
                }
            }

            // 다음 시간에 켜져야 하는 모든 스포트라이트를 켠다.
            curTimeUnit = (curTimeUnit + 1) % period;
            if(targetSpotlight.ContainsKey(curTimeUnit)) {
                foreach(Spotlight sl in targetSpotlight[curTimeUnit]) {
                    sl.TurnOn();
                }
            }

            yield return new WaitForSeconds(timePerUnit);
        }
    }
}

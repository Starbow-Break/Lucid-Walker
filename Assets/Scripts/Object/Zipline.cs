using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] Transform departures; // 출발 지점
    [SerializeField] Transform arrivals; // 도착 지점
    
    [SerializeField] LineRenderer line; // 줄
    [SerializeField] GameObject trolley; // 플레이어가 잡는 부분

    void OnValidate()
    {
        // 출발지 도착지에 맞춰서 줄 그리기
        line.positionCount = 2;
        line.startWidth = line.endWidth = 0.05f;
        line.SetPosition(0, departures.position);
        line.SetPosition(1, arrivals.position);
        line.useWorldSpace = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 출발지 도착지에 맞춰서 줄 그리기
        line.positionCount = 2;
        line.startWidth = line.endWidth = 0.05f;
        line.SetPosition(0, departures.position);
        line.SetPosition(1, arrivals.position);
        line.useWorldSpace = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

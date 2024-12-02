using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingStar : MonoBehaviour
{
    [SerializeField] GameObject star; // 별
    [SerializeField] LineRenderer routeLine; // 경로
    [Min(0.0f), SerializeField] float starSpeed = 1.0f; // 별 이동속도
    [SerializeField] bool backAndForth = false; // 왔다 갔다 여부

    int dir; // 이동 방향 (i -> i + delta)
    int routeLineVertexNum; // 경로 정점 갯수
    int starPositionIndex; // 별의 위치 값

    List<float> sumDist; // sumDist[i] = 0번 ~ i번 까지의 거리

    private void OnValidate() {
        star.transform.localPosition = routeLine.GetPosition(0);
    }

    void Awake() {
        dir = 1;
        PreProcessing();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 별을 처음 위치로 옮겨준다.
        star.transform.localPosition = routeLine.GetPosition(starPositionIndex);
    }

    void Update() {
        MoveStar();
    }

    void MoveStar() {
        if(backAndForth) {
            float dist = (Time.deltaTime * starSpeed) % (2.0f * sumDist[^1]); // 이동 거리
            float nextPosValue = sumDist[starPositionIndex]; // 다음 위치 값
            nextPosValue += (star.transform.localPosition 
                - routeLine.GetPosition(starPositionIndex)).magnitude;
            nextPosValue = (2 * sumDist[^1] + dir * nextPosValue + dist) % (2.0f * sumDist[^1]);

            dir = nextPosValue >= sumDist[^1] ? -1 : 1;

            float target = dir == 1 ? nextPosValue % sumDist[^1] : sumDist[^1] - nextPosValue % sumDist[^1];
            int nextIndex = sumDist.BinarySearch(target);
            starPositionIndex = nextIndex < 0 ? ~nextIndex - 1 : nextIndex;

            float remain = target - sumDist[starPositionIndex];
            star.transform.localPosition = routeLine.GetPosition(starPositionIndex);
            star.transform.localPosition += 
                remain *
                (routeLine.GetPosition(starPositionIndex + 1)
                    - routeLine.GetPosition(starPositionIndex)).normalized;
        }
        else {
            float dist = (Time.deltaTime * starSpeed) % sumDist[^1]; // 이동 거리
            float nextPosValue = sumDist[starPositionIndex] + sumDist[^1] + dir * dist; // 다음 위치 값
            nextPosValue += (star.transform.localPosition 
                - routeLine.GetPosition(starPositionIndex)).magnitude;
            nextPosValue %= sumDist[^1];

            int nextIndex = sumDist.BinarySearch(nextPosValue);
            starPositionIndex = nextIndex < 0 ? ~nextIndex - 1 : nextIndex;
            starPositionIndex = starPositionIndex == sumDist.Count - 1 ? 0 : starPositionIndex;

            float remain = nextPosValue - sumDist[starPositionIndex];
            star.transform.localPosition = routeLine.GetPosition(starPositionIndex);
            star.transform.localPosition += 
                remain *
                (routeLine.GetPosition(starPositionIndex + 1)
                    - routeLine.GetPosition(starPositionIndex)).normalized;
        }
    }

    // 필요한 데이터들을 전처리
    void PreProcessing() {
        routeLineVertexNum = routeLine.positionCount;
        starPositionIndex = 0;

        sumDist = new List<float>(){0};

        for(int i = 1; i < routeLine.positionCount; i++) {
            Vector3 pos1 = routeLine.GetPosition(i);
            Vector3 pos2 = routeLine.GetPosition(i-1);

            sumDist.Add((pos1 - pos2).magnitude);
            sumDist[i] += sumDist[i-1];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 경로를 따라 움직이는 오브젝트
public class MovingObjectAlongPath : MonoBehaviour
{
    [SerializeField] GameObject movingObject; // 경로따라 움직이는 오브젝트
    [SerializeField] LineRenderer pathLine; // 경로
    [Min(0.0f), SerializeField] float objectSpeed = 1.0f; // 별 이동속도
    [SerializeField] bool backAndForth = false; // 왔다 갔다 여부

    int dir; // 이동 방향 (i -> i + delta)
    int pathLineVertexNum; // 경로 정점 갯수
    int positionIndex; // 위치 값

    List<float> sumDist; // sumDist[i] = 0번 ~ i번 까지의 거리

    private void OnValidate() {
     movingObject.transform.localPosition = pathLine.GetPosition(0);
    }

    void Awake() {
        dir = 1;
        PreProcessing();
    }

    //Start is called before the first frame update
    void Start()
    {
        // 별을 처음 위치로 옮겨준다.
        movingObject.transform.localPosition = pathLine.GetPosition(positionIndex);
    }

    void Update() {
        MoveObject();
    }

    void MoveObject() {
        if(backAndForth) {
            float dist = (Time.deltaTime * objectSpeed) % (2.0f * sumDist[^1]); // 이동 거리
            float nextPosValue = sumDist[positionIndex]; // 다음 위치 값
            nextPosValue += (movingObject.transform.localPosition 
                - pathLine.GetPosition(positionIndex)).magnitude;
            nextPosValue = (2 * sumDist[^1] + dir * nextPosValue + dist) % (2.0f * sumDist[^1]);

            dir = nextPosValue >= sumDist[^1] ? -1 : 1;

            float target = dir == 1 ? nextPosValue % sumDist[^1] : sumDist[^1] - nextPosValue % sumDist[^1];
            int nextIndex = sumDist.BinarySearch(target);
            positionIndex = nextIndex < 0 ? ~nextIndex - 1 : nextIndex;

            float remain = target - sumDist[positionIndex];
            movingObject.transform.localPosition = pathLine.GetPosition(positionIndex);
            movingObject.transform.localPosition += 
                remain *
                (pathLine.GetPosition(positionIndex + 1)
                    - pathLine.GetPosition(positionIndex)).normalized;
        }
        else {
            float dist = (Time.deltaTime * objectSpeed) % sumDist[^1]; // 이동 거리
            float nextPosValue = sumDist[positionIndex] + sumDist[^1] + dir * dist; // 다음 위치 값
            nextPosValue += (movingObject.transform.localPosition 
                - pathLine.GetPosition(positionIndex)).magnitude;
            nextPosValue %= sumDist[^1];

            int nextIndex = sumDist.BinarySearch(nextPosValue);
            positionIndex = nextIndex < 0 ? ~nextIndex - 1 : nextIndex;
            positionIndex = positionIndex == sumDist.Count - 1 ? 0 : positionIndex;

            float remain = nextPosValue - sumDist[positionIndex];
            movingObject.transform.localPosition = pathLine.GetPosition(positionIndex);
            movingObject.transform.localPosition += 
                remain *
                (pathLine.GetPosition(positionIndex + 1)
                    - pathLine.GetPosition(positionIndex)).normalized;
        }
    }

    // 필요한 데이터들을 전처리
    void PreProcessing() {
        pathLineVertexNum = pathLine.positionCount;
        positionIndex = 0;

        sumDist = new List<float>(){0};

        for(int i = 1; i < pathLine.positionCount; i++) {
            Vector3 pos1 = pathLine.GetPosition(i);
            Vector3 pos2 = pathLine.GetPosition(i-1);

            sumDist.Add((pos1 - pos2).magnitude);
            sumDist[i] += sumDist[i-1];
        }
    }
}

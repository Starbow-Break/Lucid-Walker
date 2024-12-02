using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LineGenerator : MonoBehaviour
{
    [SerializeField] protected float width = 0.07f; // 선 두께
    [SerializeField] protected LineRenderer line; // Line Renderer
    [SerializeField] protected GameObject startPoint; // 시작점
    [SerializeField] protected GameObject endPoint; // 끝 점

    protected const int REPEAT = 3; // 정점 반복 횟수
    protected const int CORNER_VERTICES = 5; // 코너의 정점 수

    
    void OnValidate() { width = 0.07f; Generate(); }
    void Start() { Generate(); }

    // 생성
    void Generate() {
        line.startWidth = line.endWidth = width;
        line.numCornerVertices = CORNER_VERTICES;

        GenerateDotLine();

        startPoint.transform.localPosition = line.GetPosition(0);
        endPoint.transform.localPosition = line.GetPosition(line.positionCount - 1);
    }

    protected abstract void GenerateDotLine(); // 점선 생성
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class LineGenerator : MonoBehaviour
{
    [SerializeField] protected float lineWidth = 0.07f; // 선 두께
    [SerializeField] protected LineRenderer line; // Line Renderer
    
    protected const int REPEAT = 3; // 정점 반복 횟수
    protected const int CORNER_VERTICES = 5; // 코너의 정점 수


    void OnValidate()
    {
        lineWidth = 0.07f; 
        Generate();
    }

    void Start()
    {
        Generate();
    }

    // 생성
    void Generate() {
        line.startWidth = line.endWidth = lineWidth;
        line.numCornerVertices = CORNER_VERTICES;

        GenerateDotLine();
    }

    protected abstract void GenerateDotLine(); // 점선 생성
}

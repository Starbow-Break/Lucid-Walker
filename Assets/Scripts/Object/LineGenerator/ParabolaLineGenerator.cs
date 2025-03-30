using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaLineGenerator : LineGenerator
{
    [Min(2), SerializeField] int vertices = 3; // 점 갯수
    [SerializeField, Min(0.0f)] private float parabolaWidth = 1.0f; // 포물선 너비
    [SerializeField] private float gravity = 1.0f;  // 중력
    
    protected override void GenerateDotLine()
    {
        line.positionCount = REPEAT * vertices;
        for (int i = 0; i < vertices; i++)
        {
            float xPos = parabolaWidth / (vertices - 1) * i - parabolaWidth / 2.0f;
            Vector2 position = new Vector2(xPos, GetPositionY(xPos));
            
            for(int cnt = 0; cnt < REPEAT; cnt++) {
                line.SetPosition(REPEAT * i + cnt, position);
            }
        }
    }
    
    // x좌표에 따라 y좌표 구하기
    private float GetPositionY(float x)
    {
        return -gravity / 2.0f * (x + parabolaWidth / 2.0f) * (x - parabolaWidth / 2.0f);
    }
}

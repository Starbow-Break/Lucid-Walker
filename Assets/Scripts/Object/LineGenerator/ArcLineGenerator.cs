using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcLineGenerator : LineGenerator
{
    [Min(2), SerializeField] int vertices = 3; // 점 갯수
    [SerializeField] Vector2 center = Vector2.zero; // 중심
    [Min(0.0f), SerializeField] float radius = 1.0f; // 반지름
    [Range(0.0f, 360.0f), SerializeField] float centralAngle = 60.0f; // 중심각
    [SerializeField] float rotation = 0.0f; // 회전

    // 점선 생성
    protected override void GenerateDotLine() {
        line.positionCount = REPEAT * vertices;

        for(int i = 0; i < vertices; i++) {
            Vector2 position = new(
                Mathf.Cos(2.0f * Mathf.PI * (centralAngle / 360.0f * i / (vertices - 1) + rotation / 360.0f)),
                Mathf.Sin(2.0f * Mathf.PI * (centralAngle / 360.0f * i / (vertices - 1) + rotation / 360.0f))
            );
            position *= radius;
            position += center;

            for(int cnt = 0; cnt < REPEAT; cnt++) {
                line.SetPosition(REPEAT * i + cnt, position);
            }
        }
    }
}

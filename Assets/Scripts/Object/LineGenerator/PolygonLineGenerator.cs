using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonLineGenerator : LineGenerator
{
    [Min(3), SerializeField] int edges = 3; // 변 갯수
    [SerializeField] Vector2 center = Vector2.zero; // 중심
    [Min(0.0f), SerializeField] float radius = 1.0f; // 반지름
    [SerializeField] float rotation = 0.0f; // 회전

    // 점선 생성
    protected override void GenerateDotLine() {
        line.positionCount = REPEAT * (edges + 1);

        for(int i = 0; i <= edges; i++) {
            Vector2 position = new(
                Mathf.Cos(2.0f * Mathf.PI * (1.0f * i / edges + rotation / 360.0f)),
                Mathf.Sin(2.0f * Mathf.PI * (1.0f * i / edges + rotation / 360.0f))
            );
            position *= radius;
            position += center;

            for(int cnt = 0; cnt < REPEAT; cnt++) {
                line.SetPosition(REPEAT * i + cnt, position);
            }
        }
    }
}

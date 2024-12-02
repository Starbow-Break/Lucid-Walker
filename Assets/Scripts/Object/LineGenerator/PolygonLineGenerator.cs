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

    // 라인 생성
    protected override void Generate() {
        line.positionCount = edges + 1;

        for(int i = 0; i < line.positionCount; i++) {
            Vector2 position = new(
                Mathf.Cos(2.0f * Mathf.PI * (1.0f * i / edges + rotation / 360.0f)),
                Mathf.Sin(2.0f * Mathf.PI * (1.0f * i / edges + rotation / 360.0f))
            );
            position *= radius;
            position += center;

            line.SetPosition(i, position);
        }
    }
}

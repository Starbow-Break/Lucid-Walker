using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class StarPolygonLineGenerator : LineGenerator
{
    [Min(5), SerializeField] int edges = 5; // 변 갯수
    [Min(2), SerializeField] int step = 2; // 별 그릴 때 거칠 점의 수
    [SerializeField] Vector2 center = Vector2.zero; // 중심
    [Min(0.0f), SerializeField] float radius = 1.0f; // 반지름
    [SerializeField] float rotation = 0.0f; // 회전

    // 라인 생성
    protected override void Generate() {
        line.positionCount = edges + 1;

        for(int i = 0; i < line.positionCount; i++) {
            Vector2 position = new(
                Mathf.Cos(2.0f * Mathf.PI * (1.0f * step * i / edges + rotation / 360.0f)),
                Mathf.Sin(2.0f * Mathf.PI * (1.0f * step * i / edges + rotation / 360.0f))
            );
            position *= radius;
            position += center;

            line.SetPosition(i, position);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightLineGenerator : LineGenerator
{
    // 선분의 양 끝점
    [SerializeField] Vector2 endPoint1;
    [SerializeField] Vector2 endPoint2;

    // 라인 생성
    protected override void Generate() {
        line.positionCount = 2;
        line.SetPositions(new Vector3[]{endPoint1, endPoint2});
    }
}

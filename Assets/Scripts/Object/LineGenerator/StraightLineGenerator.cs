using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class StraightLineGenerator : LineGenerator
{
    // 선분의 양 끝점
    [SerializeField] Vector2 endPoint1;
    [SerializeField] Vector2 endPoint2;

    // 점선 생성
    protected override void GenerateDotLine() {
        line.positionCount = 2 * REPEAT;
        for(int cnt = 0; cnt < REPEAT; cnt++) {
            line.SetPosition(cnt, endPoint1);
        }
        for(int cnt = 0; cnt < REPEAT; cnt++) {
            line.SetPosition(REPEAT + cnt, endPoint2);
        }
    }
}

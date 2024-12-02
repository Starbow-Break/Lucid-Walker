using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SineWaveLineGenerator : LineGenerator
{
    [Min(2), SerializeField] int vertices = 3; // 점 갯수
    [SerializeField] Vector2 positionOffset = Vector2.zero; // 위치 Offset
    [Min(0.0f), SerializeField] float wavelength = 1.0f; // 파장
    [Min(0.0f), SerializeField] float amplitude = 1.0f; // 진폭
    [Min(0.0f), SerializeField] float time = 1.0f; // 시간 길이
    [SerializeField] float timeOffset = 0.0f; // 시간 Offset

    // 라인 생성
    protected override void Generate() {
        line.positionCount = vertices;

        for(int i = 0; i < line.positionCount; i++) {
            Vector2 position = new(
                1.0f * i / (vertices - 1) * time * wavelength,
                amplitude * Mathf.Sin(2 * Mathf.PI * (time * i / (vertices - 1) + timeOffset))
            );
            position += positionOffset;

            line.SetPosition(i, position);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal; // Light2D 사용

public class StageChangeEffect : MonoBehaviour
{
    public SpriteRenderer curtain; // 커튼 SpriteRenderer
    public TilemapRenderer tilemap; // 타일맵 Renderer
    public Light2D globalLight; // 전역 조명
    public float fadeDuration = 3f; // 페이드 지속 시간
    public CameraFollow cameraFollow; // 카메라 흔들림 참조

    private Color curtainOriginalColor;
    private Color tilemapOriginalColor;

    void Start()
    {
        // 원래 색상 저장
        curtainOriginalColor = curtain.color;
        tilemapOriginalColor = tilemap.material.color;

        // 시작 효과 실행
        StartCoroutine(StartSceneEffect());
    }

    IEnumerator StartSceneEffect()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // 알파값 줄이기
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            curtain.color = new Color(curtainOriginalColor.r, curtainOriginalColor.g, curtainOriginalColor.b, alpha);
            tilemap.material.color = new Color(tilemapOriginalColor.r, tilemapOriginalColor.g, tilemapOriginalColor.b, alpha);

            // 빛의 밝기 줄이기
            if (globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(1f, 0.4f, elapsed / fadeDuration);
            }

            yield return null;
        }

        // 흔들림 효과 시작
        if (cameraFollow != null)
        {
            cameraFollow.TriggerShake();
        }

        // 완전히 투명하게 설정
        curtain.color = new Color(curtainOriginalColor.r, curtainOriginalColor.g, curtainOriginalColor.b, 0f);
        tilemap.material.color = new Color(tilemapOriginalColor.r, tilemapOriginalColor.g, tilemapOriginalColor.b, 0f);

        if (globalLight != null)
        {
            globalLight.intensity = 0.4f;
        }

        Debug.Log("공포 분위기 연출 완료!");
    }
}

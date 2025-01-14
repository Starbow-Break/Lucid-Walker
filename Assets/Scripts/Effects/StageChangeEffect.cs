using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering.Universal; // Light2D 사용

public class StageChangeEffect : MonoBehaviour
{
    public Light2D globalLight; // 전역 조명
    public TilemapRenderer tilemap; // 타일맵 Renderer

    public float fadeDuration = 3f; // 페이드 지속 시간

    public CameraFollow cameraFollow; // 카메라 흔들림 참조
    private Animator anim;
    private Color tilemapOriginalColor;

    void Start()
    {

        anim = GetComponent<Animator>();
        tilemapOriginalColor = tilemap.material.color;

        // 시작 효과 실행
        // StartCoroutine(StartSceneEffect());
    }
    public IEnumerator StartSceneEffect()
    {
        // fadeDuration이 유효하지 않을 경우 기본값 설정
        if (fadeDuration <= 0f)
        {
            fadeDuration = 3f;
        }

        float elapsed = 0f;

        // 카메라 흔들림 효과
        // if (cameraFollow != null)
        // {
        //     cameraFollow.TriggerShake();
        // }

        // 애니메이션 트리거
        if (anim != null)
        {
            anim.SetTrigger("Change");
        }

        // 페이드 효과
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // 알파값 계산
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            // 타일맵 색상 업데이트
            if (tilemap != null && tilemap.material != null)
            {
                tilemap.material.color = new Color(tilemapOriginalColor.r, tilemapOriginalColor.g, tilemapOriginalColor.b, alpha);
            }

            // 빛의 밝기 업데이트
            if (globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(1f, 0.4f, elapsed / fadeDuration);
            }

            // 루프 강제 종료 조건 추가
            if (alpha <= 0.01f && globalLight.intensity <= 0.41f)
            {
                break;
            }

            yield return null;
        }

        // 타일맵 비활성화
        if (tilemap != null)
        {
            tilemap.gameObject.SetActive(false);
        }

        // 빛 밝기 최종 값 설정
        if (globalLight != null)
        {
            globalLight.intensity = 0.4f;
        }
    }

}

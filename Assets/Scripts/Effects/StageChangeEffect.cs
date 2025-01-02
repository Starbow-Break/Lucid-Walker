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
        StartCoroutine(StartSceneEffect());
    }

    IEnumerator StartSceneEffect()
    {
        float elapsed = 0f;

        // 흔들림 효과 시작
        if (cameraFollow != null)
        {
            cameraFollow.TriggerShake();
        }

        anim.SetTrigger("Change");

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            // 알파값 줄이기
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            tilemap.material.color = new Color(tilemapOriginalColor.r, tilemapOriginalColor.g, tilemapOriginalColor.b, alpha);
            // 빛의 밝기 줄이기
            if (globalLight != null)
            {
                globalLight.intensity = Mathf.Lerp(1f, 0.4f, elapsed / fadeDuration);
            }

            yield return null;
        }

        tilemap.material.color = new Color(tilemapOriginalColor.r, tilemapOriginalColor.g, tilemapOriginalColor.b, 0f);


        if (globalLight != null)
        {
            globalLight.intensity = 0.4f;
        }



    }
}

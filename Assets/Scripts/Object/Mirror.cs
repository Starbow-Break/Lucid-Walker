using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Mirror : Warp
{
    [SerializeField] SpriteRenderer mirroringSpriteRenderer; // 거울 상에 사용되는 Material
    [SerializeField] Camera mirroringCamera;
    RenderTexture rt;
    MaterialPropertyBlock mpb;

    float animTime = 2.0f;

    protected override void Awake()
    {   
        base.Awake();

        // Render Texture 생성
        rt = new RenderTexture(256, 256, 16);
        rt.Create();

        // SpriteRenderer에 적용
        mpb = new MaterialPropertyBlock();
        mpb.SetTexture("_MainTex", rt);
        mpb.SetFloat("_Center_Twirl_Intensity", 0.0f);
        mpb.SetFloat("_Rotation", 0.0f);
        mirroringSpriteRenderer.SetPropertyBlock(mpb);

        // 거울 상에 사용되는 카메라에 Render Texture 적용
        mirroringCamera.targetTexture = rt;
    }

    void OnEnable() {
        // 활성화 될 때마다 렌더 텍스쳐 재할당
        // 이유는 모르겠으나 비활성화 후 활성화 시 할당해 놓은 렌더 택스쳐가 사라짐
        mpb.SetTexture("_MainTex", rt);
        mirroringSpriteRenderer.SetPropertyBlock(mpb);
    }

    // 렌더러 초기화
    public void ResetRenderer()
    {
        SetMirrorRenderer(0.0f, 0.0f);
    }

    // 워프 전 애니메이션
    protected override IEnumerator WarpInAnim(GameObject warpTarget)
    {
        float curTime = 0.0f;
        SpriteRenderer warpTargetRenderer = warpTarget.GetComponent<SpriteRenderer>();

        while(curTime < animTime) {
            curTime = Mathf.Min(animTime, curTime + Time.deltaTime);

            // 거울에 효과 적용
            float twirlIntensity = 100.0f * Mathf.Pow(curTime / animTime, 2);
            float mirroringRotation = 12.0f * Mathf.PI * Mathf.Pow(curTime / animTime, 2) % (2.0f * Mathf.PI);
            SetMirrorRenderer(twirlIntensity, mirroringRotation);

            // 플레이어에 효과 적용
            if(warpTargetRenderer != null) {
                float dissolveAmount = curTime / animTime;
                SetWarpTargetRenderer(warpTargetRenderer, dissolveAmount);
            }

            yield return null;
        }

        ResetRenderer();
    }

    // 워프 후 애니메이션
    protected override IEnumerator WarpOutAnim(GameObject warpTarget)
    {
        float curTime = 0.0f;
        Debug.Log(curTime);
        SpriteRenderer warpTargetRenderer = warpTarget.GetComponent<SpriteRenderer>();

        while(curTime < animTime) {
            curTime = Mathf.Min(animTime, curTime + Time.deltaTime);

            float twirlIntensity = 100.0f * Mathf.Pow(curTime / animTime - 1.0f, 2);
            float mirroringRotation = 12.0f * Mathf.PI * (1.0f - Mathf.Pow(curTime / animTime - 1.0f, 2)) % (2.0f * Mathf.PI);
            SetMirrorRenderer(twirlIntensity, mirroringRotation);

            // 플레이어에 효과 적용
            if(warpTargetRenderer != null) {
                float dissolveAmount = 1.0f - curTime / animTime;
                SetWarpTargetRenderer(warpTargetRenderer, dissolveAmount);
            }

            yield return null;
        }
    }

    // 거울상의 렌더러 속성 값 변경
    void SetWarpTargetRenderer(Renderer targetRenderer, float dissolveAmount)
    {
        mpb.SetFloat("_DissolveAmount", dissolveAmount);
        targetRenderer.material.SetFloat("_DissolveAmount", dissolveAmount);
    }

    // 거울상의 렌더러 속성 값 변경
    void SetMirrorRenderer(float twirlIntensity, float mirroringRotation)
    {
        mpb.SetFloat("_Center_Twirl_Intensity", twirlIntensity);
        mpb.SetFloat("_Rotation", mirroringRotation);
        mirroringSpriteRenderer.SetPropertyBlock(mpb);
    }
}

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

    float twirlIntensity = 0.0f;
    float mirroringRotation = 0.0f;
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
        mirroringSpriteRenderer.SetPropertyBlock(mpb);

        // 거울 상에 사용되는 카메라에 Render Texture 적용
        mirroringCamera.targetTexture = rt;
    }

    // 워프 전 애니메이션
    protected override IEnumerator WarpInAnim()
    {
        float curTime = 0.0f;

        while(curTime < animTime) {
            curTime = Mathf.Min(animTime, curTime + Time.deltaTime);

            twirlIntensity = 100.0f * Mathf.Pow(curTime / animTime, 2);
            mirroringRotation = 12.0f * Mathf.PI * Mathf.Pow(curTime / animTime, 2) % (2.0f * Mathf.PI);
            SetMirrorRenderer(twirlIntensity, mirroringRotation);

            yield return null;
        }
    }

    // 워프 후 애니메이션
    protected override IEnumerator WarpOutAnim()
    {
        float curTime = 0.0f;

        while(curTime < animTime) {
            curTime = Mathf.Min(animTime, curTime + Time.deltaTime);

            twirlIntensity = 100.0f * Mathf.Pow(curTime / animTime - 1.0f, 2);
            mirroringRotation = 12.0f * Mathf.PI * (1.0f - Mathf.Pow(curTime / animTime - 1.0f, 2)) % (2.0f * Mathf.PI);
            SetMirrorRenderer(twirlIntensity, mirroringRotation);

            yield return null;
        }
    }

    // 거울상의 렌더러 속성 값 변경
    void SetMirrorRenderer(float twirlIntensity, float mirroringRotation)
    {
        mpb.SetFloat("_Center_Twirl_Intensity", twirlIntensity);
        mpb.SetFloat("_Rotation", mirroringRotation);
        mirroringSpriteRenderer.SetPropertyBlock(mpb);
    }
}

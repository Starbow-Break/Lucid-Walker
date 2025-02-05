using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    [SerializeField] SpriteRenderer mirroringSpriteRenderer; // 거울 상에 사용되는 Material
    [SerializeField] Camera mirroringCamera;

    RenderTexture rt;
    MaterialPropertyBlock mpb;

    protected virtual void Awake()
    {   
        // Render Texture 생성 후 Sprite Renderer에 할당
        rt = new RenderTexture(256, 256, 16);
        rt.Create();

        // SpriteRenderer에 적용
        mpb = new MaterialPropertyBlock();
        mpb.SetTexture("_MirroringTexture", rt);
        mpb.SetFloat("_Twirl_Strength", 0.0f);
        mpb.SetFloat("_Twirl_Rotation", 0.0f);
        mirroringSpriteRenderer.SetPropertyBlock(mpb);

        // 거울 상에 사용되는 카메라에 Render Texture 적용
        mirroringCamera.targetTexture = rt;
    }

    void OnEnable() {
        // 활성화 될 때마다 렌더 텍스쳐 재할당
        // 이유는 모르겠으나 비활성화 후 활성화 시 할당해 놓은 렌더 택스쳐가 사라짐
        mpb.SetTexture("_MirroringTexture", rt);
        mirroringSpriteRenderer.SetPropertyBlock(mpb);
    }

    // 렌더러 초기화
    public void ResetRenderer()
    {
        SetMirrorRenderer(0.0f, 0.0f);
    }

    // 워프 대상 오브젝트의 속성 값 변경
    public void SetWarpTargetRenderer(Renderer targetRenderer, float dissolveAmount)
    {
        mpb.SetFloat("_DissolveAmount", dissolveAmount);
        targetRenderer.SetPropertyBlock(mpb);
        // targetRenderer.material.SetFloat("_DissolveAmount", dissolveAmount);
    }

    // 거울상의 렌더러 속성 값 변경
    public void SetMirrorRenderer(float twirlStrength, float twirlRotation)
    {
        mpb.SetFloat("_Twirl_Strength", twirlStrength);
        mpb.SetFloat("_Twirl_Rotation", twirlRotation);
        mirroringSpriteRenderer.SetPropertyBlock(mpb);
    }
}

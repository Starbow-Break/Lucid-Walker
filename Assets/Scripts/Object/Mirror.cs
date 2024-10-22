using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : Warp
{
    [SerializeField] SpriteRenderer mirroringSpriteRenderer; // 거울 상에 사용되는 Material
    [SerializeField] Camera mirroringCamera;
    RenderTexture rt;
    MaterialPropertyBlock mpb;

    void Awake()
    {
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
}

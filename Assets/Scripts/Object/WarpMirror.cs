using System.Collections;
using UnityEngine;

public class WarpMirror : Warp
{
    Mirror mirror; // 거울
    float animTime = 2.0f;

    void Awake()
    {   
        mirror = GetComponent<Mirror>();

        if(mirror == null) {
            throw new NotExistMirrorComponentException("Mirror 컴포넌트가 존재하지 않습니다.");
        }
    }

    // 워프 전 애니메이션
    public override IEnumerator WarpInAnim(GameObject warpTarget)
    {
        float curTime = 0.0f;
        SpriteRenderer warpTargetRenderer = warpTarget.GetComponent<SpriteRenderer>();

        while(curTime < animTime) {
            curTime = Mathf.Min(animTime, curTime + Time.deltaTime);

            // 거울에 효과 적용
            float twirlIntensity = 100.0f * Mathf.Pow(curTime / animTime, 2);
            float mirroringRotation = 12.0f * Mathf.PI * Mathf.Pow(curTime / animTime, 2) % (2.0f * Mathf.PI);
            mirror.SetMirrorRenderer(twirlIntensity, mirroringRotation);

            // 플레이어에 효과 적용
            if(warpTargetRenderer != null) {
                float dissolveAmount = curTime / animTime;
                mirror.SetWarpTargetRenderer(warpTargetRenderer, dissolveAmount);
            }

            yield return null;
        }

        mirror.ResetRenderer();
    }

    // 워프 후 애니메이션
    public override IEnumerator WarpOutAnim(GameObject warpTarget)
    {
        float curTime = 0.0f;
        SpriteRenderer warpTargetRenderer = warpTarget.GetComponent<SpriteRenderer>();

        while(curTime < animTime) {
            curTime = Mathf.Min(animTime, curTime + Time.deltaTime);

            float twirlIntensity = 100.0f * Mathf.Pow(curTime / animTime - 1.0f, 2);
            float mirroringRotation = 12.0f * Mathf.PI * (1.0f - Mathf.Pow(curTime / animTime - 1.0f, 2)) % (2.0f * Mathf.PI);
            mirror.SetMirrorRenderer(twirlIntensity, mirroringRotation);

            // 플레이어에 효과 적용
            if(warpTargetRenderer != null) {
                float dissolveAmount = 1.0f - curTime / animTime;
                mirror.SetWarpTargetRenderer(warpTargetRenderer, dissolveAmount);
            }

            yield return null;
        }
    }

    #region EXCEPTION
    class NotExistMirrorComponentException : System.Exception {
        public NotExistMirrorComponentException(string message) : base(message) {}
    }
    #endregion
}

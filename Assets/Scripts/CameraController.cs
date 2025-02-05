using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Coroutine coroutine = null;
    CinemachineVirtualCamera virtualCamera;

    void Start() {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // 카메라 줌 조절
    public void CameraZoom(Vector3 offset, float orthoSize, float zoomSpeed) {
        if(virtualCamera == null) return;

        // 일단 작동중인 코루틴이 존재하면 해당 코루틴을 중지
        if(coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(ZoomFlow(offset, orthoSize, zoomSpeed));
    }

    IEnumerator ZoomFlow(Vector3 offset, float orthoSize, float zoomSpeed) {
        float epsilon = 0.001f;

        CinemachineFramingTransposer framingTransposer 
            = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        float startOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        Vector3 startOffset = framingTransposer.m_TrackedObjectOffset;

        float orthoVel = 2.0f * (orthoSize - startOrthoSize);
        Vector3 offsetVel = 2.0f * (offset - startOffset);

        while(
            Mathf.Abs(orthoSize - virtualCamera.m_Lens.OrthographicSize) > epsilon
            || (offset - framingTransposer.m_TrackedObjectOffset).sqrMagnitude > Mathf.Pow(epsilon, 2.0f)
        ) {
            yield return null;
            
            float newOrthoSize = Mathf.Lerp(virtualCamera.m_Lens.OrthographicSize, orthoSize, zoomSpeed * Time.deltaTime);
            Vector3 newOffset = Vector3.Lerp(framingTransposer.m_TrackedObjectOffset, offset, zoomSpeed * Time.deltaTime);

            virtualCamera.m_Lens.OrthographicSize = newOrthoSize;
            framingTransposer.m_TrackedObjectOffset = newOffset;
        }

        virtualCamera.m_Lens.OrthographicSize = orthoSize;
        framingTransposer.m_TrackedObjectOffset = offset;

        yield return null;
    }
}

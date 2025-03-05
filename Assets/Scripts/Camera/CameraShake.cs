using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType<CameraShake>();
            }

            return m_instance;
        }
    }
    private static CameraShake m_instance;

    float shakeTime;

    public void ShakeActiveCamera(float intensity, float time = float.PositiveInfinity) {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            CameraManager.ActiveCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        shakeTime = time;
    }

    public void StopShakeActiveCamera() {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
            CameraManager.ActiveCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0.0f;
        shakeTime = 0.0f;
    }

    void Update() {
        if (shakeTime > 0.0f) {
            shakeTime -= Time.deltaTime;
            if(shakeTime <= 0.0f) {
                StopShakeActiveCamera();
            }
        }
    }
}

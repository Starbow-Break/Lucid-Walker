using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineCameraSwitcher : MonoBehaviour
{
    [SerializeField] List<CinemachineVirtualCamera> cameras;

    // 지정한 카메라로 전환
    // 만약 원하는 카메라가 없다면 예상치 못한 결과가 발생할 수 있다.
    public void CameraSwitch(string cameraName) {
        foreach(CinemachineVirtualCamera camera in cameras) {
            if(camera.name == cameraName) {
                camera.Priority = 10;
            }
            else {
                camera.Priority = 0;
            }
        }
    }
}

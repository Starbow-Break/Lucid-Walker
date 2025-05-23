using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;
    [Header("Controls for lerping the Y damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float _fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private CinemachineFramingTransposer _framingTransposer;
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    public static CinemachineVirtualCamera ActiveCamera = null;
    private CinemachineVirtualCamera _currentCamera;

    private float _normYPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (_allVirtualCameras.Length > 0)
        {
            ActiveCamera = _allVirtualCameras[0];
            RegisterAll(_allVirtualCameras);

            var framing = GetFramingTransposer(ActiveCamera);
            if (framing != null)
                _normYPanAmount = framing.m_YDamping;
        }

    }

    // FramingTransposer 동적 접근
    private CinemachineFramingTransposer GetFramingTransposer(CinemachineVirtualCamera cam)
    {
        return cam?.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // 흔들림용 Perlin 동적 접근
    private CinemachineBasicMultiChannelPerlin GetPerlin(CinemachineVirtualCamera cam)
    {
        return cam?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }


    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }
    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;
        // grab the starting damping amount
        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        // lerp the pan amount 
        float elapsedTime = 0f;
        while (elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / _fallYPanTime));
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }
    public static bool IsActiveCamera(CinemachineVirtualCamera camera)
    {
        return camera == ActiveCamera;
    }


    public static void SwitchCamera(CinemachineVirtualCamera newCamera)
    {
        newCamera.Priority = 10;
        ActiveCamera = newCamera;

        foreach (CinemachineVirtualCamera cam in cameras)
        {
            if (cam != newCamera)
            {
                cam.Priority = 0;
            }
        }
    }
    public static void SwitchCamera(string cameraName)
    {
        foreach (CinemachineVirtualCamera camera in cameras)
        {
            if (camera.name == cameraName)
            {
                SwitchCamera(camera);
                return;
            }
        }

        Debug.LogError("카메라를 찾을 수 없습니다.");
    }
    public static void Register(CinemachineVirtualCamera camera)
    {
        cameras.Add(camera);
    }

    public static void Unregister(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera);
    }

    public static void RegisterAll(CinemachineVirtualCamera[] camList)
    {
        foreach (var cam in camList)
            Register(cam);
    }
}

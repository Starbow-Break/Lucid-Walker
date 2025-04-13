using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class Tutorial : MonoBehaviour
{
    [Header("뉴스 끝났을 때 활성화될 것들")]
    public GameObject volumeObject;
    public CinemachineVirtualCamera nextCamera;
    public float cameraSwitchDelay = 1.0f;
    public float volumeFadeDuration = 1.0f;
    public float volumeFadeOutDelay = 1.5f; // 불 켜지고 얼마 뒤 꺼질지
    [SerializeField] private Animator BGanim;

    private Volume volume;

    private void Start()
    {
        volume = volumeObject.GetComponent<Volume>();
        if (volume != null)
        {
            volume.weight = 0f;
            volumeObject.SetActive(true);
        }
    }

    public void OnNewsEnd()
    {
        // 전체 연출 순서를 하나의 코루틴으로 묶어서 처리
        StartCoroutine(TutorialSequence());
    }
    private IEnumerator TutorialSequence()
    {
        // 1. Volume 켜기
        if (volume != null)
            yield return StartCoroutine(FadeInVolume());

        // 2. 카메라 전환
        yield return StartCoroutine(DelayedCameraSwitch());

        // 3. 배경 불 켜기
        BGanim.SetTrigger("On");

        // 4. 볼륨 꺼지기까지 기다렸다가
        yield return StartCoroutine(FadeOutVolumeAfterDelay());

        // 5. 저장 및 메인 씬 이동
        OnTutorialComplete();
    }

    private IEnumerator FadeInVolume()
    {
        float elapsed = 0f;
        while (elapsed < volumeFadeDuration)
        {
            elapsed += Time.deltaTime;
            volume.weight = Mathf.Lerp(0f, 1f, elapsed / volumeFadeDuration);
            yield return null;
        }

        volume.weight = 1f;
    }

    private IEnumerator FadeOutVolumeAfterDelay()
    {
        yield return new WaitForSeconds(volumeFadeOutDelay);

        float elapsed = 0f;
        float startWeight = volume.weight;

        while (elapsed < volumeFadeDuration)
        {
            elapsed += Time.deltaTime;
            volume.weight = Mathf.Lerp(startWeight, 0f, elapsed / volumeFadeDuration);
            yield return null;
        }

        volume.weight = 0f;
    }

    private IEnumerator DelayedCameraSwitch()
    {
        yield return new WaitForSeconds(cameraSwitchDelay);
        CameraManager.SwitchCamera(nextCamera);
    }

    public void OnTutorialComplete()
    {
        // 데이터 갱신
        var data = DataPersistenceManager.instance.GetCurrentGameData();
        data.tutorialData.isCompleted = true;

        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("Main");
    }

}

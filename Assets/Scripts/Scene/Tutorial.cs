using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class Tutorial : MonoBehaviour
{
    [Header("뉴스 끝났을 때 활성화될 것들")]
    public GameObject volumeObject;
    public CinemachineVirtualCamera originalCamera;
    public CinemachineVirtualCamera bgCamera;
    public float cameraSwitchDelay = 1.0f;
    public float volumeFadeDuration = 1.0f;
    public float volumeFadeOutDelay = 1.5f;
    [SerializeField] private Animator BGanim;

    private Volume volume;
    private bool canProceed = false;

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
        StartCoroutine(TutorialSequence());
    }

    private IEnumerator TutorialSequence()
    {
        // 1. 카메라 전환

        CameraManager.SwitchCamera(bgCamera);
        // 2. volume 변환
        if (volume != null)
            yield return StartCoroutine(FadeInVolume());

        // yield return new WaitForSeconds(cameraSwitchDelay);

        // 3. TurnOff 애니메이션 트리거

        // 4. 4.5초 후 원래 카메라로 전환
        yield return new WaitForSeconds(4.5f);
        // CameraManager.SwitchCamera(originalCamera);

        // 5. 볼륨 꺼지기 (애니메이션 15초 기준, 10.5초 후니까)
        yield return new WaitForSeconds(10.5f - 4.5f);
        // yield return StartCoroutine(FadeOutVolumeAfterDelay());
        BGanim.SetTrigger("TurnOff");
        yield return new WaitForSeconds(7f); // 대충 전환 마무리 시간

        OnTutorialComplete(); // 자동 호출

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

    private void Update()
    {
        if (canProceed && Input.GetMouseButtonDown(0))
        {
            OnTutorialComplete();
            canProceed = false;
        }
    }

    public void OnTutorialComplete()
    {
        var data = DataPersistenceManager.instance.GetCurrentGameData();
        data.tutorialData.isCompleted = true;

        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadSceneAsync("Main");
    }
}

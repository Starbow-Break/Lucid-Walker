using UnityEngine;
using System.Collections;

public class NewsIntroSoundController : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioClip introBGM;
    public AudioClip newsMain;
    public AudioClip crackleSFX;         // 치지직 효과음
    public AudioSource fxSource;         // 효과음 전용 AudioSource (bgmSource랑 분리)

    [SerializeField] private GameObject newsIntroSprite;
    [SerializeField] private GameObject newsMainSprite;
    [SerializeField] private Animator fadeAnimator;
    public float introDuration = 8.4f; // 인트로 길이 (초)

    void Start()
    {
        // 인트로 BGM 재생
        bgmSource.clip = introBGM;
        bgmSource.Play();

        // 일정 시간 후 다음 씬 혹은 연출 진입
        StartCoroutine(GoToNewsMain());
    }

    IEnumerator GoToNewsMain()
    {
        yield return new WaitForSeconds(introDuration);

        fadeAnimator.Play("FadeIn");

        yield return new WaitForSeconds(1f);

        newsIntroSprite.SetActive(false);
        newsMainSprite.SetActive(true);

        bgmSource.clip = newsMain;
        bgmSource.Play();

        // 🧨 18초 뒤에 치지직 효과음 재생
        StartCoroutine(PlayCrackleSFXAfterDelay(18f));
    }


    IEnumerator PlayCrackleSFXAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        fxSource.PlayOneShot(crackleSFX);
    }



    IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // 볼륨 원상복귀
    }

}

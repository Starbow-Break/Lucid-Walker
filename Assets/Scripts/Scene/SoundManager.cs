using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;          // 효과음 전용
    public AudioSource backgroundMusic;    // 배경음 전용

    [Header("Audio Clips - SFX")]
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip swimSound;

    [Header("Audio Clips - BGM")]
    public AudioClip mainTheme;
    public AudioClip stage6Theme;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 넘어가도 안 사라지게
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Stage6")
        {
            PlayBackgroundMusic(stage6Theme);
        }
        if (currentScene == "Main")
        {
            PlayBackgroundMusic(mainTheme);
        }

    }


    // --------- SFX ----------
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayWalk() => PlaySFX(walkSound);
    public void PlayRun() => PlaySFX(runSound);
    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayLand() => PlaySFX(landSound);
    public void PlaySwim() => PlaySFX(swimSound);

    // --------- BGM ----------
    public void PlayBackgroundMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        if (backgroundMusic.clip == clip && backgroundMusic.isPlaying) return;

        backgroundMusic.clip = clip;
        backgroundMusic.loop = loop;
        backgroundMusic.Play();
    }

    public void StopBackgroundMusic()
    {
        backgroundMusic.Stop();
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusic.volume = Mathf.Clamp01(volume);
    }

    public void FadeOutBackgroundMusic(float duration)
    {
        StartCoroutine(FadeOutBGM(duration));
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = backgroundMusic.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            backgroundMusic.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        backgroundMusic.Stop();
        backgroundMusic.volume = startVolume; // 원래대로 돌려놓기
    }
}

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource; // 효과음 전용 오디오 소스

    [Header("Audio Clips")]
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip swimSound;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayWalk()
    {
        PlaySFX(walkSound);
    }

    public void PlayRun()
    {
        PlaySFX(runSound);
    }

    public void PlayJump()
    {
        PlaySFX(jumpSound);
    }

    public void PlayLand()
    {
        PlaySFX(landSound);
    }

    public void PlaySwim()
    {
        PlaySFX(swimSound);
    }
}

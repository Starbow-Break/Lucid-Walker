using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskBossPhase1and2Audio : MonoBehaviour
{
    [SerializeField] AudioClip _whistleClip;
    [SerializeField] AudioClip _fingerSnapClip;

    public void PlayWhistleSound()
    {
        AudioManager.MaskBossPhase12Audio.PlayOneShot(_whistleClip);
    }

    public void PlayFingerSnapSound()
    {
        AudioManager.MaskBossPhase12Audio.PlayOneShot(_fingerSnapClip);
    }
}

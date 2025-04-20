using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _maskBossPhase12Audio;

    public static AudioManager Instance { get; private set; }

    public static AudioSource MaskBossPhase12Audio => Instance._maskBossPhase12Audio;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else {
            if(Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}

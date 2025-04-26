using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Whistle")]
    [SerializeField] private AudioSource _whistle;

    [Header("Whistle")]
    [SerializeField] private AudioSource _fingerSnap;

    [Header("Whistle")]
    [SerializeField] private AudioSource _applause;

    public static AudioManager Instance { get; private set; }

    public static AudioSource Whistle => Instance._whistle;
    public static AudioSource FingerSnap => Instance._fingerSnap;
    public static AudioSource Applause => Instance._applause;

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

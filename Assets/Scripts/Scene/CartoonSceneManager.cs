using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StageCartoonEntry
{
    [Tooltip("컷툰이 있는 스테이지 번호")]
    public int stageNumber;
    [Tooltip("해당 스테이지 컷툰 패널")]
    public CartoonPanel panel;
    [Tooltip("컷툰 재생 시 사용할 BGM (선택)")]
    public AudioClip bgmClip;
}

public class CartoonSceneManager : MonoBehaviour
{
    public static CartoonSceneManager Instance;

    [Header("컷툰 매핑 (스테이지 번호 ↔ 패널+BGM)")]
    public List<StageCartoonEntry> stageCartoonEntries;
    [Header("보스 스테이지 컷툰 (스테이지 7)")]
    public CartoonPanel bossCartoonPanel;
    [Tooltip("보스 컷툰 재생 시 사용할 BGM (선택)")]
    public AudioClip bossBgmClip;

    // 런타임에 빠르게 조회하기 위한 사전
    private Dictionary<int, StageCartoonEntry> _cartoonMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 리스트 → 딕셔너리
        _cartoonMap = new Dictionary<int, StageCartoonEntry>();
        foreach (var entry in stageCartoonEntries)
        {
            if (_cartoonMap.ContainsKey(entry.stageNumber))
            {
                Debug.LogWarning($"중복된 StageCartoonEntry: Stage {entry.stageNumber}");
                continue;
            }
            _cartoonMap.Add(entry.stageNumber, entry);
        }

        // 보스 스테이지(7) 강제 등록
        if (bossCartoonPanel != null)
        {
            var bossEntry = new StageCartoonEntry
            {
                stageNumber = 7,
                panel = bossCartoonPanel,
                bgmClip = bossBgmClip
            };
            _cartoonMap[7] = bossEntry;
        }
    }

    public void PlayCartoon(int stageNumber, Action onComplete)
    {
        if (!_cartoonMap.TryGetValue(stageNumber, out var entry) || entry.panel == null)
        {
            onComplete?.Invoke();
            return;
        }

        // 1) 메인 BGM 즉시 정지
        SoundManager.Instance.StopBackgroundMusic();

        // 2) 컷툰용 BGM 재생 (loop = false)
        if (entry.bgmClip != null)
            SoundManager.Instance.PlayBackgroundMusic(entry.bgmClip, loop: false);

        // 컷툰 패널 재생
        entry.panel.Play(() =>
        {
            onComplete?.Invoke();
            // 씬 전환 시 SoundManager.OnSceneLoaded에서 BGM 복구
        });
    }
}

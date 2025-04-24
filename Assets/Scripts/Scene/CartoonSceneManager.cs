using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartoonSceneManager : MonoBehaviour
{
    public static CartoonSceneManager Instance;
    [Header("각 스테이지 컷툰 패널")]
    public List<CartoonPanel> stageCartoonPanels;  // 인덱스 = stageNumber - 1

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayCartoon(int stageNumber, Action onComplete)
    {
        if (stageNumber <= 0 || stageNumber > stageCartoonPanels.Count)
        {
            Debug.LogWarning($"❌ Stage {stageNumber} 컷툰 없음");
            onComplete?.Invoke();
            return;
        }

        var panel = stageCartoonPanels[stageNumber - 1];
        panel.Play(onComplete);
    }

}

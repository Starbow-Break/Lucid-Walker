using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartoonCardUI : MonoBehaviour
{
    [SerializeField] private Image thumbnailImage;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Button playButton;

    [SerializeField] private int stageNumber;

    public void Setup(StageProgress sp, Sprite cartoonSprite)
    {
        stageNumber = sp.stageNumber;

        bool isUnlocked = sp.cartoonScenePlayed;

        // 🔍 썸네일 설정
        thumbnailImage.sprite = cartoonSprite;

        // 🔒 잠금 상태 처리
        lockOverlay.SetActive(!isUnlocked);
        lockIcon.gameObject.SetActive(!isUnlocked);

        playButton.interactable = isUnlocked;
        playButton.onClick.RemoveAllListeners();

        if (isUnlocked)
        {
            playButton.onClick.AddListener(() =>
            {
                CartoonSceneManager.Instance.PlayCartoon(stageNumber, null);
            });
        }
    }
}


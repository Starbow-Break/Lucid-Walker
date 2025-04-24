using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StageIcon : MonoBehaviour
{

    public int stageIndex; // 0부터 시작 (Stage1 = 0)
    public Button button;
    public Image basePlateImage;
    public GameObject cartoonBookIcon;
    public GameObject lightEffect;

    public Sprite defaultSprite;
    public Sprite clearedWithTreasureSprite;
    public Sprite clearedNoTreasureSprite;
    public void Setup(int index, StageSelectManager manager)
    {
        stageIndex = index;
        button.onClick.AddListener(() => manager.TryEnterStageByClick(stageIndex));
        Debug.Log($"✅ 버튼 연결 완료: Stage {stageIndex}");

    }
    public void UpdateUI(StageProgress sp, bool isCurrent)
    {
        if (!sp.isCleared)
        {
            basePlateImage.sprite = defaultSprite;
            cartoonBookIcon.SetActive(false);
        }
        else
        {
            basePlateImage.sprite = sp.gotTreasure ? clearedWithTreasureSprite : clearedNoTreasureSprite;
            cartoonBookIcon.SetActive(sp.hasCartoonScene);
        }

        lightEffect.SetActive(isCurrent);
    }
}

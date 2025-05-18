using UnityEngine;
using System.Collections.Generic;

public class CartoonListManager : MonoBehaviour
{
  [SerializeField] private GameObject cardPrefab;
  [SerializeField] private Transform contentRoot;

  // 인스펙터에서 각 스테이지 번호에 맞게 썸네일들 연결
  [SerializeField] private List<Sprite> cartoonThumbnails;

  public void Build(GameData gameData)
  {
    // 기존 항목 제거
    foreach (Transform child in contentRoot)
      Destroy(child.gameObject);

    EpisodeData ep1 = gameData.GetEpisodeData(1);

    foreach (StageProgress sp in ep1.stageProgresses)
    {
      if (!sp.hasCartoonScene) continue;

      GameObject go = Instantiate(cardPrefab, contentRoot);
      var card = go.GetComponent<CartoonCardUI>();

      // thumbnail 리스트는 index = stageNumber - 1이라고 가정
      Sprite thumb = cartoonThumbnails[sp.stageNumber - 1];
      card.Setup(sp, thumb);
    }
  }
}

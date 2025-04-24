using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartoonPanel : MonoBehaviour
{
  public List<GameObject> panels; // Panel1, Panel2, ... 넣기
  public float delayBetweenPanels = 1.2f; // 각 패널 사이 시간


  public void Play(Action onComplete)
  {
    ResetPanels();
    gameObject.SetActive(true);
    StartCoroutine(PlayPanelsSequentially(onComplete));
  }

  private IEnumerator PlayPanelsSequentially(Action onComplete)
  {
    foreach (var panel in panels)
    {
      panel.SetActive(true);
      yield return new WaitForSeconds(delayBetweenPanels);
    }

    yield return new WaitForSeconds(1f); // 마지막 패널 보여줄 시간
    onComplete?.Invoke();
    gameObject.SetActive(false);
    ResetPanels(); // 다음 재생을 위해 초기화
  }

  private void ResetPanels()
  {
    foreach (var panel in panels)
      panel.SetActive(false);
  }
}

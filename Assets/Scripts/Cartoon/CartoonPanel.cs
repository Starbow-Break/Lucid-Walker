using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIEffects;

public class CartoonPanel : MonoBehaviour
{
  [Header("컷툰 패널들 (Panel1, Panel2 …)")]
  public List<GameObject> panels;

  [Header("타이밍")]
  [SerializeField] private float delayBetweenPanels = 1.2f;   // 패널 간 간격
  [SerializeField] private float transitionDuration = 1;   // UIEffect 디졸브 시간
  [SerializeField] private float waitAfterTransition = 0.4f;  // 디졸브 후 추가 대기

  [Header("UIEffect")]
  [SerializeField] private UIEffect uiEffect;

  public void Play(Action onComplete)
  {
    StopAllCoroutines();
    ResetPanels();
    gameObject.SetActive(true);
    StartCoroutine(PlaySequence(onComplete));
  }

  // -------------------- 내부 코루틴 --------------------

  private IEnumerator PlaySequence(Action onComplete)
  {
    // 1) 디졸브-인 (Transition Rate 1 → 0)
    yield return StartCoroutine(DissolveIn());

    // 2) 약간 대기 후 컷툰 진행
    yield return new WaitForSeconds(waitAfterTransition);
    yield return StartCoroutine(PlayPanelsSequentially());

    // 3) 끝
    onComplete?.Invoke();
    gameObject.SetActive(false);
    ResetPanels();
  }
  private IEnumerator DissolveIn()
  {
    if (uiEffect == null) yield break;

    uiEffect.transitionRate = 1f;    // 시작값
    float t = 0;
    while (t < transitionDuration)
    {
      t += Time.deltaTime;
      uiEffect.transitionRate = Mathf.Lerp(1f, 0f, t / transitionDuration);
      yield return null;
    }
    uiEffect.transitionRate = 0f;    // 보정
  }

  private IEnumerator PlayPanelsSequentially()
  {
    foreach (var panel in panels)
    {
      panel.SetActive(true);
      yield return new WaitForSeconds(delayBetweenPanels);
    }

    // 마지막 패널이 조금 더 보여지도록
    yield return new WaitForSeconds(1f);
  }

  private void ResetPanels()
  {
    foreach (var panel in panels)
      panel.SetActive(false);
  }
}

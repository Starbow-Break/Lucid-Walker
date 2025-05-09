using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIEffects;

public class CartoonPanel : MonoBehaviour
{
  [Header("컷툰 패널들")]
  public List<GameObject> panels;
  [Header("패널별 효과음")]
  public List<AudioClip> panelSFXClips;

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

  private IEnumerator PlaySequence(Action onComplete)
  {
    yield return StartCoroutine(DissolveIn());
    yield return new WaitForSeconds(waitAfterTransition);
    yield return StartCoroutine(PlayPanelsSequentially());

    // 끝
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
    for (int i = 0; i < panels.Count; i++)
    {
      panels[i].SetActive(true);

      // 효과음 재생
      if (i < panelSFXClips.Count && panelSFXClips[i] != null)
      {
        SoundManager.Instance.PlaySFX(panelSFXClips[i]);
      }

      yield return new WaitForSeconds(delayBetweenPanels);
    }

    yield return new WaitForSeconds(1f); // 마지막 컷 조금 더 보여지도록
  }

  private void ResetPanels()
  {
    foreach (var panel in panels)
      panel.SetActive(false);
  }
}

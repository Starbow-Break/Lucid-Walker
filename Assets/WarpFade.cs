using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Timeline;

public class WarpFade : MonoBehaviour
{
    [SerializeField] RectTransform hole;
    [SerializeField] RectTransform background;

    Vector2 offset;

    private void Awake() {
        offset = new(-Screen.width / 2, -Screen.height / 2);
    }

    private void OnEnable() {
        // 활성화 될 때마다 배경의 크기를 게임의 해상도에 맞춘다.
        background.sizeDelta = new(Screen.width, Screen.height);
    }

    public IEnumerator FadeInFlow(Vector2 targetScreenPosition, float duration) {
        // 초기화
        float initHolesize = 2 * Mathf.Sqrt(Mathf.Pow(Screen.width, 2) + Mathf.Pow(Screen.height, 2));
        float endHoleSize = 0.0f;

        hole.sizeDelta = initHolesize * Vector2.one;
        hole.localPosition = targetScreenPosition + offset;
        background.localPosition = -hole.localPosition;

        float playTime = 0.0f;
        while(playTime < duration) {
            yield return null;

            playTime += Time.deltaTime;
            float newHoleSize = Mathf.Lerp(initHolesize, endHoleSize, playTime / duration);

            hole.sizeDelta = newHoleSize * Vector2.one;
        }

        yield return null;
    }

    public IEnumerator FadeOutFlow(Vector2 targetScreenPosition, float duration) {
        // 초기화
        float initHolesize = 0.0f;
        float endHolesize = 2 * Mathf.Sqrt(Mathf.Pow(Screen.width, 2) + Mathf.Pow(Screen.height, 2));

        hole.sizeDelta = initHolesize * Vector2.one;
        hole.localPosition = targetScreenPosition + offset;
        background.localPosition = -hole.localPosition;

        float playTime = 0.0f;
        while(playTime < duration) {
            yield return null;

            playTime += Time.deltaTime;
            float newHoleSize = Mathf.Lerp(initHolesize, endHolesize, playTime / duration);

            hole.sizeDelta = newHoleSize * Vector2.one;
        }

        yield return null;
    }
}

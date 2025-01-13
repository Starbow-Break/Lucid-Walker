using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class CircleWipe : SceneTransition
{
    public Image circle;

    public override IEnumerator AnimateTransitionIn()
    {
        // 원을 화면 밖 왼쪽에 배치
        circle.rectTransform.anchoredPosition = new Vector2(-1000f, 0f);
        var tweener = circle.rectTransform.DOAnchorPosX(0f, 1f); // 중앙으로 이동

        yield return tweener.WaitForCompletion();
    }

    public override IEnumerator AnimateTransitionOut()
    {
        var tweener = circle.rectTransform.DOAnchorPosX(1000f, 1f);
        yield return tweener.WaitForCompletion();
    }

    // 새 메서드: 같은 씬 내 전환 효과
    public void PlayTransition(bool vertical, bool isEntering, float delay = 0.5f)
    {
        circle.gameObject.SetActive(true); // 전환 효과 시작 시 활성화

        if (vertical)
        {
            // 수직 이동 (위에서 아래 또는 아래에서 위)
            circle.rectTransform.anchoredPosition = isEntering ? new Vector2(0f, 1000f) : new Vector2(0f, -1000f);
            var tweener = circle.rectTransform.DOAnchorPosY(0f, 1f); // 중앙으로 이동

            tweener.onComplete += () =>
            {
                Debug.Log("중앙 이동 완료, 딜레이 시작!");
                StartCoroutine(DelayedMoveVertical(-1000f, 1f, delay)); // 딜레이 후 다시 이동
            };
        }
        else
        {
            // 수평 이동 (좌에서 우 또는 우에서 좌)
            circle.rectTransform.anchoredPosition = isEntering ? new Vector2(-1000f, 0f) : new Vector2(1000f, 0f);
            var tweener = circle.rectTransform.DOAnchorPosX(0f, 1f); // 중앙으로 이동

            tweener.onComplete += () =>
            {
                Debug.Log("중앙 이동 완료, 딜레이 시작!");
                StartCoroutine(DelayedMoveHorizontal(1000f, 1f, delay)); // 딜레이 후 다시 이동
            };
        }
    }

    // 수직 이동 후 딜레이와 함께 다시 이동
    private IEnumerator DelayedMoveVertical(float targetY, float duration, float delay)
    {
        yield return new WaitForSeconds(delay); // 딜레이
        var tweener = circle.rectTransform.DOAnchorPosY(targetY, duration); // 화면 밖으로 이동
        tweener.onComplete += () =>
        {
            circle.gameObject.SetActive(false); // Out 효과 후 비활성화
            Debug.Log("수직 이동 완료, Circle 비활성화");
        };
    }

    // 수평 이동 후 딜레이와 함께 다시 이동
    private IEnumerator DelayedMoveHorizontal(float targetX, float duration, float delay)
    {
        yield return new WaitForSeconds(delay); // 딜레이
        var tweener = circle.rectTransform.DOAnchorPosX(targetX, duration); // 화면 밖으로 이동
        tweener.onComplete += () =>
        {
            circle.gameObject.SetActive(false); // Out 효과 후 비활성화
            Debug.Log("수평 이동 완료, Circle 비활성화");
        };
    }

}

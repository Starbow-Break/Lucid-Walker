using UnityEngine;

public class BalloonFollowMulti : MonoBehaviour
{
    public CharacterSwitchManager characterSwitchManager; // CharacterSwitchManager 참조
    public Camera mainCamera; // 메인 카메라
    public RectTransform canvasRect; // UI 캔버스 RectTransform
    public RectTransform[] balloonRects; // 말풍선 RectTransform 배열 (좌, 우, 상, 하)
    public UnityEngine.UI.Image[] faceImages; // 말풍선에 표시할 캐릭터 얼굴 배열
    public Sprite[] characterFaces; // 캐릭터 얼굴 이미지 배열 (플레이어, 여자 캐릭터 등)

    // 각 방향별 오프셋
    public Vector2 leftOffset = new Vector2(-100, 0);
    public Vector2 rightOffset = new Vector2(100, 0);
    public Vector2 upOffset = new Vector2(0, 100);
    public Vector2 downOffset = new Vector2(0, -100);
    public int idx;

    void Update()
    {
        // 현재 비활성화된 캐릭터 가져오기
        int inactiveIndex = characterSwitchManager.GetInactiveCharacterIndex();
        Transform inactiveTarget = characterSwitchManager.GetInactiveCharacter();

        if (inactiveTarget == null || mainCamera == null || canvasRect == null) return;

        // 비활성화된 캐릭터의 월드 좌표를 뷰포트 좌표로 변환
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(inactiveTarget.position);

        // 각 말풍선을 비활성화
        foreach (var balloon in balloonRects)
            balloon.gameObject.SetActive(false);

        // 화면 밖에 있는 경우
        if (viewportPos.z > 0 && (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1))
        {
            // 화면 밖 방향 확인 및 적절한 RectTransform 선택
            RectTransform activeBalloon = null;
            Vector3 clampedViewportPos = viewportPos;
            Vector2 offset = Vector2.zero; // 최종 오프셋 값

            if (viewportPos.x < 0)
            {
                activeBalloon = balloonRects[0]; // Left
                idx = 0;
                clampedViewportPos.x = 0; // 화면 왼쪽
                offset = leftOffset; // 왼쪽 오프셋
            }
            else if (viewportPos.x > 1)
            {
                activeBalloon = balloonRects[1]; // Right
                idx = 1;
                clampedViewportPos.x = 1; // 화면 오른쪽
                offset = rightOffset; // 오른쪽 오프셋
            }
            if (viewportPos.y < 0)
            {
                activeBalloon = balloonRects[2]; // Up
                idx = 3;
                clampedViewportPos.y = 1; // 화면 아래
                offset = upOffset; // 아래 오프셋
            }
            else if (viewportPos.y > 1)
            {
                activeBalloon = balloonRects[3]; // Down
                idx = 2;
                clampedViewportPos.y = 0; // 화면 위
                offset = downOffset; // 위 오프셋
            }

            // 세부 위치 조정 (대각선 판별)
            if (activeBalloon != null)
            {
                if (viewportPos.x > 1 && viewportPos.y > 1)
                {
                    // 오른쪽 위
                    offset += new Vector2(50, 50); // 추가 오프셋
                }
                else if (viewportPos.x > 1 && viewportPos.y < 0)
                {
                    // 오른쪽 아래
                    offset += new Vector2(50, -50);
                }
                else if (viewportPos.x < 0 && viewportPos.y > 1)
                {
                    // 왼쪽 위
                    offset += new Vector2(-50, 50);
                }
                else if (viewportPos.x < 0 && viewportPos.y < 0)
                {
                    // 왼쪽 아래
                    offset += new Vector2(-50, -50);
                }

                activeBalloon.gameObject.SetActive(true);

                // 캐릭터 얼굴 설정
                faceImages[idx].sprite = characterFaces[inactiveIndex];

                // 클램핑된 뷰포트 좌표를 스크린 좌표로 변환
                Vector3 worldPosition = mainCamera.ViewportToWorldPoint(clampedViewportPos);

                // 스크린 좌표를 캔버스 좌표로 변환
                Vector2 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, mainCamera, out Vector2 localPosition);

                // 최종 오프셋 적용
                localPosition += offset;

                // 말풍선 위치 업데이트
                activeBalloon.anchoredPosition = localPosition;
            }
        }
    }
}

using UnityEngine;

public class ClueUIButton : MonoBehaviour
{
    [SerializeField] private GameObject panel; // 클릭 시 활성화할 패널
    [SerializeField] private PortalCurtain portalCurtain; // PortalCurtain 스크립트 참조

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭 감지
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.transform == transform) // 현재 오브젝트와 충돌 확인
            {
                if (panel != null)
                {
                    panel.SetActive(true); // 패널 활성화
                }

                // PortalCurtain의 함수 호출
                if (portalCurtain != null)
                {
                    portalCurtain.ActivateCurtain(); // PortalCurtain에서 처리 로직 실행
                }
            }
        }
    }
}

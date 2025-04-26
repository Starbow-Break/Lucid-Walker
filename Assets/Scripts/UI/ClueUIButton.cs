using UnityEngine;

public class ClueUIButton : MonoBehaviour
{
    [SerializeField] private GameObject panel; // 클릭 시 활성화할 패널
    [SerializeField] private MoviePortal moviePortal; // MoviePortal 스크립트 참조

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ❶ 마우스 스크린 좌표 → 월드 좌표 (z=0 평면으로 강제)
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos = new Vector2(wp.x, wp.y);  // z 버림

            // ❷ 한 점에서 모든 방향으로 쏘는 대신 OverlapPoint가 더 직관적
            Collider2D col = GetComponent<Collider2D>();     // 반드시 Collider2D 달려 있어야 함
            if (col != null && col.OverlapPoint(mousePos))
            {
                if (panel != null)
                    panel.SetActive(true);

                if (moviePortal != null)
                {
                    moviePortal.OnClueClicked();  // MoviePortal의 해당 메서드 호출
                }
            }


        }
    }

}

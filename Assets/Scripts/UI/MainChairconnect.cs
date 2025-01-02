using System.Collections;
using UnityEngine;

public class MainChairconnect : MonoBehaviour
{
    public GameObject panel; // 활성화할 UI 패널
    public GameObject connectUI; // 활성화할 연결 UI 오브젝트
    private Animator chairAnimator; // 의자 오브젝트의 Animator

    public Animator televisionAnimator; // 두 번째 애니메이터 참조
    public Animator televisionAnimator2;  // 세 번째 애니메이터 참조

    private bool isInteracting = false; // 상호작용 상태 체크

    private void Start()
    {
        // 의자 오브젝트의 Animator 가져오기
        chairAnimator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player 태그를 가진 오브젝트와 충돌 시 처리
        if (other.CompareTag("Player"))
        {
            StartCoroutine(WaitForKeyPress(other.gameObject));
        }
    }

    private IEnumerator WaitForKeyPress(GameObject player)
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (!isInteracting)
                {
                    // 상호작용 시작
                    isInteracting = true;

                    // 플레이어 비활성화
                    player.SetActive(false);

                    // 의자 애니메이션 실행
                    if (chairAnimator != null)
                    {
                        chairAnimator.SetBool("Connect", true);
                    }


                    // UI 패널 활성화
                    if (panel != null)
                    {
                        panel.SetActive(true);
                    }

                    // 애니메이터 활성화

                    televisionAnimator.SetBool("On", true);
                    televisionAnimator2.SetBool("On", true);

                    // 연결 UI 활성화
                    if (connectUI != null)
                    {
                        connectUI.SetActive(true);
                    }
                }
                else
                {
                    // 상호작용 종료
                    isInteracting = false;

                    // 플레이어 다시 활성화
                    player.SetActive(true);

                    // 의자 애니메이션 종료
                    if (chairAnimator != null)
                    {
                        chairAnimator.SetBool("Connect", false);
                    }

                    //  애니메이터 비활성화
                    televisionAnimator.SetBool("On", false);
                    televisionAnimator2.SetBool("On", false);

                    // UI 패널 비활성화
                    if (panel != null)
                    {
                        panel.SetActive(false);
                    }

                    // 연결 UI 비활성화
                    if (connectUI != null)
                    {
                        connectUI.SetActive(false);
                    }
                }
            }

            yield return null; // 매 프레임 대기
        }
    }
}

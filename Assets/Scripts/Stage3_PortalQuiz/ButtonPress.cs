using System.Collections;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public QuizManager quizManager; // QuizManager 참조
    public string portalName; // 포탈 이름 (예: "ㄱ_first", "ㄱ_second")
    public Animator buttonAnimator; // 버튼 애니메이터

    private bool isPlayerInTrigger = false; // 플레이어가 트리거 안에 있는지 확인

    private void Start()
    {
        // 버튼을 QuizManager에 등록
        if (quizManager != null)
        {
            quizManager.RegisterButton(portalName, this);
        }
    }

    private void Update()
    {
        // 플레이어가 트리거 안에 있을 때 Z 키 입력 감지
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.Z))
        {
            ActivateButton();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true; // 플레이어가 트리거 안에 들어옴
            Debug.Log("Player entered trigger area!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false; // 플레이어가 트리거를 떠남
            Debug.Log("Player exited trigger area!");
        }
    }

    private void ActivateButton()
    {
        // 버튼 애니메이터의 Activate 트리거 활성화
        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("Activate", true);
            Debug.Log("Button activated!");
        }

        // QuizManager 호출
        if (quizManager != null)
        {
            quizManager.NowPortal(portalName);
            Debug.Log($"QuizManager called with portal: {portalName}");
        }
    }

    public void DeactivateButton()
    {
        // 버튼 애니메이터의 Activate 트리거 비활성화
        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("Activate", false);
            Debug.Log($"Button {portalName} deactivated!");
        }
    }
}

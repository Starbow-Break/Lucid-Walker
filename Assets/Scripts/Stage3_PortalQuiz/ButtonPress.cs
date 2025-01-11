using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public QuizManager quizManager; // QuizManager 참조
    public string portalName; // 포탈 이름
    public Animator buttonAnimator; // 버튼 애니메이터
    public GameObject deactivateSwitch;
    public GameObject Purplelight; // Light 컴포넌트

    private bool isPlayerInTrigger = false; // 플레이어가 트리거 안에 있는지 확인
    private bool activated = false;

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
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.Z))
        {
            if (!activated)
                ActivateButton();
            else
                DeactivateButton();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
        }
    }

    private void ActivateButton()
    {
        deactivateSwitch.SetActive(false);

        // Light 활성화
        Purplelight.SetActive(true);
        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("Activate", true);
        }

        if (quizManager != null)
        {
            quizManager.NowPortal(portalName);
        }

        activated = true; // 버튼 활성화 상태 갱신
    }

    public void DeactivateButton()
    {
        deactivateSwitch.SetActive(true);

        Purplelight.SetActive(false);


        if (buttonAnimator != null)
        {
            buttonAnimator.SetBool("Activate", false);
            Debug.Log($"Button {portalName} deactivated!");
        }

        if (quizManager != null)
        {
            quizManager.DeactivatePortal(portalName);
        }

        activated = false; // 버튼 비활성화 상태 갱신
    }
}

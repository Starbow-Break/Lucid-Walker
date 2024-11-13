using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;
    public Transform focusPoint; // 대화 상대 (카메라가 포커스할 대상)
    private CameraFollow cameraFollow;

    private void Start()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 카메라를 대화 대상에 포커스
            if (cameraFollow != null && focusPoint != null)
            {
                cameraFollow.SetDialogueFocus(focusPoint);
            }

            // 대화 시작
            DialogueManager.Instance.StartDialogue(dialogueData);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 대화 종료 후 카메라를 플레이어로 포커스
            if (cameraFollow != null)
            {
                cameraFollow.ClearDialogueFocus();
            }

            DialogueManager.Instance.EndDialogue();
        }
    }
}

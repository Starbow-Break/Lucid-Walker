using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    private Queue<DialogueLine> lines; // 주고받는 대화를 큐로 관리
    private bool isDialogueActive = false; // 대화 활성 상태 확인

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 대화가 활성 상태일 때만 클릭을 통해 다음 대사로 넘어감
        if (isDialogueActive && Input.GetMouseButtonDown(0))
        {
            DisplayNextLine();
        }
    }

    public void StartDialogue(DialogueData dialogueData)
    {
        if (dialogueData == null || dialogueData.lines == null || dialogueData.lines.Count == 0)
        {
            Debug.LogError("Dialogue data or lines are missing.");
            return;
        }

        lines = new Queue<DialogueLine>(dialogueData.lines);
        isDialogueActive = true; // 대화 활성화
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (lines == null || lines.Count == 0 || DialogueUI.Instance == null)
        {
            EndDialogue(); // 대화가 끝나면 EndDialogue 호출
            return;
        }

        DialogueLine line = lines.Dequeue();
        DialogueUI.Instance.UpdateDialogueText(line.characterName, line.characterImage, line.sentence);
    }

    public void EndDialogue()
    {
        isDialogueActive = false; // 대화 비활성화
        DialogueUI.Instance.HideDialogueBox();
    }
    public bool IsDialogueFinished()
    {
        return !isDialogueActive; // 대화가 활성 상태가 아니면 끝난 것으로 간주
    }

}

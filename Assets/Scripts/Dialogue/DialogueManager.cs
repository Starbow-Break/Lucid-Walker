using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;
    public static DialogueManager Instance;

    private Queue<DialogueLine> lines; // 주고받는 대화를 큐로 관리
    private int dialogueStep = 0; // 현재 대화 단계 추적
    public delegate void DialogueEvent(int step);
    public event DialogueEvent OnDialogueStep; // 특정 단계에서 이벤트 트리거

    private bool isDialogueActive = false; // 대화 활성 상태 확인
    private bool isPaused = false; // 대화 일시 중지 여부 확인
    private Coroutine displayLineCoroutine;

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
        // 대화가 활성 상태이고 일시 중지 상태가 아닐 때만 진행
        if (isDialogueActive && !isPaused && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)))
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
        dialogueStep = 0; // 대화 단계 초기화
        isDialogueActive = true; // 대화 활성화
        isPaused = false;  // 대화 시작 시 일시 중지 해제
        DisplayNextLine();
    }

    public void DisplayNextLine()
    {
        if (isPaused) return; // 일시 중지 상태면 실행 안 함

        if (lines == null || lines.Count == 0 || DialogueUI.Instance == null)
        {
            EndDialogue(); // 대화가 끝나면 EndDialogue 호출
            return;
        }

        // 대기 중인 코루틴이 있다면 중단
        if (displayLineCoroutine != null)
        {
            StopCoroutine(displayLineCoroutine);
        }

        DialogueLine line = lines.Dequeue();

        // 대화 단계 이벤트 호출
        OnDialogueStep?.Invoke(dialogueStep);
        displayLineCoroutine = StartCoroutine(DisplayLine(line.sentence));

        DialogueUI.Instance.UpdateDialogueText(line.characterName, line.characterImage);
        dialogueStep++;
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

    private IEnumerator DisplayLine(string line)
    {
        // 텍스트를 한 글자씩 출력
        DialogueUI.Instance.SetDialogueText(""); // 텍스트 초기화

        foreach (char letter in line.ToCharArray())
        {
            DialogueUI.Instance.AppendDialogueText(letter.ToString());
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // 대화 일시 중지
    public void PauseDialogue()
    {
        isPaused = true;
    }

    // 대화 재개
    public void ResumeDialogue()
    {
        isPaused = false;
    }
}

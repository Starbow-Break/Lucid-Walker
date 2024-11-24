using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class QuizStep
    {
        public string answerName; // 정답 이름 (예: "ㄱ", "ㄴ", "ㅁ")
        public List<string> correctSequence; // 정답 순서 리스트
        public LineRenderer lineRenderer; // 정답 맞았을 때 활성화할 LineRenderer
    }

    public List<QuizStep> quizSteps; // 전체 퀴즈 리스트
    public GameObject finalPortal; // 최종 문 오브젝트
    public int maxSequenceLength = 4; // 저장 가능한 최대 정답 길이
    public Dictionary<string, ButtonPress> activeButtons = new Dictionary<string, ButtonPress>(); // 활성화된 버튼 관리

    private List<string> currentSequence = new List<string>(); // 현재 플레이어가 진행 중인 답
    private int currentQuizIndex = 0; // 현재 진행 중인 퀴즈 인덱스

    private void Start()
    {
        // 시작 시 모든 LineRenderer 비활성화
        foreach (var step in quizSteps)
        {
            if (step.lineRenderer != null)
            {
                step.lineRenderer.gameObject.SetActive(false);
            }
        }

        // 최종 문 비활성화
        if (finalPortal != null)
        {
            finalPortal.SetActive(false);
        }
    }

    public void NowPortal(string portalName)
    {
        // 새 포탈 추가
        currentSequence.Add(portalName);

        // 최대 길이 초과 시 첫 번째 값 제거
        if (currentSequence.Count > maxSequenceLength)
        {
            string removedPortal = currentSequence[0];
            currentSequence.RemoveAt(0);

            // 버튼 비활성화 처리
            if (activeButtons.ContainsKey(removedPortal))
            {
                activeButtons[removedPortal].DeactivateButton(); // 버튼 비활성화 호출
                activeButtons.Remove(removedPortal); // 딕셔너리에서 제거
            }
        }

        Debug.Log($"현재 시퀀스: {string.Join(", ", currentSequence)}");

        // 현재 퀴즈와 비교
        CheckCurrentSequence();
    }

    private void CheckCurrentSequence()
    {
        if (currentQuizIndex >= quizSteps.Count)
        {
            Debug.Log("모든 퀴즈 완료!");
            return;
        }

        QuizStep currentStep = quizSteps[currentQuizIndex];

        // 현재 입력이 정답과 일치하는지 확인
        if (IsSequenceCorrect(currentSequence, currentStep.correctSequence))
        {
            // 정답 맞춤
            ActivateLine(currentStep);
            currentSequence.Clear(); // 현재 시퀀스 초기화
            currentQuizIndex++; // 다음 퀴즈로 이동

            // 모든 퀴즈 완료 시 최종 문 활성화
            if (currentQuizIndex >= quizSteps.Count && finalPortal != null)
            {
                finalPortal.SetActive(true);
                Debug.Log("모든 퀴즈 완료! 최종 문 활성화.");
            }
        }
    }

    private bool IsSequenceCorrect(List<string> playerSequence, List<string> correctSequence)
    {
        if (playerSequence.Count != correctSequence.Count)
            return false;

        for (int i = 0; i < playerSequence.Count; i++)
        {
            if (playerSequence[i] != correctSequence[i])
                return false;
        }
        return true;
    }

    private void ActivateLine(QuizStep step)
    {
        if (step.lineRenderer != null)
        {
            step.lineRenderer.gameObject.SetActive(true);
            Debug.Log($"{step.answerName} 정답 맞춤! 라인 활성화됨.");
        }
    }

    public void RegisterButton(string portalName, ButtonPress button)
    {
        if (!activeButtons.ContainsKey(portalName))
        {
            activeButtons.Add(portalName, button); // 버튼 등록
        }
    }

    // 현재 스택 초기화 함수 추가
    public void ResetCurrentSequence()
    {
        // 모든 활성화된 버튼 비활성화
        foreach (var portalName in currentSequence)
        {
            if (activeButtons.ContainsKey(portalName))
            {
                activeButtons[portalName].DeactivateButton(); // 버튼 비활성화
            }
        }

        // 딕셔너리 비우기
        activeButtons.Clear();

        // 시퀀스 초기화
        currentSequence.Clear();

        Debug.Log("Current sequence reset and all buttons deactivated.");
    }
}

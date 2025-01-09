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
    public Dictionary<string, ButtonPress> activeButtons = new Dictionary<string, ButtonPress>(); // 활성화된 버튼 관리

    private List<string> currentSequence = new List<string>(); // 현재 플레이어가 진행 중인 답

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
    }

    public void NowPortal(string portalName)
    {
        // 포탈이 이미 시퀀스에 존재하면 추가하지 않음
        if (currentSequence.Contains(portalName))
        {
            Debug.Log($"중복된 포탈 이름 {portalName} - 추가하지 않음.");
            return;
        }

        // 새 포탈 추가
        currentSequence.Add(portalName);
        Debug.Log($"현재 시퀀스: {string.Join(", ", currentSequence)}");

        // 현재 퀴즈와 비교
        CheckCurrentSequence();
    }

    private void CheckCurrentSequence()
    {
        foreach (var step in quizSteps)
        {
            // 순서 상관없이 정답 확인
            if (IsSequenceCorrectIgnoringOrder(currentSequence, step.correctSequence))
            {
                // 정답 맞춤
                ActivateLine(step);
                RemoveSequence(step.correctSequence); // 정답 시퀀스를 currentSequence에서 제거
            }
        }

        // 모든 퀴즈 완료 시 최종 문 활성화
        if (quizSteps.TrueForAll(step => step.lineRenderer.gameObject.activeSelf) && finalPortal != null)
        {
            finalPortal.SetActive(true);
            Debug.Log("모든 퀴즈 완료! 최종 문 활성화.");
        }
    }

    private bool IsSequenceCorrectIgnoringOrder(List<string> playerSequence, List<string> correctSequence)
    {
        // correctSequence의 모든 요소가 playerSequence에 포함되어 있는지 확인
        foreach (var correct in correctSequence)
        {
            if (!playerSequence.Contains(correct))
            {
                return false;
            }
        }
        return true;
    }

    private void RemoveSequence(List<string> sequence)
    {
        // currentSequence에서 정답 시퀀스의 요소 제거
        foreach (var item in sequence)
        {
            currentSequence.Remove(item);
        }
        Debug.Log($"Updated sequence after removal: {string.Join(", ", currentSequence)}");
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
            activeButtons.Add(portalName, button);
            Debug.Log($"Button {portalName} registered.");
        }
    }

    public void DeactivatePortal(string portalName)
    {
        // 버튼 비활성화 처리
        if (activeButtons.ContainsKey(portalName))
        {
            activeButtons[portalName].DeactivateButton();
            activeButtons.Remove(portalName);
            currentSequence.Remove(portalName); // currentSequence에서 제거
            Debug.Log($"Portal {portalName} deactivated and removed from sequence.");
        }
    }

    // 현재 스택 초기화 함수 추가
    public void ResetCurrentSequence()
    {
        foreach (var portalName in currentSequence)
        {
            if (activeButtons.ContainsKey(portalName))
            {
                activeButtons[portalName].DeactivateButton();
            }
        }

        activeButtons.Clear();
        currentSequence.Clear();
        Debug.Log("Current sequence reset and all buttons deactivated.");
    }
}

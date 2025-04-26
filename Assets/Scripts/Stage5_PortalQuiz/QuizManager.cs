using System.Collections.Generic;
using UnityEngine;
using System.Collections;


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
    public GameObject chandelier;
    public float moveDownDistance = 9f; // 샹들리에가 내려갈 거리
    public float moveSpeed = 2f; // 샹들리에 이동 속도

    private Vector3 targetPosition; // 샹들리에의 목표 위치
    private bool isMoving = false; // 샹들리에가 움직이는 중인지 확인

    public Dictionary<string, ButtonPress> activeButtons = new Dictionary<string, ButtonPress>(); // 활성화된 버튼 관리
    private List<string> currentSequence = new List<string>(); // 현재 플레이어가 진행 중인 답
    public GameObject brokenChandelier;

    // 카메라 포커싱과 흔들림 효과
    public CameraFollow cameraFollow;
    public Transform focusPoint; // 대화 상대 (카메라가 포커스할 대상)
    public float cameraLerpSpeed = 2f; // 카메라 크기 변경 속도
    private int count = 0;
    public GameObject chandelierSmoke;
    public GameObject StageSucessDoor;


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

        // 샹들리에 초기 위치 설정
        targetPosition = chandelier.transform.position;
    }

    private void Update()
    {
        // 샹들리에 이동 처리
        if (isMoving)
        {
            chandelier.transform.position = Vector3.Lerp(chandelier.transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(chandelier.transform.position, targetPosition) < 0.01f)
            {
                chandelier.transform.position = targetPosition;
                isMoving = false;
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
            {
                if (IsSequenceCorrectIgnoringOrder(currentSequence, step.correctSequence))
                {
                    // 정답 맞춤
                    ActivateLine(step);
                    RemoveSequence(step.correctSequence); // 정답 시퀀스를 currentSequence에서 제거
                    if (count == 0)
                        MoveChandelierDown(); // 샹들리에 이동
                    count++;
                }
            }
            if (count == 2)
            {
                // 모든 퀴즈 완료 시 최종 문 활성화
                if (quizSteps.TrueForAll(step => step.lineRenderer.gameObject.activeSelf))
                {
                    moveSpeed = 5f;
                    MoveChandelierDown(); // 샹들리에 이동
                    QuizSuccess();
                }
            }
        }

    }

    private bool IsSequenceCorrectIgnoringOrder(List<string> playerSequence, List<string> correctSequence)
    {
        // 길이 먼저 비교
        if (playerSequence.Count != correctSequence.Count)
        {
            return false;
        }

        // 두 리스트를 정렬 후 요소 비교
        var sortedPlayerSequence = new List<string>(playerSequence);
        var sortedCorrectSequence = new List<string>(correctSequence);

        sortedPlayerSequence.Sort();
        sortedCorrectSequence.Sort();

        for (int i = 0; i < sortedPlayerSequence.Count; i++)
        {
            if (sortedPlayerSequence[i] != sortedCorrectSequence[i])
            {
                return false;
            }
        }

        return true;
    }

    private void QuizSuccess()
    {
        StartCoroutine(CameraEffect());
        Debug.Log("모든 퀴즈 성공!");
    }

    private IEnumerator CameraEffect()
    {
        yield return new WaitForSeconds(1f);
        // cameraFollow.TriggerShake();
        chandelierSmoke.SetActive(true);
        cameraFollow.SetDialogueFocus(focusPoint);
        yield return new WaitForSeconds(3f);
        chandelier.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        brokenChandelier.SetActive(true);
        StageSucessDoor.SetActive(true);
        yield return new WaitForSeconds(5f);
        cameraFollow.ClearDialogueFocus();
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

    private void MoveChandelierDown()
    {
        // 샹들리에의 목표 위치 설정
        targetPosition -= new Vector3(0, moveDownDistance, 0);
        isMoving = true; // 이동 시작
        Debug.Log("Chandelier is moving down!");
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
        currentSequence.Remove(portalName); // currentSequence에서 제거
        Debug.Log($"Portal {portalName} deactivated and removed from sequence.");
        CheckCurrentSequence();
    }

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

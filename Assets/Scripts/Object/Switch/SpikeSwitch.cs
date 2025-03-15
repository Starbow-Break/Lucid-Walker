using System.Collections;
using UnityEngine;
using Cinemachine;

public class SpikeSwitch : MonoBehaviour
{
    private Animator anim;
    private bool isPlayerInRange = false; // 플레이어가 트리거 안에 있는지 확인
    public GameObject spike;            // 움직일 스파이크 오브젝트

    // 스파이크가 이동할 오프셋과 이동에 걸리는 시간
    public float moveOffset = 0.5f;
    public float moveDuration = 0.2f;

    // 추가: 수평 이동 여부 플래그 (false면 기본적으로 위/아래 이동)
    public bool horizontalMovement = false;

    private Vector3 originalSpikePosition; // 스파이크의 원래 위치
    private Vector3 targetSpikePosition;   // 스파이크가 움직였을 때의 목표 위치

    // 명확한 변수 명칭: 그룹 카메라와 스파이크 집중 카메라
    public CinemachineVirtualCamera groupVCam;
    public CinemachineVirtualCamera focusVCam;

    private CinemachineBrain brain;

    // 우선순위 값 설정 (예: 그룹은 10, 집중은 40)
    public int normalPriority = 10;
    public int focusPriority = 40;

    // 상호작용하는 플레이어의 GameObject를 저장할 변수
    private GameObject interactingPlayer;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (Camera.main != null)
            brain = Camera.main.GetComponent<CinemachineBrain>();

        if (spike != null)
        {
            originalSpikePosition = spike.transform.position;
            // horizontalMovement 플래그에 따라 목표 위치 계산 (수평 또는 수직)
            targetSpikePosition = horizontalMovement ?
                originalSpikePosition + new Vector3(moveOffset, 0, 0) :
                originalSpikePosition + new Vector3(0, moveOffset, 0);
        }

        // 초기 상태: vCam이 할당된 경우에만 우선순위 설정
        if (groupVCam != null)
            groupVCam.Priority = normalPriority;
        if (focusVCam != null)
            focusVCam.Priority = normalPriority - 1; // 예: 9
    }

    private void Update()
    {
        // 플레이어가 트리거 내에 있고, Z키 입력 시 실행
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            bool isActive = false;
            if (interactingPlayer != null)
            {
                // PlayerController 또는 FemaleCharacterController 컴포넌트의 활성 상태 확인
                if (interactingPlayer.TryGetComponent<PlayerController>(out var pc))
                {
                    isActive = pc.isActive;
                }
                else if (interactingPlayer.TryGetComponent<FemaleCharacterController>(out var fc))
                {
                    isActive = fc.isActive;
                }
            }
            // 활성화된 플레이어가 아닐 경우 상호작용 무시
            if (!isActive)
                return;

            // 애니메이터의 "On" 상태 토글
            bool newState = !anim.GetBool("On");
            anim.SetBool("On", newState);

            // 스파이크 위치 이동 (켜지면 목표 위치로, 꺼지면 원래 위치로)
            if (newState)
            {
                StartCoroutine(MoveSpike(spike.transform.position, targetSpikePosition, moveDuration));
            }
            else
            {
                StartCoroutine(MoveSpike(spike.transform.position, originalSpikePosition, moveDuration));
            }

            // vCam이 하나라도 할당된 경우에만 카메라 전환 로직 실행
            if (groupVCam != null || focusVCam != null)
            {
                FocusOnTarget();
                StartCoroutine(RevertToGroupAfterDelay(1f));
            }
        }
    }

    // 스파이크 위치를 부드럽게 이동시키는 코루틴
    private IEnumerator MoveSpike(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            spike.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        spike.transform.position = endPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bool isActive = false;
            // PlayerController 또는 FemaleCharacterController 컴포넌트의 활성 상태 확인
            if (collision.TryGetComponent<PlayerController>(out var pc))
            {
                isActive = pc.isActive;
            }
            else if (collision.TryGetComponent<FemaleCharacterController>(out var fc))
            {
                isActive = fc.isActive;
            }
            if (isActive)
            {
                isPlayerInRange = true;
                interactingPlayer = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.gameObject == interactingPlayer)
        {
            isPlayerInRange = false;
            interactingPlayer = null;
        }
    }

    // 집중 카메라로 전환 (스파이크 집중)
    private void FocusOnTarget()
    {
        if (focusVCam != null)
            focusVCam.Priority = focusPriority; // 예: 40
        if (groupVCam != null)
            groupVCam.Priority = normalPriority; // 예: 10
    }

    // 지정 시간 후에 그룹 카메라로 복귀시키는 코루틴
    private IEnumerator RevertToGroupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (focusVCam != null)
            focusVCam.Priority = normalPriority - 1; // 예: 9
        if (groupVCam != null)
            groupVCam.Priority = normalPriority;     // 예: 10
    }
}

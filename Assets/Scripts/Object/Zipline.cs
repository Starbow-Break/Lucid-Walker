using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum ZiplineStatus {
    READY, // 출발 준비 상태
    MOVE, // 이동 상태
    ARRIVE, // 도착 상태
    RETURN // 복귀 상태
}

public class Zipline : MonoBehaviour
{
    [Header("Departures/Arrivals")]
    [SerializeField] Transform departures; // 출발 지점
    [SerializeField] Transform arrivals; // 도착 지점
    
    [Header("Wire")]
    [SerializeField] LineRenderer line; // 줄
    [SerializeField] float additionalLength; // 줄의 추가 여분 길이

    [Header("Trolley")]
    [SerializeField] GameObject trolley; // 플레이어가 잡는 부분
    [SerializeField] Transform playerAttachPoint; // 플레이어가 부착되는 위치

    [Header("Speed")]
    [SerializeField] float accelaration = 2.0f; // 가속도
    [SerializeField] float maxSpeed = 4.0f; // 최대 속력
    [SerializeField] float returnAccelaration = 1.0f; // 복귀 가속도
    [SerializeField] float returnMaxSpeed = 2.0f; // 복귀 최대 속력

    float velocity = 0.0f;
    ZiplineStatus status;
    Transform playerParent = null;
    PlayerController interactingPlayerController = null; // 상호작용중인 플레이어
    PlayerController attachingPlayerController = null; // 현재 부착중인 플레이어

    void Awake() {
        // Trolley 활성화 후 출발지로 이동
        trolley.gameObject.SetActive(true);
        trolley.transform.position = departures.position;

        // 출발지와 도착지의 자식 오브젝트들은 비활성화
        // 위치 정보는 필요하므로 자식 오브젝트들만 비 활성화 한다.
        for(int i = 0; i < departures.childCount; i++) {
            departures.GetChild(i).gameObject.SetActive(false);
        }
        for(int i = 0; i < arrivals.childCount; i++) {
            arrivals.GetChild(i).gameObject.SetActive(false);
        }

        // 초기 상태는 출발 준비 상태
        status = ZiplineStatus.READY;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 dir = (arrivals.position - departures.position).normalized;

        // 출발지 도착지에 맞춰서 줄 그리기
        line.positionCount = 2;
        line.startWidth = line.endWidth = 0.05f;
        line.SetPosition(0, departures.position - additionalLength * (Vector3)dir);
        line.SetPosition(1, arrivals.position + additionalLength * (Vector3)dir);
        line.useWorldSpace = true;
    }

    void Update()
    {
        switch(status) {
            case ZiplineStatus.READY: { // 준비 상태이면
                if(Input.GetKeyDown(KeyCode.Z) && interactingPlayerController != null) {
                    status = ZiplineStatus.MOVE;
                    AttachPlayer(interactingPlayerController);
                    Debug.Log("짚라인 이동!");
                }
                break;
            }
            case ZiplineStatus.MOVE: { // 이동 상태이면
                Move(arrivals, accelaration, maxSpeed);
                if(trolley.transform.position == arrivals.position) {
                    status = ZiplineStatus.ARRIVE;
                    velocity = 0.0f;
                    Debug.Log("짚라인 도착!");
                }
                break;
            }
            case ZiplineStatus.ARRIVE: { // 도착 상태이면
                if(Input.GetKeyDown(KeyCode.Z)) {
                    status = ZiplineStatus.RETURN;
                    DetachPlayer();
                    Debug.Log("짚라인 복귀!");
                }
                break;
            }
            case ZiplineStatus.RETURN: { // 복귀 상태이면
                Move(departures, returnAccelaration, returnMaxSpeed);
                if(trolley.transform.position == departures.position) {
                    status = ZiplineStatus.READY;
                    velocity = 0.0f;
                    Debug.Log("짚라인 준비!");
                }
                break;
            }
        }
    }

    void Move(Transform targetTransform, float acc, float maxSpd)
    {
        velocity = Mathf.Min(maxSpd, velocity + Time.deltaTime * acc);

        Vector2 remain = targetTransform.position - trolley.transform.position;
        Vector2 delta = velocity * remain.normalized;
        Vector2 newPosition = remain.sqrMagnitude <= delta.sqrMagnitude 
                            ? targetTransform.position 
                            : trolley.transform.position + (Vector3)delta;
        
        trolley.transform.position = new(newPosition.x, newPosition.y, 0.0f);
    }

    void AttachPlayer(PlayerController pc)
    {
        GameObject playerObj = pc.gameObject;
        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
        attachingPlayerController = pc;

        playerParent = playerObj.transform.parent;
        playerObj.transform.parent = trolley.transform;
        playerObj.transform.localPosition = playerAttachPoint.localPosition;

        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        pc.enabled = false;
    }

    void DetachPlayer()
    {
        GameObject playerObj = attachingPlayerController.gameObject;
        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();

        playerObj.transform.parent = playerParent;
        playerParent = null;

        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        attachingPlayerController.enabled = true;
        attachingPlayerController = null;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            interactingPlayerController = other.GetComponent<PlayerController>();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player")) {
            interactingPlayerController = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

enum ZiplineStatus {
    // 초기, 출발 준비, 이동 중, 도착, 복귀
    INIT, READY, MOVE, ARRIVE, RETURN
}

public class Zipline : MonoBehaviour
{
    [Header("Trunk")]
    [SerializeField] Transform departuresWireAttachPoint; // 출발 지점 기둥에 와이어가 부착되는 위치
    [SerializeField] Transform arrivalsWireAttachPoint; // 도착 지점 기둥에 와이어가 부착되는 위치
    
    [Header("Wire")]
    [SerializeField] LineRenderer line; // 줄
    [SerializeField] float thickness = 0.2f; // 두께

    [Header("Trolley")]
    [SerializeField] float departurePositionOffset; // 출발 위치 오프셋
    [SerializeField] float arrivalPositionOffset; // 도착 위치 오프셋
    [SerializeField] Trolley trolley; // 플레이어가 잡는 부분
    [SerializeField] Transform playerAttachPoint; // 플레이어가 부착되는 위치

    [Header("Speed")]
    [SerializeField] float accelaration = 2.0f; // 가속도
    [SerializeField] float maxSpeed = 4.0f; // 최대 속력
    [SerializeField] float returnAccelaration = 1.0f; // 복귀 가속도
    [SerializeField] float returnMaxSpeed = 2.0f; // 복귀 최대 속력

    Vector3 departures, arrivals;
    float velocity = 0.0f;
    ZiplineStatus status;
    Transform playerParent = null;
    PlayerController attachingPlayerController = null; // 현재 부착중인 플레이어

    public bool isRightDir => arrivals.x - departures.x >= 0;

    void OnValidate() {
        if(thickness > 0.0f) {
            DrawWire(thickness);
        }

        if(departuresWireAttachPoint != null && arrivalsWireAttachPoint != null) {
            float wireLen = Vector3.Distance(departuresWireAttachPoint.position, arrivalsWireAttachPoint.position);
            departurePositionOffset = Mathf.Clamp(departurePositionOffset, 0.0f, wireLen);
            arrivalPositionOffset = Mathf.Clamp(arrivalPositionOffset, 0.0f, wireLen);
        }
    }

    void Awake() {
        Vector3 dir = (arrivalsWireAttachPoint.position - departuresWireAttachPoint.position).normalized;

        // 출발지, 도착지 계산
        departures = departuresWireAttachPoint.position + dir * departurePositionOffset;
        arrivals = arrivalsWireAttachPoint.position - dir * arrivalPositionOffset;

        status = ZiplineStatus.INIT;
    }

    // Start is called before the first frame update
    void Start()
    {
        DrawWire(thickness);
    }

    void Update()
    {
        switch(status) {
            case ZiplineStatus.MOVE: { // 이동 상태이면
                Move(arrivals, accelaration, maxSpeed);
                if(trolley.transform.position == arrivals) {
                    status = ZiplineStatus.ARRIVE;
                    velocity = 0.0f;
                    Debug.Log("짚라인 도착!");
                }
                break;
            }
            case ZiplineStatus.ARRIVE: { // 도착 상태이면
                DetachPlayer();
                status = ZiplineStatus.RETURN;
                Debug.Log("짚라인 복귀!");
                break;
            }
            case ZiplineStatus.RETURN: { // 복귀 상태이면
                Move(departures, returnAccelaration, returnMaxSpeed);
                if(trolley.transform.position == departures) {
                    status = ZiplineStatus.READY;
                    velocity = 0.0f;
                    Debug.Log("짚라인 준비!");
                }
                break;
            }
        }
    }

    void DrawWire(float _thickness) {
        // 출발지 도착지에 맞춰서 줄 그리기
        line.positionCount = 2;
        line.startWidth = line.endWidth = _thickness;
        line.SetPosition(0, departuresWireAttachPoint.position);
        line.SetPosition(1, arrivalsWireAttachPoint.position);
        line.useWorldSpace = true;
    }

    public Trolley GetTargetTrolley() {
        return trolley;
    }
    
    public void SetTrolley(ItemFollowBag bag, Trolley trolley) {
        StartCoroutine(SetTrolleyFlow(bag, trolley));
    }

    IEnumerator SetTrolleyFlow(ItemFollowBag bag, Trolley trolley) {
        bag.RemoveItem(trolley);

        trolley.isFollow = true;
        trolley.FollowTarget(departures);
        while((departures - trolley.transform.position).sqrMagnitude >= 0.001f)
        {
            yield return null;
        }

        // 옷걸이 위치 정확히 맞추고 준비 상태로 변경
        trolley.transform.SetParent(transform);
        trolley.transform.position = departures;
        status = ZiplineStatus.READY;
        trolley.isFollow = false;
        trolley.SetZipline(this);
        yield return null;
    }

    void Move(Vector3 targetPosition, float acc, float maxSpd)
    {
        velocity = Mathf.Min(maxSpd, velocity + Time.deltaTime * acc);

        Vector2 remain = targetPosition - trolley.transform.position;
        Vector2 delta = velocity * Time.deltaTime * remain.normalized;
        Vector2 newPosition = remain.sqrMagnitude <= delta.sqrMagnitude 
                            ? targetPosition
                            : trolley.transform.position + (Vector3)delta;
        
        trolley.transform.position = new(newPosition.x, newPosition.y, 0.0f);
    }

    // 플레이어 탑승
    public void BoardPlayer(PlayerController pc) {
        if(status == ZiplineStatus.READY) { // 짚라인이 준비 상태라면 플레이어 탑승
            status = ZiplineStatus.MOVE;
            if(pc.IsFacingRight != isRightDir) {
                pc.Turn();
            }
            AttachPlayer(pc);
            Debug.Log("짚라인 이동!");
        }
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

        pc.SetZiplineAnim(true);
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
        attachingPlayerController.SetZiplineAnim(false);
        attachingPlayerController = null;
    }
}

using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Transform target;
    private Transform originalTarget;   // 원래의 타겟을 저장해둠


    public float shakeDuration = 0.5f;  // 흔들림 지속 시간
    public float shakeAmount = 0.1f;    // 흔들림 강도
    public float decreaseFactor = 1.0f; // 흔들림 감소 속도
    public float followSpeed = 0.1f; // Lerp를 사용할 때 이동 속도


    [Header("Padding")]
    [SerializeField] float paddingLeft = 0.0f;
    [SerializeField] float paddingRight = 0.0f;
    [SerializeField] float paddingTop = 0.0f;
    [SerializeField] float paddingBottom = 0.0f;

    private Vector3 originalPos;
    private float currentShakeDuration = 0f;
    private TilemapRenderer tr;

    void Start()
    {
        originalPos = transform.position;  // 카메라의 원래 위치 저장

        if (tilemap != null)
        {
            tr = tilemap.gameObject.GetComponent<TilemapRenderer>();
        }


        if (target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
            originalTarget = target;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 카메라 범위의 너비, 높이 계산
        Camera camera = gameObject.GetComponent<Camera>();
        Vector3 bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = camera.ScreenToWorldPoint(new(Screen.width, Screen.height));
        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;

        Vector3 followPosition;

        if (tr != null) // 타일맵을 설정했다면 우선 적용 (추후 제거 예정)
        {
            followPosition = new Vector3(
                Mathf.Clamp(target.position.x, tr.bounds.min.x + width / 2 + paddingLeft, tr.bounds.max.x - width / 2 - paddingRight),
                Mathf.Clamp(target.position.y, tr.bounds.min.y + height / 2 + paddingBottom, tr.bounds.max.y - height / 2 - paddingTop),
                transform.position.z
            );
        }
        else
        {
            Map currentMap = FindMap(target);
            if (currentMap != null)
            { // Map이 있다면 해당 Map에 설정된 경계 안에서 이동
                followPosition = new Vector3(
                    Mathf.Clamp(target.position.x, currentMap.boundMin.x + width / 2, currentMap.boundMax.x - width / 2),
                    Mathf.Clamp(target.position.y, currentMap.boundMin.y + height / 2, currentMap.boundMax.y - height / 2),
                    transform.position.z
                );
            }
            else
            { // 아니면 범위 제한 X
                followPosition = new(target.position.x, target.position.y, transform.position.z);
            }
        }

        // 흔들림이 있을 경우 적용
        if (currentShakeDuration > 0)
        {
            // 플레이어를 따라가면서 흔들림을 적용
            transform.position = followPosition + Random.insideUnitSphere * shakeAmount;
            currentShakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            // 흔들림이 끝나면 플레이어의 위치에 고정
            currentShakeDuration = 0f;
            transform.position = followPosition;
        }
    }

    private Map FindMap(Transform t) {
        while(t != null) {
            Map map = t.GetComponent<Map>();
            if(map != null) {
                return map;
            }
            t = t.parent;
        }

        return null;
    }

    // 카메라 흔들림을 시작하는 함수
    public void TriggerShake()
    {
        currentShakeDuration = shakeDuration;
        originalPos = new Vector3(target.position.x, target.position.y, transform.position.z);  // 현재 카메라 위치 저장
    }

    // tilemap 참조 변경 함수
    public void SetTileMap(Tilemap newTilemap)
    {
        tilemap = newTilemap;
        if (tilemap != null)
        {
            tr = tilemap.gameObject.GetComponent<TilemapRenderer>();
        }
    }

    // 카메라 타겟 변경 메서드
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (newTarget != null)
        {
            originalTarget = newTarget; // 원래 타겟 업데이트
        }
    }

    // 대화 시 카메라 타겟을 대화 대상에 맞추는 메서드
    public void SetDialogueFocus(Transform newFocusTarget)
    {
        if (newFocusTarget != null)
        {
            target = newFocusTarget;  // 대화 상대를 타겟으로 설정
        }
    }

    // 대화 종료 시 카메라 타겟을 플레이어로 되돌림
    public void ClearDialogueFocus()
    {
        target = originalTarget;  // 플레이어를 다시 타겟으로 설정
    }
}

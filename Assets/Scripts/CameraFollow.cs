using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Transform target;

    public float shakeDuration = 0.5f;  // 흔들림 지속 시간
    public float shakeAmount = 0.1f;    // 흔들림 강도
    public float decreaseFactor = 1.0f; // 흔들림 감소 속도

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

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        // 카메라 범위의 너비, 높이 계산
        Camera camera = gameObject.GetComponent<Camera>();
        Vector3 bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = camera.ScreenToWorldPoint(new(Screen.width, Screen.height));
        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;

        Vector3 followPosition;

        if (tr != null) // 타일맵을 설정했다면 우선 적용
        {
            followPosition = new Vector3(
                Mathf.Clamp(target.position.x, tr.bounds.min.x + width / 2 + paddingLeft, tr.bounds.max.x - width / 2 - paddingRight),
                Mathf.Clamp(target.position.y, tr.bounds.min.y + height / 2 + paddingBottom, tr.bounds.max.y - height / 2 - paddingTop),
                transform.position.z
            );
        }
        else
        {
            Map currentMap = target.parent.GetComponent<Map>();
            if(currentMap != null) { // Map이 있다면 해당 Map에 설정된 경계 안에서 이동
                followPosition = new Vector3(
                    Mathf.Clamp(target.position.x, currentMap.boundMin.x + width / 2, currentMap.boundMax.x - width / 2),
                    Mathf.Clamp(target.position.y, currentMap.boundMin.y + height / 2, currentMap.boundMax.y - height / 2),
                    transform.position.z
                );
            }
            else { // 아니면 범위 제한 X
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

    // 카메라 흔들림을 시작하는 함수
    public void TriggerShake()
    {
        currentShakeDuration = shakeDuration;
        originalPos = new Vector3(target.position.x, target.position.y, transform.position.z);  // 현재 카메라 위치 저장
    }
    public void SetTarget(Tilemap newTilemap)
    {
        tilemap = newTilemap;
        tr = tilemap != null ? tilemap.GetComponent<TilemapRenderer>() : null;
    }
}

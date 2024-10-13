using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Transform target;
    public float shakeDuration = 0.5f;  // 흔들림 지속 시간
    public float shakeAmount = 0.1f;    // 흔들림 강도
    public float decreaseFactor = 1.0f; // 흔들림 감소 속도

    private Vector3 originalPos;
    private float currentShakeDuration = 0f;
    private TilemapRenderer tr;

    void Start()
    {
        originalPos = transform.position;  // 카메라의 원래 위치 저장

        if(tilemap != null) {
            tr = tilemap.gameObject.GetComponent<TilemapRenderer>();
        }

        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    void FixedUpdate()
    {
        Vector3 followPosition;

        if(tr != null) {
            Camera camera = gameObject.GetComponent<Camera>();
            
            Vector3 bottomLeft = camera.ScreenToWorldPoint(Vector3.zero);
            Vector3 topRight = camera.ScreenToWorldPoint(new(Screen.width, Screen.height));

            float width = topRight.x - bottomLeft.x;
            float height = topRight.y - bottomLeft.y;

            // 플레이어를 따라가는 기본 위치 설정
            followPosition = new Vector3(
                Mathf.Clamp(target.position.x, tr.bounds.min.x + width / 2, tr.bounds.max.x - width / 2), 
                Mathf.Clamp(target.position.y, tr.bounds.min.y + height / 2, tr.bounds.max.x - height / 2), 
                transform.position.z
            );
        }
        else {
            followPosition = new(target.position.x, target.position.y, transform.position.z);
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
}

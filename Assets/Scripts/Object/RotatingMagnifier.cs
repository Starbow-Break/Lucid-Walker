using UnityEngine;

public class RotatingMagnifier : MonoBehaviour
{
    [SerializeField] private Transform pivot; // 회전 중심
    [SerializeField] private float rotationSpeed = 50f; // 회전 속도
    [SerializeField] private float radius = 1f; // 회전 반지름

    private Vector3 initialPosition; // 초기 위치 저장
    private Quaternion fixedRotation; // 고정된 회전값 저장

    private void Start()
    {
        // 회전 중심 설정
        if (pivot == null)
        {
            Debug.LogError("Pivot Transform이 설정되지 않았습니다!");
            return;
        }

        // 초기 위치와 회전값 저장
        initialPosition = transform.position;
        fixedRotation = transform.rotation;
    }

    private void Update()
    {
        if (pivot != null)
        {
            // 돋보기를 pivot 기준으로 회전
            transform.RotateAround(pivot.position, Vector3.forward, rotationSpeed * Time.deltaTime);

            // 회전 반지름 유지
            Vector3 direction = (transform.position - pivot.position).normalized;
            transform.position = pivot.position + direction * radius;

            // 회전값 고정
            transform.rotation = fixedRotation;
        }
    }
}

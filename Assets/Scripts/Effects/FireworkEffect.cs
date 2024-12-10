using UnityEngine;

public class FireworkEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem; // 폭죽 파티클 시스템
    [SerializeField] private GameObject stageClearUI; // 스테이지 클리어 UI
    [SerializeField] private float targetAngle = 28.0f; // 최종 목표 Angle
    [SerializeField] private float angleChangeSpeed = 10.0f; // Angle 변경 속도

    private bool isPlaying = false;

    void Update()
    {
        // stageClearUI가 활성화되었는지 확인
        if (stageClearUI.activeSelf && !isPlaying)
        {
            StartFireworkEffect();
        }

        // 폭죽 효과 업데이트
        if (isPlaying)
        {
            UpdateFireworkAngle();
        }
    }

    private void StartFireworkEffect()
    {
        // 파티클 재생
        particleSystem.Play();
        isPlaying = true;

        // 시작 Angle을 0으로 설정
        var shape = particleSystem.shape;
        shape.angle = 0f;
    }

    private void UpdateFireworkAngle()
    {
        // 현재 Shape 모듈 가져오기
        var shape = particleSystem.shape;

        // 목표 Angle로 점진적으로 변경
        if (shape.angle < targetAngle)
        {
            shape.angle += angleChangeSpeed * Time.deltaTime;

            // 목표 각도에 도달하면 멈추기
            if (shape.angle >= targetAngle)
            {
                shape.angle = targetAngle;
                isPlaying = false; // 변경 멈춤
            }
        }
    }
}

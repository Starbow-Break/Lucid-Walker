using UnityEngine;

public class WaterController : MonoBehaviour
{
    [SerializeField] private Transform waterTransform; // 물의 Transform
    [SerializeField] private float maxScaleY = 12.8f; // 물의 최대 높이
    [SerializeField] private float fillSpeed = 0.5f; // 물이 차오르는 속도

    private float targetScaleY = 0.0f; // 목표 Y 스케일
    private bool isFilling = false; // 물이 차오르는 중인지 여부
    private ParticleSystem currentParticle; // 현재 활성화된 파티클

    private void Start()
    {
        // 물의 초기 높이 설정
        if (waterTransform != null)
        {
            targetScaleY = waterTransform.localScale.y;
        }
    }

    // 레버가 작동될 때 호출되는 메서드
    public void IncreaseWaterLevel(int increment, ParticleSystem leverParticle)
    {
        // 물 높이 증가
        if (waterTransform != null && targetScaleY < maxScaleY)
        {
            targetScaleY = Mathf.Min(targetScaleY + increment, maxScaleY);
            isFilling = true;

            // 현재 활성화할 파티클 설정
            currentParticle = leverParticle;

            // 파티클 실행
            if (currentParticle != null && !currentParticle.isPlaying)
            {
                currentParticle.Play();
            }
        }
    }

    private void Update()
    {
        // 물이 목표 높이에 도달할 때까지 차오름
        if (isFilling && waterTransform.localScale.y < targetScaleY)
        {
            float newScaleY = Mathf.MoveTowards(waterTransform.localScale.y, targetScaleY, fillSpeed * Time.deltaTime);
            waterTransform.localScale = new Vector3(waterTransform.localScale.x, newScaleY, waterTransform.localScale.z);
        }
        else if (isFilling && waterTransform.localScale.y >= targetScaleY)
        {
            // 목표 높이에 도달하면 파티클 비활성화 및 차오름 종료
            isFilling = false;

            if (currentParticle != null && currentParticle.isPlaying)
            {
                currentParticle.Stop();
                currentParticle = null; // 현재 파티클 해제
            }
        }
    }
}

using UnityEngine;

public class WaterController : MonoBehaviour
{
    [SerializeField] private Transform waterTransform; // 물의 Transform
    [SerializeField] private ParticleSystem waterParticle; // 물이 나오는 파티클
    [SerializeField] private float maxScaleY = 12.8f; // 물의 최대 높이
    [SerializeField] private float scaleIncrement = 3.0f; // 루버를 돌릴 때마다 증가하는 목표 Y 스케일
    [SerializeField] private float fillSpeed = 0.5f; // 물이 차오르는 속도

    private float targetScaleY = 0.0f; // 목표 Y 스케일
    private bool isFilling = false; // 물이 차오르는 중인지 여부

    private void Start()
    {
        // 초기 상태에서 파티클 비활성화
        if (waterParticle != null)
        {
            waterParticle.Stop();
        }

        // 물의 초기 높이 설정
        if (waterTransform != null)
        {
            targetScaleY = waterTransform.localScale.y;
        }
    }

    // 루버를 돌릴 때 호출될 메서드
    public void IncreaseWaterLevel()
    {
        // 파티클 활성화 및 목표 높이 설정
        if (waterParticle != null && !waterParticle.isPlaying)
        {
            waterParticle.Play();
        }

        // 목표 높이를 설정하고 차오르기 시작
        if (waterTransform != null && targetScaleY < maxScaleY)
        {
            targetScaleY = Mathf.Min(targetScaleY + scaleIncrement, maxScaleY);
            isFilling = true; // 물이 차오르는 중으로 설정
        }
    }

    private void Update()
    {
        // 물이 목표 높이에 도달할 때까지 천천히 차오름
        if (isFilling && waterTransform.localScale.y < targetScaleY)
        {
            float newScaleY = Mathf.MoveTowards(waterTransform.localScale.y, targetScaleY, fillSpeed * Time.deltaTime);
            waterTransform.localScale = new Vector3(waterTransform.localScale.x, newScaleY, waterTransform.localScale.z);
        }
        else if (isFilling && waterTransform.localScale.y >= targetScaleY)
        {
            // 목표 높이에 도달하면 파티클을 비활성화하고 차오름 종료
            isFilling = false;
            if (waterParticle != null && waterParticle.isPlaying)
            {
                waterParticle.Stop();
            }
        }
    }
}

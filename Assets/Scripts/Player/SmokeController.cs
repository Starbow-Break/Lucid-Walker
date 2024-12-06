using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeController : MonoBehaviour
{
    public ParticleSystem defaultSmokeEffect; // 기본 연기 효과
    public ParticleSystem swimmingSmokeEffect; // 수영 시 연기 효과
    private Rigidbody2D rb;
    private float minSpeedToEmit = 0.1f; // 연기 생성 최소 속도
    private bool isInWater = false; // 물 안에 있는지 여부

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultSmokeEffect.Stop(); // 초기 상태에서 기본 연기 비활성화
        swimmingSmokeEffect.Stop(); // 초기 상태에서 수영 연기 비활성화
    }

    void Update()
    {
        if (isInWater)
        {
            HandleSwimmingSmoke();
        }
        else
        {
            HandleDefaultSmoke();
        }
    }

    private void HandleDefaultSmoke()
    {
        if (swimmingSmokeEffect.isPlaying)
        {
            swimmingSmokeEffect.Stop(); // 수영 연기 중지
        }

        float speed = Mathf.Abs(rb.velocity.x);

        if (speed > minSpeedToEmit)
        {
            if (!defaultSmokeEffect.isPlaying)
                defaultSmokeEffect.Play();

            var emission = defaultSmokeEffect.emission;
            emission.rateOverTime = speed * 10; // 속도에 따라 빈도 조절
        }
        else
        {
            if (defaultSmokeEffect.isPlaying)
                defaultSmokeEffect.Stop();
        }
    }

    private void HandleSwimmingSmoke()
    {
        if (defaultSmokeEffect.isPlaying)
        {
            defaultSmokeEffect.Stop(); // 기본 연기 중지
        }

        if (!swimmingSmokeEffect.isPlaying)
        {
            swimmingSmokeEffect.Play(); // 수영 연기 활성화
        }

        // 수영 연기는 속도에 따라 빈도를 조절하지 않고 고정값으로 설정 가능
        var emission = swimmingSmokeEffect.emission;
        emission.rateOverTime = 5; // 고정된 수영 연기 빈도
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isInWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isInWater = false;
        }
    }
}

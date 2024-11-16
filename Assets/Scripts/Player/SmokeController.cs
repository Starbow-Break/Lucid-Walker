using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SmokeController : MonoBehaviour
{
    public ParticleSystem smokeEffect; // 연기 효과를 위한 Particle System
    private Rigidbody2D rb;
    private float minSpeedToEmit = 0.1f; // 연기 생성 최소 속도
    private bool isInWater = false; // 물 안에 있는지 여부

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        smokeEffect.Stop(); // 초기 상태에서 연기 비활성화
    }

    void Update()
    {
        // 물 안에서는 연기 중지
        if (isInWater)
        {
            if (smokeEffect.isPlaying)
                smokeEffect.Stop();
            return;
        }

        // 캐릭터의 속도 체크
        float speed = Mathf.Abs(rb.velocity.x);

        if (speed > minSpeedToEmit)
        {
            // 캐릭터가 움직이고 있으면 연기 효과를 활성화
            if (!smokeEffect.isPlaying)
                smokeEffect.Play();

            // 속도에 따라 연기 빈도 조절
            var emission = smokeEffect.emission;
            emission.rateOverTime = speed * 10;
        }
        else
        {
            // 캐릭터가 멈추면 연기 효과를 중지
            if (smokeEffect.isPlaying)
                smokeEffect.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 물에 닿았을 때
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isInWater = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 물에서 나왔을 때
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isInWater = false;
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class LeverController : MonoBehaviour
{
    [SerializeField] private WaterController waterController; // 공유되는 WaterController
    [SerializeField] private Animator leverAnimator; // 레버 애니메이터
    [SerializeField] private ParticleSystem leverParticle; // 레버별 고유 파티클
    [SerializeField] private int waterIncreaseAmount = 4; // 레버 당 증가하는 물 높이

    public bool isActivated = false; // 레버가 이미 작동했는지 여부

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                // 레버 애니메이션 실행
                if (leverAnimator != null)
                {
                    leverAnimator.SetTrigger("Rotate");

                    // 레버 파티클 실행
                    if (leverParticle != null && !leverParticle.isPlaying)
                    {
                        leverParticle.Play();
                    }

                    // 애니메이션 길이에 맞게 레버 작동 상태 설정
                    StartCoroutine(ActivateLever());
                }
                else
                {
                    Debug.LogWarning("Lever Animator is not assigned!");
                }
            }
        }
    }

    private IEnumerator ActivateLever()
    {
        // 애니메이션 실행 시간 동안 대기
        yield return new WaitForSeconds(leverAnimator.GetCurrentAnimatorStateInfo(0).length);

        // 공유된 WaterController를 사용해 물 높이 증가 호출 (레버 파티클도 전달)
        if (waterController != null)
        {
            waterController.IncreaseWaterLevel(waterIncreaseAmount, leverParticle);
        }

        // 레버가 작동된 상태로 설정
        isActivated = true;
    }
}
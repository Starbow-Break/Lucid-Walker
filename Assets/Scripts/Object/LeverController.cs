using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeverController : MonoBehaviour
{
    [SerializeField] private WaterController waterController;
    [SerializeField] private Animator leverAnimator; // 레버 애니메이터

    private bool isActivated = false; // 레버가 이미 작동했는지 여부

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                // 레버 회전 애니메이션 실행
                if (leverAnimator != null)
                {
                    leverAnimator.SetTrigger("Rotate");

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
        // 애니메이션이 실행될 시간을 기다림
        yield return new WaitForSeconds(leverAnimator.GetCurrentAnimatorStateInfo(0).length);

        // 물 높이 증가
        if (waterController != null)
        {
            waterController.IncreaseWaterLevel();
        }

        // 레버가 작동된 상태로 설정
        isActivated = true;
    }
}

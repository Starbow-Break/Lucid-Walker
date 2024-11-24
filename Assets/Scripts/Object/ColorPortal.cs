using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPortal : MonoBehaviour
{
    public QuizManager quizManager; // QuizManager 참조
    public Transform targetPosition;
    public ParticleSystem playerParticleSystem;
    public Color portalParticleColor;
    public GameObject tileMapToDeactivate;
    public List<Collider2D> collidersToDisable;
    public float targetCameraSize = 7.5f;
    public float cameraLerpSpeed = 2f;
    public MoviePortal moviePortal; // MoviePortal 참조

    private float originalCameraSize;
    private bool isPlayerInPortal = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true;
            StartCoroutine(WaitForKeyPress(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false;
            StopCoroutine(WaitForKeyPress(other));
        }
    }

    private IEnumerator WaitForKeyPress(Collider2D player)
    {
        while (isPlayerInPortal)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                TeleportPlayer(player);
                yield break;
            }
            yield return null;
        }
    }

    private void TeleportPlayer(Collider2D player)
    {
        if (targetPosition != null)
        {
            player.transform.position = targetPosition.position;
        }

        if (playerParticleSystem != null)
        {
            var mainModule = playerParticleSystem.main;
            mainModule.startColor = portalParticleColor;
        }

        if (targetPosition != null && targetPosition.CompareTag("PortalEnd"))
        {
            if (tileMapToDeactivate != null)
            {
                tileMapToDeactivate.SetActive(false);
            }

            // 플레이어의 Order in Layer를 0으로 설정
            SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
            if (playerSprite != null)
            {
                playerSprite.sortingOrder = 10;
            }

            foreach (Collider2D collider in collidersToDisable)
            {
                collider.enabled = false;
            }

            // MoviePortal의 카메라 크기 변경 코루틴 종료
            if (moviePortal != null)
            {
                moviePortal.StopCameraSizeChange();
                moviePortal.StartCameraSizeChange(targetCameraSize);
            }

            if (quizManager != null)
            {
                quizManager.ResetCurrentSequence();
            }
        }
    }

}

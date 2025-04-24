using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraBoundSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera ActiveCamera;
    public PolygonCollider2D NewBoundingShape;
    public Transform TeleportDestination; // New teleport destination
    public PlayerController Player; // Reference to the player object
    public CircleWipe CircleWipeEffect; // Reference to CircleWipe for transitions

    private Vector2 triggerPosition; // Trigger position
    private bool isSwitching = false; // Prevents duplicate triggers
    private float switchCooldown = 0.2f; // Cooldown duration

    private void Start()
    {
        // Store the current trigger object's position
        triggerPosition = transform.position;
        if (CircleWipeEffect == null)
        {
            CircleWipeEffect = LevelManager.Circle;
            if (CircleWipeEffect == null)
                Debug.LogError("CircleWipeEffect를 찾을 수 없습니다!");
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (other.CompareTag("Player"))
        {
            bool isPlayerMovingRight = player.transform.position.x > triggerPosition.x;

            if ((!isPlayerMovingRight && player.IsFacingRight) || (isPlayerMovingRight && !player.IsFacingRight))
            {
                if (!isSwitching)
                {
                    bool vertical = false;
                    bool isEntering = isPlayerMovingRight;

                    CircleWipeEffect.PlayTransition(vertical, isEntering);

                    StartCoroutine(TeleportWithEffect(isPlayerMovingRight));
                }
            }
        }
    }

    private IEnumerator TeleportWithEffect(bool isPlayerMovingRight)
    {
        isSwitching = true;

        yield return new WaitForSeconds(1f);

        // 텔레포트

        Player.transform.position = TeleportDestination.position;

        SwitchCameraBound();

        StartCoroutine(TriggerCooldown());
    }

    private IEnumerator TriggerCooldown()
    {
        yield return new WaitForSeconds(switchCooldown);
        isSwitching = false;
    }

    public void SwitchCameraBound()
    {
        CinemachineConfiner2D _cinemachineConfinder = ActiveCamera.GetComponent<CinemachineConfiner2D>();
        _cinemachineConfinder.m_BoundingShape2D = NewBoundingShape;
        _cinemachineConfinder.InvalidateCache();
    }
}

using UnityEngine;

public class CharacterSwitchManager : MonoBehaviour
{
    public PlayerController playerController; // 플레이어 컨트롤러 참조
    public FemaleCharacterController femaleCharacterController; // 여성 캐릭터 컨트롤러 참조
    public CameraFollow cameraFollow; // 카메라 추적

    public KeyCode switchKey = KeyCode.Tab; // 캐릭터 전환 키

    private void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            SwitchCharacter();
        }
    }

    private void SwitchCharacter()
    {
        bool playerActive = playerController.isActive;

        // 현재 활성화된 캐릭터를 비활성화
        if (playerActive)
        {
            DisableCharacter(playerController);
            EnableCharacter(femaleCharacterController);
            cameraFollow.SetTarget(femaleCharacterController.transform);
        }
        else
        {
            DisableCharacter(femaleCharacterController);
            EnableCharacter(playerController);
            cameraFollow.SetTarget(playerController.transform);
        }
    }

    private void EnableCharacter(MonoBehaviour character)
    {
        if (character is PlayerController pc)
        {
            pc.SetToIdleState();
            pc.isActive = true;
            pc.enabled = true;
            Rigidbody2D rb = pc.GetComponent<Rigidbody2D>();
            rb.isKinematic = false; // 물리 효과 활성화
        }
        else if (character is FemaleCharacterController fc)
        {
            fc.SetToIdleState();
            fc.isActive = true;
            fc.enabled = true;
            Rigidbody2D rb = fc.GetComponent<Rigidbody2D>();
            rb.isKinematic = false; // 물리 효과 활성화

        }
    }

    private void DisableCharacter(MonoBehaviour character)
    {
        if (character is PlayerController pc)
        {
            pc.isActive = false;
            pc.enabled = false;
            Rigidbody2D rb = pc.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero; // 속도 초기화
            rb.isKinematic = true; // 물리 효과 비활성화
        }
        else if (character is FemaleCharacterController fc)
        {
            fc.isActive = false;
            fc.enabled = false;
            Rigidbody2D rb = fc.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero; // 속도 초기화
            rb.isKinematic = true; // 물리 효과 비활성화
        }
    }

    public Transform GetActiveCharacter()
    {
        return playerController.isActive ? playerController.transform : femaleCharacterController.transform;
    }

    public Transform GetInactiveCharacter()
    {
        return !playerController.isActive ? playerController.transform : femaleCharacterController.transform;
    }

    public int GetInactiveCharacterIndex()
    {
        return playerController.isActive ? 1 : 0;
    }
}

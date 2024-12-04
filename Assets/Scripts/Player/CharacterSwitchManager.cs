using UnityEngine;

public class CharacterSwitchManager : MonoBehaviour
{
    public PlayerController playerController; // PlayerController 참조
    public FemaleCharacterController femaleCharacterController; // FemaleCharacterController 참조
    public CameraFollow cameraFollow; // CameraFollow 참조

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
        // 활성화 상태 전환
        bool playerActive = playerController.isActive;

        playerController.isActive = !playerActive;
        femaleCharacterController.isActive = playerActive;

        // 카메라 타겟 변경
        if (playerController.isActive)
        {
            cameraFollow.SetTarget(playerController.transform); // 플레이어를 카메라 타겟으로 설정
        }
        else if (femaleCharacterController.isActive)
        {
            cameraFollow.SetTarget(femaleCharacterController.transform); // 여성 캐릭터를 카메라 타겟으로 설정
        }
    }

    // 현재 활성화된 캐릭터의 Transform 반환
    public Transform GetActiveCharacter()
    {
        if (playerController.isActive)
            return playerController.transform;
        else
            return femaleCharacterController.transform;
    }

    // 현재 비활성화된 캐릭터의 Transform 반환
    public Transform GetInactiveCharacter()
    {
        if (!playerController.isActive)
            return playerController.transform;
        else
            return femaleCharacterController.transform;
    }

    // 비활성화된 캐릭터의 인덱스 반환
    public int GetInactiveCharacterIndex()
    {
        return playerController.isActive ? 1 : 0; // 1: 여성 캐릭터, 0: 플레이어
    }
}

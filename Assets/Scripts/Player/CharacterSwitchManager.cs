using UnityEngine;

public class CharacterSwitchManager : MonoBehaviour
{
    public PlayerController playerController; // PlayerController 참조
    public FemaleCharacterController femaleCharacterController; // FemaleCharacterController 참조
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

        // // 애니메이션 상태 초기화 (선택 사항)
        // if (playerController.isActive)
        // {
        //     playerController.anim.SetBool("walk", false);
        //     playerController.anim.SetBool("run", false);
        // }
        // else if (femaleCharacterController.isActive)
        // {
        //     femaleCharacterController.anim.SetBool("walk", false);
        // }

        Debug.Log("Switched character. Player active: " + playerController.isActive);
    }
}

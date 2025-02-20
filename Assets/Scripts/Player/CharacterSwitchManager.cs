using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterSwitchManager : MonoBehaviour
{
    public PlayerController playerController; // 플레이어 컨트롤러 참조
    public FemaleCharacterController femaleCharacterController; // 여성 캐릭터 컨트롤러 참조

    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera targetcam; // TargetGroup 카메라
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // player 1인 캠

    public KeyCode switchKey = KeyCode.Tab; // 캐릭터 전환 키

    public GameObject femaleOnlyRoute; // 인스펙터에서 넣어둔다.
    public GameObject cutoutMask; // 여주 주변을 보이게 할 Mask
    public GameObject GlowingRoute;

    private TilemapRenderer femaleRouteTilemapRenderer; // TilemapRenderer를 가져올 변수

    [Header("Settings")]
    public float thresholdDistance = 10f;  // 둘의 거리가 이 값을 초과하면 VCam2로 전환

    private void Start()
    {
        if (femaleOnlyRoute != null)
        {
            femaleRouteTilemapRenderer = femaleOnlyRoute.GetComponent<TilemapRenderer>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            SwitchCharacter();
        }

        float distance = Vector3.Distance(playerController.transform.position, femaleCharacterController.transform.position);

        if (distance > thresholdDistance)
        {
            targetcam.Priority = 10; // 예) 낮춤
            virtualCamera.Priority = 20; // 예) 높임
        }
        else
        {
            // 다시 가까워지면 VCam1 우선
            targetcam.Priority = 20;
            virtualCamera.Priority = 10;
        }
    }

    private void SwitchCharacter()
    {
        bool playerActive = playerController.isActive;

        // 현재 활성화된 캐릭터를 비활성화
        if (playerActive)
        {
            playerController.SetToIdleState();
            DisableCharacter(playerController);

            EnableCharacter(femaleCharacterController);
            virtualCamera.Follow = femaleCharacterController.transform;

            // **여주만 볼 수 있는 길 활성화**
            GlowingRoute.SetActive(true);
            SetMaskInteraction(false);
        }
        else
        {
            femaleCharacterController.SetToIdleState();
            DisableCharacter(femaleCharacterController);

            EnableCharacter(playerController);
            virtualCamera.Follow = playerController.transform;

            // **여주 전용 길 비활성화**
            // femaleOnlyRoute.SetActive(false);
            SetMaskInteraction(true);
            GlowingRoute.SetActive(false);
        }
    }

    // private IEnumerator ActivateFemaleOnlyRouteWithDelay(bool active, float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     GlowingRoute.SetActive(active);
    // }

    private void SetMaskInteraction(bool enableMask)
    {
        if (femaleRouteTilemapRenderer != null)
        {
            femaleRouteTilemapRenderer.maskInteraction = enableMask
                ? SpriteMaskInteraction.VisibleInsideMask
                : SpriteMaskInteraction.None;
        }
    }



    private void EnableCharacter(MonoBehaviour character)
    {
        if (character is PlayerController pc)
        {
            pc.isActive = true;
            pc.enabled = true;
            Rigidbody2D rb = pc.GetComponent<Rigidbody2D>();
            rb.isKinematic = false; // 물리 효과 활성화
        }
        else if (character is FemaleCharacterController fc)
        {
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

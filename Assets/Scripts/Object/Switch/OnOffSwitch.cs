using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class OnOffSwitch : SpotlightSwitch
{
    [SerializeField] Sprite onSprite;
    [SerializeField] Sprite offSprite;


    // 상호작용하는 플레이어의 GameObject를 저장할 변수
    private GameObject interactingPlayer;

    protected override void Awake()
    {
        base.Awake();

        if (spotlight.isOn)
        {
            sr.sprite = onSprite;
        }
        else
        {
            sr.sprite = offSprite;
        }
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInteracting && other.CompareTag("Player"))
        {
            bool isActive = false;
            // 플레이어 컨트롤러나 여성 캐릭터 컨트롤러 컴포넌트 중 하나를 확인
            if (other.TryGetComponent<PlayerController>(out var pc))
            {
                isActive = pc.isActive;
            }
            else if (other.TryGetComponent<FemaleCharacterController>(out var fc))
            {
                isActive = fc.isActive;
            }
            if (isActive)
            {
                isInteracting = true;
                interactingPlayer = other.gameObject;
            }
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (isInteracting && other.gameObject == interactingPlayer)
        {
            isInteracting = false;
            interactingPlayer = null;
        }
    }


    void Update()
    {
        // Z키 입력과 스포트라이트가 깨지지 않은 상태 확인
        if (isInteracting && Input.GetKeyDown(KeyCode.Z) && !spotlight.isBroken)
        {
            bool isActive = false;
            if (interactingPlayer != null)
            {
                if (interactingPlayer.TryGetComponent<PlayerController>(out var pc))
                {
                    isActive = pc.isActive;
                }
                else if (interactingPlayer.TryGetComponent<FemaleCharacterController>(out var fc))
                {
                    isActive = fc.isActive;
                }
            }
            // 플레이어가 활성 상태일 때만 스위치 작동
            if (isActive)
            {
                spotlight.Switch();
                ChangeSprite(spotlight.isOn);
            }
        }
    }


    void ChangeSprite(bool value)
    {
        sr.sprite = value ? onSprite : offSprite;
    }
}

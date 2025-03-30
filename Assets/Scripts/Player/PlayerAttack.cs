using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // 기본 총알 프리팹 (AttackUpgrade1 구매 후 사용)
    public GameObject bullet;
    // 큰 총알 프리팹 (AttackUpgrade2 구매 시 발사 확률 적용)
    public GameObject bigBullet;

    public Transform pos;    // 총알 생성 위치
    public float cooltime;   // 공격 쿨타임
    public float curtime;    // 현재 쿨타임

    public Animator anim;    // 플레이어 애니메이터

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 플레이어가 활성화되어 있지 않으면 입력 무시
        if (!GetComponent<PlayerController>().isActive)
            return;

        // 기본 공격 스킬(AttackUpgrade1)이 없으면 공격 불가
        if (!HasAttackUpgrade("AttackUpgrade1"))
            return;

        if (curtime <= 0)
        {
            // 키 한 번 눌림 처리 (GetKeyDown 사용)
            if (Input.GetKeyDown(KeyCode.X))
            {
                // 공격 애니메이션 트리거만 설정
                if (anim != null)
                {
                    anim.SetTrigger("Shoot");
                    Debug.Log("애니메이션 Shoot 트리거 실행");
                }
                // 쿨타임 초기화
                curtime = cooltime;
            }
        }
        curtime -= Time.deltaTime;
    }

    /// <summary>
    /// 애니메이션 이벤트에서 호출되는 함수.
    /// 이 함수가 호출될 때 실제 총알 발사 로직을 실행합니다.
    /// </summary>
    public void OnShootEvent()
    {
        bool fireBigBullet = false;
        // AttackUpgrade2가 있으면 bigBullet 발사 확률 적용
        if (HasAttackUpgrade("AttackUpgrade2"))
        {
            float chance = 0.3f; // 기본 확률 30%
            if (HasAttackUpgrade("AttackUpgrade3"))
            {
                chance = 0.6f; // AttackUpgrade3가 있으면 확률 60%
            }
            if (Random.value < chance)
            {
                fireBigBullet = true;
            }
        }

        if (fireBigBullet)
        {
            FireBigBullet();
        }
        else
        {
            FireBullet();
        }
    }

    // 구매 여부 체크: GameData의 purchasedUpgradeIDs에 해당 업그레이드 ID가 있는지 확인
    private bool HasAttackUpgrade(string upgradeID)
    {
        GameData gameData = DataPersistenceManager.instance.GetCurrentGameData();
        return gameData.purchasedUpgradeIDs.Contains(upgradeID);
    }

    // 기본 총알 발사 함수
    public void FireBullet()
    {
        if (bullet != null && pos != null)
        {
            GameObject newBullet = Instantiate(bullet, pos.position, Quaternion.identity);
            float direction = transform.localScale.x > 0 ? 1 : -1;
            newBullet.transform.localScale = new Vector3(direction, 1, 1);
            Debug.Log("FireBullet 호출");
        }
    }

    // 큰 총알 발사 함수
    public void FireBigBullet()
    {
        if (bigBullet != null && pos != null)
        {
            GameObject newBullet = Instantiate(bigBullet, pos.position, Quaternion.identity);
            float direction = transform.localScale.x > 0 ? 1 : -1;
            newBullet.transform.localScale = new Vector3(direction, 1, 1);
            Debug.Log("FireBigBullet 호출");
        }
    }
}

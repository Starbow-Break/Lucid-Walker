using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private ShopUI shopUI;  // Inspector에서 연결

    private void Awake()
    {
        if (shopUI == null)
        {
            Debug.LogError("ShopUI가 인스펙터에서 연결되지 않았습니다. 연결해주세요.");
        }
    }

    public void BuyItem(ItemData item)
    {
        var data = DataPersistenceManager.instance.GetCurrentGameData();

        if (data.gold < item.price)
        {
            Debug.Log("골드 부족!");
            return;
        }

        // 이미 산 아이템이면 패스
        if (data.purchasedUpgradeIDs.Contains(item.upgradeID))
        {
            Debug.Log("이미 구매한 아이템입니다.");
            return;
        }

        //-------------------------
        // 1. 골드 차감 & ID 등록
        //-------------------------
        data.gold -= item.price;
        data.purchasedUpgradeIDs.Add(item.upgradeID);

        //-------------------------
        // 2. 실제 스탯 적용
        //-------------------------
        ApplyStat(item, data);

        //-------------------------
        // 3. 세이브 & UI 새로고침
        //-------------------------
        DataPersistenceManager.instance.SaveGame();
        Debug.Log($"구매 완료 ▶ {item.itemName}");

        if (shopUI != null)
            StartCoroutine(RefreshUINextFrame(item));
    }

    /* ------------- 분리된 스탯 적용 메서드 ------------- */
    private void ApplyStat(ItemData item, GameData g)
    {
        switch (item.itemType)
        {
            case ItemType.HeartUpgrade:
                g.heartCount += 1;
                playerStats?.IncreaseMaxHearts(1);
                break;

            case ItemType.EnergyAmountUpgrade:
                g.maxEnergy += 20f;
                g.currentEnergy = g.maxEnergy;
                playerStats?.IncreaseMaxEnergy(20f);
                break;

            case ItemType.EnergyRegenUpgrade:
                g.energyRegenRate += 2f;
                playerStats?.IncreaseEnergyRegenRate(2f);
                break;

            case ItemType.AttackUpgrade:
                // TODO: 공격력 증가 로직
                break;

            case ItemType.LuckUpgrade:
                // TODO: 행운 증가 로직
                break;
        }
    }

    /* ------------- UI 새로고침을 다음 프레임으로 미룸 ------------- */
    private IEnumerator RefreshUINextFrame(ItemData bought)
    {
        yield return null;                      // 한 프레임 기다리기
        shopUI.UpdateUIAfterPurchase();         // 슬롯/버튼 상태
        shopUI.ShowItemDetail(bought);          // 오른쪽 상세 패널
    }
}

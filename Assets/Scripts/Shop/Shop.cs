using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private PlayerStats playerStats;
    private ShopUI shopUI;

    private void Awake()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        shopUI = FindObjectOfType<ShopUI>();
    }

    public void BuyItem(ItemData itemData)
    {
        GameData gameData = DataPersistenceManager.instance.GetCurrentGameData();

        // 골드 충분한지 체크
        if (gameData.gold >= itemData.price)
        {
            // 골드 차감
            gameData.gold -= itemData.price;

            // 아직 구매하지 않은 아이템이면 purchasedUpgradeIDs에 추가
            if (!gameData.purchasedUpgradeIDs.Contains(itemData.upgradeID))
            {
                gameData.purchasedUpgradeIDs.Add(itemData.upgradeID);

                switch (itemData.itemType)
                {
                    case ItemType.HeartUpgrade:
                        // 하트 업그레이드
                        gameData.heartCount += 1;

                        if (playerStats != null)
                        {
                            playerStats.IncreaseMaxHearts(1);
                        }
                        break;

                    case ItemType.EnergyAmountUpgrade:
                        // 에너지 최대치 업그레이드 (예: +20)
                        if (playerStats != null)
                        {
                            playerStats.IncreaseMaxEnergy(20f);
                        }
                        // GameData에도 동기화
                        gameData.maxEnergy += 20f;
                        gameData.currentEnergy = gameData.maxEnergy;
                        break;

                    case ItemType.EnergyRegenUpgrade:
                        // 에너지 회복률 업그레이드 (예: +2)
                        if (playerStats != null)
                        {
                            playerStats.IncreaseEnergyRegenRate(2f);
                        }
                        gameData.energyRegenRate += 2f;
                        break;

                    case ItemType.AttackUpgrade:
                        // 공격력 업그레이드 로직 (예: playerStats.AttackPower += 5f 등)
                        // GameData에도 반영할 수 있음
                        Debug.Log("공격력 업그레이드 적용 (예시)");
                        break;

                    case ItemType.LuckUpgrade:
                        // 행운 업그레이드 로직 (예: playerStats.Luck += 1f 등)
                        Debug.Log("행운 업그레이드 적용 (예시)");
                        break;
                }
            }

            // 구매 내용 세이브
            DataPersistenceManager.instance.SaveGame();

            Debug.Log("구매 완료: " + itemData.itemName);
            Debug.Log("구매된 업그레이드: " + string.Join(", ", gameData.purchasedUpgradeIDs));

            // UI 갱신
            if (shopUI != null)
            {
                // 현재 선택된 아이템을 저장
                ItemData currentItem = itemData;

                // UI 갱신
                shopUI.UpdateUIAfterPurchase();

                // 같은 아이템을 다시 선택하여 상세 패널 갱신
                shopUI.ShowItemDetail(currentItem);
            }
        }
        else
        {
            Debug.Log("골드 부족!");
        }
    }
}

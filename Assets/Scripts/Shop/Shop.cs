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

        if (gameData.gold >= itemData.price)
        {
            gameData.gold -= itemData.price;

            if (!gameData.purchasedUpgradeIDs.Contains(itemData.upgradeID))
            {
                gameData.purchasedUpgradeIDs.Add(itemData.upgradeID);

                // 만약 구매한 아이템이 HeartUpgrade이면 heartCount 증가
                if (itemData.itemType == ItemType.HeartUpgrade)
                {
                    gameData.heartCount += 1;

                    // PlayerStats에도 하트 증가 처리가 필요하면
                    if (playerStats != null)
                    {
                        playerStats.IncreaseMaxHearts(1);
                    }
                }
            }

            DataPersistenceManager.instance.SaveGame();

            Debug.Log("구매 완료: " + itemData.itemName);
            Debug.Log("구매된 업그레이드: " + string.Join(", ", gameData.purchasedUpgradeIDs));

            if (shopUI != null)
            {
                shopUI.UpdateUIAfterPurchase();
            }
        }
        else
        {
            Debug.Log("골드 부족!");
        }
    }

}
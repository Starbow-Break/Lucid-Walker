using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Slot Anchors (Manual Placement)")]
    [SerializeField] private List<Transform> slotPositions;

    [Header("Slot Prefab & Item Data")]
    [SerializeField] private GameObject shopItemSlotPrefab;
    [SerializeField] private List<ItemData> itemDataList;

    [Header("Right Panel")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailDescription;
    [SerializeField] private Button buyButton;

    [Header("Buy Button Sprites")]
    [SerializeField] private Sprite buyButtonGlowingSprite;    // 구매 가능(빛나는) 상태 스프라이트
    [SerializeField] private Sprite buyButtonPurchasedSprite;  // 구매 후 상태 스프라이트

    private Shop shopManager;
    private ItemData currentSelectedItem;

    private void Awake()
    {
        shopManager = FindObjectOfType<Shop>();
    }

    private void Start()
    {
        // (1) 각 아이템 데이터를 slotPositions에 따라 수동 배치
        for (int i = 0; i < itemDataList.Count; i++)
        {
            if (i >= slotPositions.Count)
            {
                Debug.LogWarning("슬롯 앵커 개수보다 아이템이 더 많습니다. 나머지는 생성되지 않음.");
                break;
            }

            GameObject slotObj = Instantiate(shopItemSlotPrefab, slotPositions[i]);
            ShopItemSlot slot = slotObj.GetComponent<ShopItemSlot>();
            slot.Initialize(itemDataList[i], this);
        }

        // 모든 슬롯 생성 후 UI 갱신 (한 번만 호출)
        UpdateUIAfterPurchase();

        // 아무것도 선택되지 않았으면 기본으로 HeartUpgrade1을 선택
        if (currentSelectedItem == null)
        {
            foreach (ItemData data in itemDataList)
            {
                if (data.itemType == ItemType.HeartUpgrade && data.upgradeID.Equals("HeartUpgrade1"))
                {
                    ShowItemDetail(data);
                    break;
                }
            }
        }

        // (2) 구매 버튼 이벤트 연결
        buyButton.onClick.AddListener(OnClickBuyButton);
    }

    // DataPersistenceManager에 있는 gameData를 반환
    public GameData GetGameData()
    {
        return DataPersistenceManager.instance.GetCurrentGameData();
    }

    // 현재 선택된 아이템을 반환하는 메서드 (ShopItemSlot에서 활성 슬롯 확인에 사용)
    public ItemData GetCurrentSelectedItem()
    {
        return currentSelectedItem;
    }

    // 오른쪽 상세 패널 업데이트
    public void ShowItemDetail(ItemData data)
    {
        currentSelectedItem = data;
        GameData gameData = GetGameData();
        bool isPurchased = gameData.purchasedUpgradeIDs.Contains(data.upgradeID);
        bool isLocked = false;

        if (data.itemType == ItemType.HeartUpgrade)
        {
            if (data.upgradeID.Equals("HeartUpgrade1"))
                isLocked = false;
            else if (data.upgradeID.Equals("HeartUpgrade2"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade1");
            else if (data.upgradeID.Equals("HeartUpgrade3"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade2");
        }

        // 오른쪽 상세 패널 이미지: 구매되면 활성 이미지, 아니면 비활성 이미지 사용
        detailIcon.sprite = isPurchased ? data.rightPanelActiveSprite : data.rightPanelInactiveSprite;
        detailName.text = data.itemName;
        detailDescription.text = data.itemDescription;

        // BuyButton의 스프라이트 변경: 구매되지 않은 경우 빛나는 이미지, 구매된 경우 기본 이미지 사용
        if (buyButton != null && buyButton.image != null)
        {
            buyButton.image.sprite = isPurchased ? buyButtonPurchasedSprite : buyButtonGlowingSprite;
        }

        buyButton.interactable = !isPurchased && !isLocked;

        // 상세 패널 업데이트 후, 모든 슬롯의 활성 상태 갱신 (activeGlowImage 업데이트)
        // ShopItemSlot[] slots = FindObjectsOfType<ShopItemSlot>();
        // foreach (ShopItemSlot slot in slots)
        // {
        //     slot.UpdateLockState();
        // }
    }

    // 구매 버튼 클릭 시 호출
    private void OnClickBuyButton()
    {
        if (currentSelectedItem == null) return;
        shopManager.BuyItem(currentSelectedItem);
    }

    // 구매 후 UI 갱신: 모든 슬롯과 상세 패널 업데이트
    public void UpdateUIAfterPurchase()
    {
        ShopItemSlot[] slots = FindObjectsOfType<ShopItemSlot>();
        foreach (ShopItemSlot slot in slots)
        {
            slot.UpdateSlotState();  // 통합 메서드 호출
        }

        if (currentSelectedItem != null)
        {
            ShowItemDetail(currentSelectedItem);
        }
    }

}

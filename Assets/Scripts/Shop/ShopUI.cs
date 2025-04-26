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
    private List<ShopItemSlot> activeSlots = new List<ShopItemSlot>();

    private void Awake()
    {
        shopManager = FindObjectOfType<Shop>();
    }

    private void Start()
    {
        CreateAllSlots();

        // 모든 슬롯 생성 후 UI 갱신 (한 번만 호출)
        UpdateAllSlots();

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

        // 구매 버튼 이벤트 연결
        buyButton.onClick.AddListener(OnClickBuyButton);
    }

    // 슬롯 생성 메서드
    private void CreateAllSlots()
    {
        // 기존에 생성된 슬롯 제거
        ClearAllSlots();

        // 슬롯 새로 생성
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
            activeSlots.Add(slot);
        }
    }

    // 모든 슬롯 제거
    private void ClearAllSlots()
    {
        activeSlots.Clear();

        foreach (Transform slotTransform in slotPositions)
        {
            for (int i = slotTransform.childCount - 1; i >= 0; i--)
            {
                Destroy(slotTransform.GetChild(i).gameObject);
            }
        }
    }

    // 모든 슬롯 상태 업데이트
    private void UpdateAllSlots()
    {
        foreach (ShopItemSlot slot in activeSlots)
        {
            slot.UpdateSlotState();
        }
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

    // 잠금 상태 확인 메서드 (ShopItemSlot의 체크 로직과 동일하게 적용)
    private bool CheckIsLocked(ItemData itemData, GameData gameData)
    {
        // 기본값
        bool isLocked = false;

        // (1) 하트 업그레이드
        if (itemData.itemType == ItemType.HeartUpgrade)
        {
            if (itemData.upgradeID.Equals("HeartUpgrade1"))
                isLocked = false;
            else if (itemData.upgradeID.Equals("HeartUpgrade2"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade1");
            else if (itemData.upgradeID.Equals("HeartUpgrade3"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade2");
        }

        // (2) 에너지 업그레이드
        else if (itemData.upgradeID.Equals("EnergyAmount1"))
        {
            isLocked = false;
        }
        else if (itemData.upgradeID.Equals("EnergyRegen1"))
        {
            isLocked = !gameData.purchasedUpgradeIDs.Contains("EnergyAmount1");
        }
        else if (itemData.upgradeID.Equals("EnergyAmount2"))
        {
            isLocked = !gameData.purchasedUpgradeIDs.Contains("EnergyAmount1");
        }
        else if (itemData.upgradeID.Equals("EnergyRegen2"))
        {
            isLocked = !gameData.purchasedUpgradeIDs.Contains("EnergyRegen1");
        }
        else if (itemData.upgradeID.Equals("EnergyAmount3"))
        {
            isLocked = !gameData.purchasedUpgradeIDs.Contains("EnergyAmount2");
        }

        // (3) 공격력 업그레이드
        else if (itemData.upgradeID.Equals("AttackUpgrade1"))
        {
            isLocked = false;
        }
        else if (itemData.upgradeID.Equals("AttackUpgrade2"))
        {
            isLocked = !gameData.purchasedUpgradeIDs.Contains("AttackUpgrade1");
        }
        else if (itemData.upgradeID.Equals("AttackUpgrade3"))
        {
            isLocked = !gameData.purchasedUpgradeIDs.Contains("AttackUpgrade2");
        }

        // (4) 행운 업그레이드
        else if (itemData.upgradeID.Equals("LuckUpgrade1"))
        {
            isLocked = false;
        }
        else if (itemData.upgradeID.Equals("LuckUpgrade2"))
        {
            isLocked = !gameData.purchasedUpgradeIDs.Contains("LuckUpgrade1");
        }
        else if (itemData.upgradeID.Equals("LuckUpgrade3"))
        {
            isLocked = !gameData.purchasedUpgradeIDs.Contains("LuckUpgrade2");
        }

        return isLocked;
    }

    // 오른쪽 상세 패널 업데이트
    public void ShowItemDetail(ItemData data)
    {
        if (data == null) return;

        currentSelectedItem = data;
        GameData gameData = GetGameData();
        bool isPurchased = gameData.purchasedUpgradeIDs.Contains(data.upgradeID);

        // 모든 아이템 타입에 대한 잠금 상태 확인
        bool isLocked = CheckIsLocked(data, gameData);

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

        // 상세 패널 업데이트 후, 모든 슬롯의 활성 상태 갱신
        UpdateAllSlots();
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
        // 각 슬롯의 상태 갱신 (아이템 생성/제거 대신 상태만 업데이트)
        UpdateAllSlots();

        // 오른쪽 패널 갱신
        if (currentSelectedItem != null)
        {
            ShowItemDetail(currentSelectedItem);
        }
    }

    public void ClearSelectedItem()
    {
        currentSelectedItem = null;
    }
}
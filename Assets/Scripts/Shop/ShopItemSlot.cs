using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private Image icon;                   // 슬롯에 표시될 아이콘 이미지
    [SerializeField] private GameObject lockOverlay;         // 잠금 상태 오버레이
    [SerializeField] private Button button;                  // 슬롯 클릭 버튼
    [SerializeField] private Image activeGlowImage;     // 활성화된 슬롯 뒤에 표시할 빛나는 이미지

    private ItemData itemData;
    private ShopUI shopUI;

    // Initialize()는 인스펙터에 ShopUI를 수동으로 할당할 필요 없이 호출 시 전달받습니다.
    public void Initialize(ItemData data, ShopUI shopUI)
    {
        this.itemData = data;
        this.shopUI = shopUI;
        UpdateIcon();
        UpdateLockState();
        button.onClick.AddListener(OnClickItem);
    }

    // 왼쪽 슬롯에 표시할 아이콘을 구매 및 잠금 상태에 따라 설정
    public void UpdateIcon()
    {
        GameData gameData = shopUI.GetGameData();
        bool isPurchased = gameData.purchasedUpgradeIDs.Contains(itemData.upgradeID);
        bool isLocked = false;

        if (itemData.itemType == ItemType.HeartUpgrade)
        {
            if (itemData.upgradeID.Equals("HeartUpgrade1"))
                isLocked = false;
            else if (itemData.upgradeID.Equals("HeartUpgrade2"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade1");
            else if (itemData.upgradeID.Equals("HeartUpgrade3"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade2");
        }

        if (isLocked)
            icon.sprite = itemData.leftPanelLockedSprite;
        else if (isPurchased)
        {
            icon.sprite = itemData.leftPanelActiveSprite;
            activeGlowImage.gameObject.SetActive(true);
        }
        else
            icon.sprite = itemData.leftPanelInactiveSprite;
    }

    public void UpdateLockState()
    {
        GameData gameData = shopUI.GetGameData();
        bool isLocked = false;
        if (itemData.itemType == ItemType.HeartUpgrade)
        {
            if (itemData.upgradeID.Equals("HeartUpgrade1"))
                isLocked = false;
            else if (itemData.upgradeID.Equals("HeartUpgrade2"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade1");
            else if (itemData.upgradeID.Equals("HeartUpgrade3"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade2");
        }

        lockOverlay.SetActive(isLocked);
        button.interactable = !isLocked;
        UpdateIcon();

    }


    private void OnClickItem()
    {
        shopUI.ShowItemDetail(itemData);
    }
}

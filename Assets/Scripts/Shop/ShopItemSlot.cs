using UnityEngine;
using UnityEngine.UI;


public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField] private Button button;
    [SerializeField] private Image activeGlowImage;

    private ItemData itemData;
    private ShopUI shopUI;

    public void Initialize(ItemData data, ShopUI shopUI)
    {
        this.itemData = data;
        this.shopUI = shopUI;
        UpdateSlotState();   // 슬롯 상태 한 번 갱신
        button.onClick.AddListener(OnClickItem);
    }

    /// <summary>
    /// 슬롯 상태를 갱신하는 통합 메서드
    /// 잠금 여부, 구매 여부, 아이콘, 버튼, 오버레이 등을 전부 여기서 처리
    /// </summary>
    /// 

    public void UpdateSlotState()
    {
        GameData gameData = shopUI.GetGameData();
        bool isPurchased = gameData.purchasedUpgradeIDs.Contains(itemData.upgradeID);
        bool isLocked = CheckIsLocked(itemData, gameData);

        if (isLocked)
        {
            icon.sprite = itemData.leftPanelLockedSprite;
            activeGlowImage.gameObject.SetActive(false);
        }
        else
        {
            if (isPurchased)
            {
                icon.sprite = itemData.leftPanelActiveSprite;
                activeGlowImage.gameObject.SetActive(true);
            }
            else
            {
                icon.sprite = itemData.leftPanelInactiveSprite;
                activeGlowImage.gameObject.SetActive(false);
            }
        }

        lockOverlay.SetActive(isLocked);
        button.interactable = !isLocked;
    }

    // public void UpdateSlotState()
    // {
    //     GameData gameData = shopUI.GetGameData();
    //     bool isPurchased = gameData.purchasedUpgradeIDs.Contains(itemData.upgradeID);

    //     // 1) 잠금 여부 계산
    //     bool isLocked = CheckIsLocked(itemData, gameData);

    //     // 2) 아이콘 설정
    //     if (isLocked)
    //     {
    //         icon.sprite = itemData.leftPanelLockedSprite;
    //         activeGlowImage.gameObject.SetActive(false);
    //     }
    //     else if (isPurchased)
    //     {
    //         icon.sprite = itemData.leftPanelActiveSprite;
    //         activeGlowImage.gameObject.SetActive(true);
    //     }
    //     else
    //     {
    //         icon.sprite = itemData.leftPanelInactiveSprite;
    //         activeGlowImage.gameObject.SetActive(false);
    //     }

    //     // 3) 오버레이와 버튼 상태
    //     lockOverlay.SetActive(isLocked);
    //     button.interactable = !isLocked;
    // }

    /// <summary>
    /// 업그레이드ID(또는 itemType)에 따른 잠금 여부를 계산하는 메서드
    /// </summary>
    private bool CheckIsLocked(ItemData itemData, GameData gameData)
    {
        // 기본값
        bool isLocked = false;

        // (1) 하트 업그레이드 예시
        if (itemData.itemType == ItemType.HeartUpgrade)
        {
            if (itemData.upgradeID.Equals("HeartUpgrade1"))
                isLocked = false;
            else if (itemData.upgradeID.Equals("HeartUpgrade2"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade1");
            else if (itemData.upgradeID.Equals("HeartUpgrade3"))
                isLocked = !gameData.purchasedUpgradeIDs.Contains("HeartUpgrade2");
        }

        // (2) 에너지 업그레이드 예시
        if (itemData.upgradeID.Equals("EnergyAmount1"))
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
        if (itemData.upgradeID.Equals("AttackUpgrade1"))
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
        if (itemData.upgradeID.Equals("LuckUpgrade1"))
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

    private void OnClickItem()
    {
        shopUI.ShowItemDetail(itemData);
    }
}

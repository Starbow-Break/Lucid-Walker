using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public int price;
    public ItemType itemType;
    public string upgradeID;

    // 왼쪽 패널용 스프라이트들
    public Sprite leftPanelInactiveSprite;    // 기본 상태 (비활성화, 아직 구매 전)
    public Sprite leftPanelActiveSprite;      // 활성 상태 (예: 구매 후 혹은 강조 상태)
    public Sprite leftPanelLockedSprite;      // 잠금 상태 (예: 이전 단계 미구매로 잠금)

    // 오른쪽 패널용 스프라이트들 (상세정보 패널)
    public Sprite rightPanelInactiveSprite;   // 구매 전 상태
    public Sprite rightPanelActiveSprite;     // 구매 후(활성화, 빛나는) 상태
}

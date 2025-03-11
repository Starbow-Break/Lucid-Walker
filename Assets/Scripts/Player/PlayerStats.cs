using UnityEngine;

public class PlayerStats : MonoBehaviour, IDataPersistence
{
    // 최대 하트 수 (기본 3)
    [SerializeField] private int maxHearts = 3;
    // 새로 추가: 골드 보유량
    [SerializeField] private int gold = 100;



    // IDataPersistence 구현
    public void LoadData(GameData data)
    {
        // 세이브 파일에 heartCount가 있다면 가져와서 적용
        this.maxHearts = data.heartCount;
        this.gold = data.gold;
    }

    public void SaveData(ref GameData data)
    {
        // 현재 maxHearts를 세이브 파일에 반영
        data.heartCount = this.maxHearts;
        data.gold = this.gold; // 현재 골드 세이브
    }


    public int GetMaxHearts()
    {
        return maxHearts;
    }

    // 외부에서 호출할 수 있는 최대 하트 증가 메서드
    public void IncreaseMaxHearts(int amount = 1)
    {
        maxHearts += amount;
    }

}

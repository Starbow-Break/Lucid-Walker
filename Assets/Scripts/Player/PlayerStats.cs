using UnityEngine;

public class PlayerStats : MonoBehaviour, IDataPersistence
{
    // 최대 하트 수 (기본 3)
    [SerializeField] private int maxHearts = 3;
    // 새로 추가: 골드 보유량
    [SerializeField] private int gold = 100;
    // ---기력 관련 변수들 ---
    [SerializeField] private float maxEnergy = 100f;       // 기력 최대치
    [SerializeField] private float currentEnergy = 100f;   // 현재 기력
    [SerializeField] private float energyRegenRate = 5f;   // 초당 기력 회복량
    [SerializeField] private float energyDrainRate = 10f;  // 달릴 때/ 수영 초당 소모량
    public static PlayerStats Instance { get; private set; }
    public bool IsSinking { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // IDataPersistence 구현
    public void LoadData(GameData data)
    {
        // 세이브 파일에 heartCount가 있다면 가져와서 적용
        this.maxHearts = data.heartCount;
        this.gold = data.gold;
        this.maxEnergy = data.maxEnergy;
        this.currentEnergy = data.currentEnergy;
        this.energyRegenRate = data.energyRegenRate;
        this.energyDrainRate = data.energyDrainRate;
    }

    public void SaveData(ref GameData data)
    {
        // 현재 maxHearts를 세이브 파일에 반영
        data.heartCount = this.maxHearts;
        data.gold = this.gold; // 현재 골드 세이브

        // 추가: 기력 관련 데이터 세이브
        data.maxEnergy = this.maxEnergy;
        data.currentEnergy = this.currentEnergy;
        data.energyRegenRate = this.energyRegenRate;
        data.energyDrainRate = this.energyDrainRate;
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


    public float GetMaxEnergy() => maxEnergy;
    public float GetCurrentEnergy() => currentEnergy;
    public float GetEnergyRegenRate() => energyRegenRate;
    public float GetEnergyDrainRate() => energyDrainRate;

    public void SetCurrentEnergy(float value)
    {
        currentEnergy = Mathf.Clamp(value, 0f, maxEnergy);
    }

    // 상점에서 기력 능력치를 구매할 때 사용할 수 있는 예시 메서드
    public void IncreaseMaxEnergy(float amount)
    {
        maxEnergy += amount;
        currentEnergy = maxEnergy; // 구매 시 기력 풀로 채우는 식으로 처리 가능
    }

    public void IncreaseEnergyRegenRate(float amount)
    {
        energyRegenRate += amount;
    }

}

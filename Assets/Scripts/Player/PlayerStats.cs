using UnityEngine;
using System;

public class PlayerStats : MonoBehaviour, IDataPersistence
{
    // ───────────── Singleton ─────────────
    public static PlayerStats Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ───────────── Hearts ─────────────
    [SerializeField] private int maxHearts = 3;
    public int MaxHearts => maxHearts;
    public void IncreaseMaxHearts(int amt = 1) => maxHearts += amt;

    // ───────────── Gold ─────────────
    [SerializeField] private int gold = 100;
    public int Gold => gold;
    public event Action<int> OnGoldChanged;          // UI가 구독

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        return true;
    }
    public void AddGold(int amount)
    {
        gold += amount;
        OnGoldChanged?.Invoke(gold);
    }

    // ───────────── Energy ─────────────
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy = 100f;
    [SerializeField] private float energyRegenRate = 5f;
    [SerializeField] private float energyDrainRate = 10f;

    public float MaxEnergy => maxEnergy;
    public float CurrentEnergy => currentEnergy;
    public float EnergyRegenRate => energyRegenRate;
    public float EnergyDrainRate => energyDrainRate;

    public void SetCurrentEnergy(float v) => currentEnergy = Mathf.Clamp(v, 0, maxEnergy);
    public void IncreaseMaxEnergy(float amt) { maxEnergy += amt; currentEnergy = maxEnergy; }
    public void IncreaseEnergyRegenRate(float amt) { energyRegenRate += amt; }

    // ───────────── Attack & Luck (추가) ─────────────
    // [SerializeField] private int attackPower = 10;
    [SerializeField] private int luck = 0;
    // public int AttackPower => attackPower;
    public int Luck => luck;

    // public void IncreaseAttack(int amt) => attackPower += amt;
    public void IncreaseLuck(int amt) => luck += amt;

    // ───────────── IDataPersistence ─────────────
    public void LoadData(GameData d)
    {
        maxHearts = d.heartCount;
        gold = d.gold;

        maxEnergy = d.maxEnergy;
        currentEnergy = d.currentEnergy;
        energyRegenRate = d.energyRegenRate;
        energyDrainRate = d.energyDrainRate;

        // attackPower = d.attackPower;  
        luck = d.luck;
    }

    public void SaveData(ref GameData d)
    {
        d.heartCount = maxHearts;
        d.gold = gold;

        d.maxEnergy = maxEnergy;
        d.currentEnergy = currentEnergy;
        d.energyRegenRate = energyRegenRate;
        d.energyDrainRate = energyDrainRate;

        // d.attackPower = attackPower;
        d.luck = luck;
    }

    // ───────────── 기타 상태 ─────────────
    public bool IsSinking { get; set; }
}

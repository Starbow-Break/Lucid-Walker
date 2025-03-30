using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerEnergyBar : MonoBehaviour
{
    public Transform character;
    public Vector3 offset;
    [SerializeField] private Slider energySlider;
    [SerializeField] private PlayerStats playerStats;


    void Update()
    {
        if (character != null)
        {
            transform.position = character.position + offset;
        }
        if (playerStats != null)
        {
            float currentEnergy = playerStats.GetCurrentEnergy();
            float maxEnergy = playerStats.GetMaxEnergy();

            // 슬라이더 값 = 현재 기력 / 최대 기력
            energySlider.value = currentEnergy / maxEnergy;
        }
    }
}

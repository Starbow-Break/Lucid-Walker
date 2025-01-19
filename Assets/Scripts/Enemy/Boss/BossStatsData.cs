using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 보스 스텟 데이터
[CreateAssetMenu(fileName = "Boss Stats Data", menuName = "Scriptable Object/Boss Stats Data", order = int.MaxValue)]
public class BossStatsData : ScriptableObject
{
    // NORMAL : 우리가 평소에 생각하는 체력 (데미지를 받음으로서 감소)
    // TIME_ATTACK : 타임 어택 (시간이 지남에 따라 감소)
    [System.Serializable]
    public enum HealthType {
        NORMAL, TIME_ATTACK
    }

    [SerializeField] public int hp;
    [SerializeField] public HealthType healthType;
}

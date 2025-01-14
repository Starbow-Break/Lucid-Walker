using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬 정보를 담는 ScriptableObject
[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Object/Skill Data", order = int.MaxValue)]
public class SkillData : ScriptableObject
{
    [SerializeField] string skillName;
    [SerializeField] Skill skill;
}

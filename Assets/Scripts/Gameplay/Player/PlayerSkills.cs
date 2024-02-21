using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : SingletonBehaviour<PlayerSkills>
{
    public List<SkillData> Skills { get; private set; } = new();

    public event Action<SkillData> OnSkillPickup;

    public void AddSkill(SkillData skill)
    {
        OnSkillPickup?.Invoke(skill);
        if (Skills.Contains(skill))
        {
            if (skill.IsStackable)
                Skills.Find(s => s.name == skill.name).CurrentDuration += skill.Duration;

            return;
        }

        Skills.Add(skill);
    }
}
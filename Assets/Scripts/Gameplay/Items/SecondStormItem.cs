using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondStormItem : MonoBehaviour
{
    void Start()
    {
        foreach (var skill in PlayerSkills.Instance.SkillBehaviours)
        {
            skill.CooldownMultiplier -= 0.1f;
        }

        PlayerSkills.Instance.OnSkillPickup += OnPickup;
    }

    private void OnDisable()
    {
        PlayerSkills.Instance.OnSkillPickup -= OnPickup;
    }

    private void OnPickup(SkillData data)
    {
        foreach (var skill in PlayerSkills.Instance.SkillBehaviours)
        {
            skill.CooldownMultiplier -= 0.3f;
        }
    }
}

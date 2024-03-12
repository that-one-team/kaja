using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerSkills : SingletonBehaviour<PlayerSkills>
{
    [SerializeField] Transform _skillContainer;
    public Dictionary<string, SkillData> Skills { get; private set; } = new();
    public event Action<SkillData> OnSkillPickup;

    readonly string[] _keybinds = { "Q", "E", "R" };

    AudioSource _source;

    void Start()
    {
        _source = GetComponent<AudioSource>();
    }

    public void AddSkill(SkillData skill)
    {
        OnSkillPickup?.Invoke(skill);
        GetComponent<AudioSource>().PlayOneShot(skill.PickupSfx, 0.2f);
        if (Skills.ContainsValue(skill))
        {
            if (skill.IsStackable)
                Skills.Where(s => s.Value.name == skill.name).FirstOrDefault().Value.CurrentDuration += skill.Duration;

            return;
        }

        var kb = _keybinds[Skills.Count];
        var ui = Instantiate(skill.SkillBehaviour, _skillContainer);
        ui.GetComponentInChildren<TextMeshProUGUI>().text = kb;
        var behaviour = ui.GetComponent<Skill>();
        behaviour.UseKeybind = (KeyCode)Enum.Parse(typeof(KeyCode), kb, true);
        behaviour.Data = skill;
        Skills.Add(kb, skill);
    }

    /// <summary>
    /// Only plays the sound for now
    /// </summary>
    /// <param name="skill"></param>
    public void UseSkill(SkillData skill)
    {
        _source.PlayOneShot(skill.PickupSfx, 0.5f);
    }
}
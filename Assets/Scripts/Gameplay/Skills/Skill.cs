using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(Rigidbody))]
public class Skill : Interactable
{
    [Header("Skill settings")]
    [Expandable]
    public SkillData Data;

    void Start()
    {
        var light = GetComponent<Light>();
        light.intensity = 2;
        light.color = Data.PickupIndicatorColor;
    }

    public override void Interact()
    {
        PlayerSkills.Instance.AddSkill(Data);
        Destroy(gameObject);
    }

    public virtual void UseSkill() { }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "proj/SkillData", order = 0)]
public class SkillData : ScriptableObject
{
    public new string name;
    [TextArea]
    public string Description;
    public bool IsStackable;
    public float Duration = 5;
    public float Cooldown = 100;
    public float CurrentDuration { get; set; }

    public Color PickupIndicatorColor = new(0, 0, 1, 0.5f);
    public AudioClip PickupSfx;
}
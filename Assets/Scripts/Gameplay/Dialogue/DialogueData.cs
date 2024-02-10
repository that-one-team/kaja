using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea]
    public string Line;
    public float CharacterSpeed = 0.5f;
}

[CreateAssetMenu(fileName = "Dialogue Data", menuName = "proj/Dialogue Data", order = 0)]
public class DialogueData : ScriptableObject
{
    [field: SerializeField] public string SpeakerName { get; set; }
    [field: SerializeField] public AudioClip DialogueAudio { get; set; }
    [field: SerializeField] public DialogueLine[] Lines { get; private set; }
    [field: SerializeField] public bool ShowSpeakerName { get; private set; }
    [field: SerializeField] public int MaxInteractions { get; private set; } = 1;

}
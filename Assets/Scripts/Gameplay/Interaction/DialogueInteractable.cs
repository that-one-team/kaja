using NaughtyAttributes;
using UnityEngine;

public class DialogueInteractable : Interactable
{
    [Expandable]
    public DialogueData Data;

    public int CurrentInteractionCount { get; private set; } = 0;

    public override void Interact()
    {
        if (CurrentInteractionCount == Data.MaxInteractions) return;
        CurrentInteractionCount++;
        DialogueManager.Instance.RunDialogue(Data);
    }
}

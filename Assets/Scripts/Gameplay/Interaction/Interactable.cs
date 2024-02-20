using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    LOOK_AT,
    COLLIDE,
    BUTTON
}

public class Interactable : MonoBehaviour
{
    [field: SerializeField]
    public InteractionType InteractionType { get; protected set; }

    public virtual void Interact()
    {

    }
}

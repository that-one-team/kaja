using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    LOOK_AT,
    COLLIDE
}

public class Interactable : MonoBehaviour
{
    [field: SerializeField]
    public InteractionType InteractionType { get; private set; }

    public virtual void Interact()
    {

    }
}

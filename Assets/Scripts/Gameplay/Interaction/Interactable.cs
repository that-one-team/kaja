using Unity.VisualScripting;
using UnityEngine;

public enum InteractionType
{
    LOOK_AT,
    COLLIDE,
}

[RequireComponent(typeof(Outline))]
public class Interactable : MonoBehaviour
{
    [Header("Interaction settings")]
    public InteractionType InteractionType;
    [SerializeField] bool _showOutlineOnHover = true;

    public bool IsInteractable { get; protected set; } = true;

    Outline _outline;

    private void OnValidate()
    {
        _outline = GetComponent<Outline>();
        _outline.OutlineColor = Color.yellow;
        _outline.OutlineWidth = 6;
        _outline.enabled = false;
    }

    private void OnEnable()
    {
        _outline = GetComponent<Outline>();
    }

    public void ShowOutlines(bool isShowing)
    {
        if (!_showOutlineOnHover || _outline == null) return;

        _outline.enabled = IsInteractable && isShowing;
    }

    public virtual void Interact()
    {

    }
}

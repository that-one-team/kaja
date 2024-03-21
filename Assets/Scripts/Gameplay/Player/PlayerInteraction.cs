using NaughtyAttributes.Test;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] float _interactionRange;
    [SerializeField] LayerMask _interactionMask;

    bool _hasInteraction;
    RaycastHit _hit;
    GameObject _lastHit;

    void Start()
    {
        PlayerInputs.Instance.Actions.Fire.performed += OnShoot;
        PlayerInputs.Instance.Items.SelectItem.performed += OnItemSelect;
        PlayerInputs.Instance.Actions.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.Actions.Fire.performed -= OnShoot;
        PlayerInputs.Instance.Items.SelectItem.performed -= OnItemSelect;
        PlayerInputs.Instance.Actions.Interact.performed -= OnInteract;
    }

    void OnShoot(InputAction.CallbackContext ctx)
    {
        var wep = PlayerInventory.Instance.CurrentWeapon;
        if (wep != null) wep.Shoot();
    }

    void OnItemSelect(InputAction.CallbackContext ctx)
    {
        var val = (int)ctx.ReadValue<float>();
        PlayerInventory.Instance.EquipWeapon(val - 1);
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!_hasInteraction) return;
        var obj = _hit.collider;

        if (obj.TryGetComponent(out Interactable interactable))
        {
            if (interactable.InteractionType != InteractionType.LOOK_AT) return;

            interactable.Interact();
        }
    }

    void Update()
    {
        _hasInteraction = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out _hit, _interactionRange, _interactionMask);

        if (_hasInteraction && _hit.collider.TryGetComponent(out Interactable interactable))
        {
            _lastHit = _hit.collider.gameObject;
            interactable.ShowOutlines(true);
        }
        else
        {
            if (_lastHit == null) return;
            if (_lastHit.TryGetComponent(out Interactable inter))
            {
                inter.ShowOutlines(false);
                _lastHit = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Item item))
        {
            if (item.Data.InteractionType != InteractionType.COLLIDE) return;
            PlayerInventory.Instance.AddItem(item);
        }

        if (other.TryGetComponent(out Interactable interactable))
        {
            if (interactable.InteractionType != InteractionType.COLLIDE) return;
            interactable.Interact();
        }
    }
}

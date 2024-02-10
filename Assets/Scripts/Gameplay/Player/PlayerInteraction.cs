using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    void Start()
    {
        PlayerInputs.Instance.Actions.Fire.performed += OnShoot;
        PlayerInputs.Instance.Items.SelectItem.performed += OnItemSelect;
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


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Item>(out Item item))
        {
            if (item.Data.InteractionType == ItemInteractionType.LOOK_AT) return;
            PlayerInventory.Instance.AddItem(item);
        }

        if (other.TryGetComponent<Interactable>(out Interactable interactable))
        {
            interactable.Interact();
        }
    }
}

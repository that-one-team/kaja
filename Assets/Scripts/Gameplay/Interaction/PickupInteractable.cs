using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PickupInteractable : Interactable
{
    Rigidbody _rb;
    bool _isBeingPickedUp = false;
    Transform _cam;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        PlayerInputs.Instance.Actions.Fire.performed += OnFire;

        _cam = Camera.main.transform;
    }

    private void OnDisable()
    {
        PlayerInputs.Instance.Actions.Fire.performed -= OnFire;
    }

    public override void Interact()
    {
        _isBeingPickedUp = true;
        _rb.useGravity = false;
        _rb.drag = 20;
        PlayerInventory.Instance.EquipWeapon(99, force: true);
    }

    void OnFire(InputAction.CallbackContext ctx)
    {
        if (!_isBeingPickedUp) return;
        _rb.drag = 0;
        _rb.useGravity = true;
        _isBeingPickedUp = false;
        _rb.AddForce(_cam.transform.forward * 50, ForceMode.Impulse);
        PlayerInventory.Instance.EquipWeapon(PlayerInventory.Instance.CurrentWeaponIndex, force: true);
    }

    void Update()
    {
        if (!_isBeingPickedUp) return;

        var point = _cam.position + _cam.forward * 3;

        if (Vector3.Distance(transform.position, point) > 0.1f)
        {
            var dir = point - transform.position;
            _rb.AddForce(9000 * Time.deltaTime * dir);
        }
    }
}

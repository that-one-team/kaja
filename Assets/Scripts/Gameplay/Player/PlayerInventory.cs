using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerInventory : SingletonBehaviour<PlayerInventory>
{
    [ShowNonSerializedField] private const int MAX_WEAPON_COUNT = 3;

    public List<ItemData> Weapons { get; private set; } = new();
    public List<ItemData> Items { get; private set; } = new();

    private readonly List<GameObject> _spawnedWeapons = new(MAX_WEAPON_COUNT);
    private int _currWeaponIndex = -1;

    public Weapon CurrentWeapon
    {
        get
        {
            if (_currWeaponIndex < Weapons.Count)
                return _spawnedWeapons[_currWeaponIndex].GetComponent<Weapon>();
            return null;
        }
    }

    [Header("Settings")]
    [SerializeField] int _maxWeaponCount = 3;
    [SerializeField] GameObject _weaponPrefab;
    [SerializeField] Transform _weaponHolder;

    public event Action<Item> OnItemAdd;
    public event Action<Item> OnReplaceItem;

    public bool IsReplacingItem { get; private set; } = false;

    private void Start()
    {
        for (int i = 0; i < _maxWeaponCount; i++)
        {
            var wep = Instantiate(_weaponPrefab, _weaponHolder).GetComponent<Weapon>();
            wep.gameObject.SetActive(false);
            _spawnedWeapons.Add(wep.gameObject);
        }
    }

    public void AddItem(Item itemToAdd)
    {
        if (itemToAdd.Data.Type == ItemType.WEAPON)
        {
            if (Weapons.Count == _maxWeaponCount)
            {
                StartCoroutine(ReplaceItem(itemToAdd));
            }
            else
            {
                Weapons.Add(itemToAdd.Data);
                UpdateWeaponsVisuals();
            }
        }

        OnItemAdd?.Invoke(itemToAdd);
        Notifications.Instance.Notify("Picked up " + itemToAdd.Data.FriendlyName);
        Destroy(itemToAdd.gameObject);
    }

    IEnumerator ReplaceItem(Item item)
    {
        IsReplacingItem = true;
        while (IsReplacingItem)
        {
            OnReplaceItem?.Invoke(item);
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                DropWeapon(0);
                Weapons[0] = item.Data;
            }
            yield return new WaitForEndOfFrame();
        }
        IsReplacingItem = false;
        yield break;
    }

    public void DropWeapon(int index)
    {
    }

    void UpdateWeaponsVisuals()
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            var wep = Weapons[i];
            var onHand = _spawnedWeapons[i];

            onHand.GetComponent<Weapon>().Data = wep;
            onHand.GetComponent<Weapon>().UpdateVisuals();
        }

        if (_currWeaponIndex == -1 && Weapons.Count > 0)
        {
            _currWeaponIndex = 0;
            EquipWeapon(_currWeaponIndex, force: true);
        }
    }

    public void EquipWeapon(int index, bool force = false)
    {
        if (Weapons.Count <= index || (_currWeaponIndex == index && !force)) return;

        if (CurrentWeapon != null)
            CurrentWeapon.Unequip();

        _currWeaponIndex = index;

        var weapon = _spawnedWeapons[index].GetComponent<Weapon>();
        weapon.gameObject.SetActive(true);
        weapon.Equip();
    }
}

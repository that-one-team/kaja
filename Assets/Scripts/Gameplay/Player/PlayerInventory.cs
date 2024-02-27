using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerInventory : SingletonBehaviour<PlayerInventory>
{
    [ShowNonSerializedField][ReadOnly] private const int MAX_WEAPON_COUNT = 4;

    public List<ItemData> Weapons { get; private set; } = new();
    public List<ItemData> Items { get; private set; } = new();

    private readonly List<GameObject> _spawnedWeapons = new(MAX_WEAPON_COUNT);
    public int CurrentWeaponIndex { get; private set; } = -1;

    public Weapon CurrentWeapon
    {
        get
        {
            if (CurrentWeaponIndex < Weapons.Count && CurrentWeaponIndex >= 0)
                return _spawnedWeapons[CurrentWeaponIndex].GetComponent<Weapon>();
            return null;
        }
    }

    [Header("Settings")]
    [SerializeField] GameObject _weaponPrefab;
    [SerializeField] Transform _weaponHolder;
    [SerializeField] ItemData _startingWeapon;

    public event Action<Item> OnItemAdd;
    public event Action<Item> OnReplaceItem;

    public bool IsReplacingItem { get; private set; } = false;

    private void Start()
    {
        for (int i = 0; i < MAX_WEAPON_COUNT; i++)
        {
            var wep = Instantiate(_weaponPrefab, _weaponHolder).GetComponent<Weapon>();
            wep.gameObject.SetActive(false);
            _spawnedWeapons.Add(wep.gameObject);
        }
        // AddItem(_startingWeapon);
        Weapons.Add(_startingWeapon);
        UpdateWeaponsVisuals();
    }

    public void AddItem(Item itemToAdd)
    {
        if (itemToAdd.Data.Type == ItemType.WEAPON)
        {
            if (Weapons.Count == MAX_WEAPON_COUNT)
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

        if (CurrentWeaponIndex == -1 && Weapons.Count > 0)
        {
            CurrentWeaponIndex = 0;
            EquipWeapon(CurrentWeaponIndex, force: true);
        }
    }

    public void EquipWeapon(int index, bool force = false)
    {
        if (index == 99)
        {
            foreach (var weap in _spawnedWeapons)
            {
                weap.GetComponent<Weapon>().Unequip();
            }
            return;
        }

        if (Weapons.Count <= index || (CurrentWeaponIndex == index && !force)) return;

        var weapon = _spawnedWeapons[index].GetComponent<Weapon>();
        if (CurrentWeapon != weapon)
            CurrentWeapon?.Unequip();

        CurrentWeaponIndex = index;
        weapon.Equip();
    }
}

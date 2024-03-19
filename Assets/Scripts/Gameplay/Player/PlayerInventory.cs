using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class PlayerInventory : SingletonBehaviour<PlayerInventory>
{
    [ShowNonSerializedField][ReadOnly] private const int MAX_WEAPON_COUNT = 4;

    public List<ItemData> Weapons { get; private set; } = new();
    public List<ItemData> Items { get; private set; } = new();

    public List<GameObject> SpawnedWeapons { get; } = new(MAX_WEAPON_COUNT);
    public int CurrentWeaponIndex { get; private set; } = -1;

    public Weapon CurrentWeapon
    {
        get
        {
            if (CurrentWeaponIndex < Weapons.Count && CurrentWeaponIndex >= 0)
                return SpawnedWeapons[CurrentWeaponIndex].GetComponent<Weapon>();
            return null;
        }
    }

    [Header("Settings")]
    [SerializeField] GameObject _weaponPrefab;
    [SerializeField] Transform _weaponHolder;
    [SerializeField] Transform _itemsHolder;
    [SerializeField] ItemData _startingWeapon;
    [SerializeField] AudioClip _genericPickupSfx;

    public event Action<Weapon> OnWeaponEquip;
    public event Action<ItemData> OnItemAdd;
    public event Action<ItemData> OnItemRemove;

    public bool IsReplacingItem { get; private set; } = false;

    private void Start()
    {
        for (int i = 0; i < MAX_WEAPON_COUNT; i++)
        {
            var wep = Instantiate(_weaponPrefab, _weaponHolder).GetComponent<Weapon>();
            wep.gameObject.SetActive(false);
            SpawnedWeapons.Add(wep.gameObject);
        }
        Weapons.Add(_startingWeapon);
        UpdateWeaponsVisuals();
    }

    public void AddItem(Item itemToAdd)
    {
        bool showNotif = true;
        if (itemToAdd.Data.Type == ItemType.WEAPON)
        {
            if (!Weapons.Contains(itemToAdd.Data))
            {
                if (Weapons.Count == MAX_WEAPON_COUNT) return;
                Weapons.Add(itemToAdd.Data);
            }
            else
            {
                TryAddAmmo(itemToAdd.Data);
                showNotif = false;
            }

            UpdateWeaponsVisuals();
        }
        else
        {
            var item = itemToAdd.Data;
            if (Items.Contains(item) && !item.IsStackable) return;

            Items.Add(item);
            if (item.PickupObject != null) Instantiate(item.PickupObject, _itemsHolder);
        }

        // pickup sound
        var pickupSfx = itemToAdd.Data.PickupAudio != null ? itemToAdd.Data.PickupAudio : _genericPickupSfx;
        GetComponent<AudioSource>().PlayOneShot(pickupSfx, 0.3f);

        OnItemAdd?.Invoke(itemToAdd.Data);
        if (showNotif)
            Notifications.Instance.Notify("Picked up " + itemToAdd.Data.FriendlyName);
        Destroy(itemToAdd.gameObject);
    }

    void UpdateWeaponsVisuals()
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            var wep = Weapons[i];
            var onHand = SpawnedWeapons[i];

            var weapon = onHand.GetComponent<Weapon>();
            weapon.Data = wep;
            if (weapon.Ammo == 0)
                weapon.Ammo = wep.InitialAmmo;
            weapon.UpdateVisuals();
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
            foreach (var weap in SpawnedWeapons)
            {
                weap.GetComponent<Weapon>().Unequip();
            }
            return;
        }

        if (Weapons.Count <= index || (CurrentWeaponIndex == index && !force)) return;

        var weapon = SpawnedWeapons[index].GetComponent<Weapon>();
        if (CurrentWeapon != weapon)
            CurrentWeapon.Unequip();

        CurrentWeaponIndex = index;
        OnWeaponEquip?.Invoke(weapon);
        weapon.Equip();
    }

    public bool TryAddAmmo(ItemData weapon, int ammo = 0)
    {
        foreach (var weap in SpawnedWeapons)
        {
            var wep = weap.GetComponent<Weapon>();
            if (wep.Data == null) break;
            if (wep.Data.FriendlyName == weapon.FriendlyName)
            {
                var ammoToAdd = ammo == 0 ? weapon.InitialAmmo : ammo;
                wep.Ammo += ammoToAdd;

                Notifications.Instance.Notify($"Picked up {ammoToAdd} {weapon.FriendlyName} ammo");
                GetComponent<AudioSource>().PlayOneShot(_genericPickupSfx, 0.3f);
                return true;
            }
        }

        return false;
    }

    public bool RemoveItem(string itemName)
    {
        var item = Items.Where(i => i.FriendlyName == itemName).FirstOrDefault();
        if (item == null) return false;

        OnItemRemove?.Invoke(item);
        Items.Remove(item);
        return true;
    }

    public void ClearWeaponTargets()
    {
        foreach (var weap in SpawnedWeapons)
        {
            var weapon = weap.GetComponent<Weapon>();
            weapon.Target = null;
        }
    }
}

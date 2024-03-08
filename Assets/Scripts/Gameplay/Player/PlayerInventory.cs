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

    public event Action<Weapon> OnWeaponEquip;
    public event Action<Item> OnItemAdd;

    public bool IsReplacingItem { get; private set; } = false;

    private void Start()
    {
        for (int i = 0; i < MAX_WEAPON_COUNT; i++)
        {
            var wep = Instantiate(_weaponPrefab, _weaponHolder).GetComponent<Weapon>();
            wep.gameObject.SetActive(false);
            _spawnedWeapons.Add(wep.gameObject);
        }
        Weapons.Add(_startingWeapon);
        UpdateWeaponsVisuals();
    }

    public void AddItem(Item itemToAdd)
    {
        if (itemToAdd.Data.Type == ItemType.WEAPON)
        {
            if (!Weapons.Contains(itemToAdd.Data))
            {
                if (Weapons.Count == MAX_WEAPON_COUNT)
                {
                    // StartCoroutine(ReplaceItem(itemToAdd));
                    // !Play *Source engine disabled button sound*
                    return;
                }

                Weapons.Add(itemToAdd.Data);
            }
            else
                AddAmmoToWeapon(itemToAdd.Data);

            UpdateWeaponsVisuals();
        }

        // pickup sound
        if (itemToAdd.Data.PickupAudio != null)
        {
            GetComponent<AudioSource>().PlayOneShot(itemToAdd.Data.PickupAudio, 0.3f);
        }

        OnItemAdd?.Invoke(itemToAdd);
        Notifications.Instance.Notify("Picked up " + itemToAdd.Data.FriendlyName);
        Destroy(itemToAdd.gameObject);
    }

    void UpdateWeaponsVisuals()
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            var wep = Weapons[i];
            var onHand = _spawnedWeapons[i];

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
        OnWeaponEquip?.Invoke(weapon);
        weapon.Equip();
    }

    void AddAmmoToWeapon(ItemData weapon)
    {
        print(weapon.FriendlyName + " in inventory. Adding ammo..");
        foreach (var weap in _spawnedWeapons)
        {
            var wep = weap.GetComponent<Weapon>();
            if (wep.Data.FriendlyName == weapon.FriendlyName)
            {
                print(weapon.InitialAmmo);
                wep.Ammo += weapon.InitialAmmo;
                return;
            }
        }
    }
}

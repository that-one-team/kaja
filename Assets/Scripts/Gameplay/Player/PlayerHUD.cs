using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Room")]
    [SerializeField] CanvasGroup _newRoomGroup;
    [SerializeField] TextMeshProUGUI _worldText;
    [SerializeField] TextMeshProUGUI _roomText;

    [Header("Gameplay")]
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] Image _skillPickupIndicator;
    [SerializeField] TextMeshProUGUI _scoreText;
    [SerializeField] TextMeshProUGUI _timerText;

    [Header("Weapons")]
    [SerializeField] Image _currentWeaponImage;
    [SerializeField] TextMeshProUGUI _currentWeaponText;
    [SerializeField] TextMeshProUGUI _currentWeaponAmmo;

    [Header("Items")]
    [SerializeField] GameObject _itemUIPrefab;
    [SerializeField] Transform[] _itemSlots;
    ItemSlot[] _slots;

    WorldBrain _currentWorld;

    GameStopwatch _timer;

    void Start()
    {
        Initialize(WorldManager.Instance.CurrentWorld);
        WorldManager.Instance.OnWorldChange += Initialize;
        PlayerScore.Instance.OnAddScore += (int score) => _scoreText.text = score.ToString();
        PlayerInventory.Instance.OnWeaponEquip += EquipWeapon;
        PlayerInventory.Instance.OnItemAdd += OnItemAdd;
        PlayerInventory.Instance.OnItemRemove += OnItemRemove;
        _timer = GameStopwatch.Instance;
    }

    private void OnItemAdd(ItemData item)
    {
        if (item.Type != ItemType.ITEM) return;
        var slot = _itemSlots.Where(i => i.transform.childCount < 3).FirstOrDefault();
        if (slot == null) return;

        var addedItem = Instantiate(_itemUIPrefab, slot).GetComponent<ItemSlot>();
        addedItem.name = item.FriendlyName;
        addedItem.QuantityUI.text = "";
        addedItem.ImageUI.sprite = item.UISprite;
    }

    private void OnItemRemove(ItemData item) =>
        _itemSlots.SelectMany(slot => slot.Cast<Transform>())
            .Where(itemSlot => itemSlot.name == item.FriendlyName)
            .ToList()
            .ForEach(ItemSlot => Destroy(ItemSlot.gameObject));

    private void EquipWeapon(Weapon weapon)
    {
        _currentWeaponText.text = weapon.Data.FriendlyName;
        _currentWeaponImage.sprite = weapon.Data.UISprite;
    }

    private void Initialize(WorldBrain brain)
    {
        _currentWorld = WorldManager.Instance.CurrentWorld;
        _currentWorld.OnChangeRoom += ChangeRoom;
        Player.Instance.OnHealthChanged += PlayerHurt;
        PlayerSkills.Instance.OnSkillPickup += SkillPickup;
    }

    void SkillPickup(SkillData skill)
    {
        _skillPickupIndicator.gameObject.SetActive(true);
        _skillPickupIndicator.color = skill.PickupIndicatorColor;
        _skillPickupIndicator.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(() => _skillPickupIndicator.gameObject.SetActive(false));
    }

    void PlayerHurt(int changed, int remaining)
    {
        _hpText.text = remaining.ToString();

        var seq = DOTween.Sequence();
        seq.Append(_hpText.transform.DOScale(1.5f, 0.15f));
        seq.Append(_hpText.transform.DOScale(1f, 0.15f));
        seq.Play();
    }

    void ChangeRoom(Room room)
    {
        if (room == null) return;
        _worldText.text = _currentWorld.name;
        _roomText.text = _currentWorld.CurrentRoom.FriendlyName;
        _newRoomGroup.gameObject.SetActive(true);

        _newRoomGroup.alpha = 0;
        var seq = DOTween.Sequence();
        seq.Append(_newRoomGroup.DOFade(1, 0.2f));
        seq.Append(_newRoomGroup.DOFade(0, 0.2f).SetDelay(3));
        seq.AppendCallback(() => _newRoomGroup.gameObject.SetActive(false));
        seq.Play();
    }

    void Update()
    {
        var time = TimeSpan.FromSeconds(_timer.CurrentTime);
        _timerText.text = time.ToString(@"mm\:ss");

        var currWeapon = PlayerInventory.Instance.CurrentWeapon;
        if (currWeapon != null)
            _currentWeaponAmmo.text = currWeapon.Data.InitialAmmo == 0 ? "-" : currWeapon.Ammo.ToSafeString();
    }

}

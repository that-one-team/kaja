using System;
using System.Collections;
using System.Collections.Generic;
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

    WorldBrain _currentWorld;

    GameStopwatch _timer;

    void Start()
    {
        Initialize(WorldManager.Instance.CurrentWorld);
        WorldManager.Instance.OnWorldChange += Initialize;
        PlayerScore.Instance.OnAddScore += (int score) => _scoreText.text = score.ToString();
        PlayerInventory.Instance.OnWeaponEquip += EquipWeapon;
        _timer = GameStopwatch.Instance;
    }

    private void EquipWeapon(Weapon weapon)
    {
        _currentWeaponText.text = weapon.Data.FriendlyName;
        _currentWeaponImage.sprite = weapon.Data.UISprite;
    }

    private void Initialize(WorldBrain brain)
    {
        print("initialized hud");
        _currentWorld = WorldManager.Instance.CurrentWorld;
        _currentWorld.OnChangeRoom += ChangeRoom;
        Player.Instance.OnHurt += PlayerHurt;
        PlayerSkills.Instance.OnSkillPickup += SkillPickup;
    }

    void SkillPickup(SkillData skill)
    {
        _skillPickupIndicator.gameObject.SetActive(true);
        _skillPickupIndicator.color = skill.PickupIndicatorColor;
        _skillPickupIndicator.DOFade(0, 0.5f).SetDelay(0.5f).OnComplete(() => _skillPickupIndicator.gameObject.SetActive(false));
    }

    void PlayerHurt(int remaining)
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

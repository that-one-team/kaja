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

    void Start()
    {
        WorldBrain.Instance.OnChangeRoom += ChangeRoom;
        Player.Instance.OnHurt += PlayerHurt;
        PlayerSkills.Instance.OnSkillPickup += SkillPickup;

        PlayerHurt(Player.Instance.Health);
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
        _worldText.text = WorldBrain.Instance.name;
        _roomText.text = WorldBrain.Instance.CurrentRoom.FriendlyName;
        _newRoomGroup.gameObject.SetActive(true);

        _newRoomGroup.alpha = 0;
        var seq = DOTween.Sequence();
        seq.Append(_newRoomGroup.DOFade(1, 0.2f));
        seq.Append(_newRoomGroup.DOFade(0, 0.2f).SetDelay(3));
        seq.AppendCallback(() => _newRoomGroup.gameObject.SetActive(false));
        seq.Play();
    }

}

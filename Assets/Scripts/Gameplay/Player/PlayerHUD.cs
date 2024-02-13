using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [Header("Room")]
    [SerializeField] CanvasGroup _newRoomGroup;
    [SerializeField] TextMeshProUGUI _worldText;
    [SerializeField] TextMeshProUGUI _roomText;

    void Start()
    {
        GameManager.Instance.OnChangeRoom += ChangeRoom;
    }

    void ChangeRoom(Room room)
    {
        if (room == null) return;
        _worldText.text = GameManager.Instance.CurrentWorld;
        _roomText.text = GameManager.Instance.ActiveRoom.FriendlyName;
        _newRoomGroup.gameObject.SetActive(true);

        _newRoomGroup.alpha = 0;
        var seq = DOTween.Sequence();
        seq.Append(_newRoomGroup.DOFade(1, 0.2f));
        seq.Append(_newRoomGroup.DOFade(0, 0.2f).SetDelay(3));
        seq.AppendCallback(() => _newRoomGroup.gameObject.SetActive(false));
        seq.Play();
    }

}

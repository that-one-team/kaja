using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Collections.Generic;
using NaughtyAttributes;

public class Notifications : SingletonBehaviour<Notifications>
{
    [ShowNonSerializedField]
    private static readonly int POOL_SIZE = 10;

    [SerializeField] GameObject _notificationPrefab;
    [ReadOnly][SerializeField] List<GameObject> _notifPool = new(POOL_SIZE);

    private void Start()
    {
        for (int i = 0; i < POOL_SIZE; i++)
        {
            var notif = Instantiate(_notificationPrefab, transform);
            notif.name = $"Notif{i + 1}";
            notif.SetActive(false);
            _notifPool.Add(notif);
        }
    }

    public void Notify(string message, float duration = 3)
    {
        foreach (var item in _notifPool)
        {
            if (item.activeSelf) continue;

            var text = item.GetComponent<TextMeshProUGUI>();
            text.text = message;
            item.SetActive(true);
            text.DOFade(0, 0.5f).SetDelay(duration).OnComplete(() => item.SetActive(false));
            break;
        }

    }
}
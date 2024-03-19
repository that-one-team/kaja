using System;
using System.Collections;
using UnityEngine;

public class VickxItem : MonoBehaviour
{
    Player _player;
    private void Start()
    {
        _player = Player.Instance;

        StartCoroutine(HealPlayer());
    }

    private IEnumerator HealPlayer()
    {
        while (Player.Instance.IsAlive)
        {
            if (Player.Instance.Health < Player.Instance.MaxHealth)
            {
                yield return new WaitForSeconds(3);
                Player.Instance.Heal(4);
            }
            yield return new WaitForEndOfFrame();
        }
    }


}
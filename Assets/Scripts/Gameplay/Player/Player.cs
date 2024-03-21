using System;
using System.Collections;
using System.Collections.Generic;
using TOT.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : LivingBeing
{
    [SerializeField] GameObject _deathCamera;
    public static Player Instance { get; private set; }
    public bool IsAlive { get => Health > 0; }

    private void Awake()
    {
        Instance = this;
        _deathCamera.SetActive(false);
    }
    [SerializeField] PlayerHUD _hud;

    public override void Revive()
    {
        base.Revive();
        _deathCamera.SetActive(false);
        _deathCamera.GetComponent<Animator>().SetBool("Death", false);
        GameManager.Instance.Freeze(false);

        PlayerInventory.Instance.ClearInventory();
    }

    public override void Die()
    {
        GameManager.Instance.Freeze(true);
        //i cant access the event froim here
        // skill issue most likely
        PlayerAudio.Instance.PlayDeath();
        _deathCamera.SetActive(true);
        _deathCamera.GetComponent<Animator>().SetBool("Death", true);

        StartCoroutine(Respawn());

        static IEnumerator Respawn()
        {
            yield return new WaitForSeconds(5);
            GameManager.Instance.Forfeit();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingBeing
{
    [SerializeField] GameObject _deathCamera;
    public static Player Instance { get; private set; }
    public bool IsAlive { get => Health > 0; }

    [Header("SFX")]
    [SerializeField] List<AudioClip> _hurtSFX;
    [SerializeField] AudioClip _deathSFX;

    private void Awake()
    {
        Instance = this;
        _deathCamera.SetActive(false);
    }
    [SerializeField] PlayerHUD _hud;

    public override void Revive()
    {
        base.Revive();
        _deathCamera.GetComponent<Animator>().SetBool("Death", false);
        GameManager.Instance.Freeze(false);
    }

    public override void Die()
    {
        GameManager.Instance.Freeze(true);
        _deathCamera.SetActive(true);
        _deathCamera.GetComponent<Animator>().SetBool("Death", true);

        StartCoroutine(Respawn());

        static IEnumerator Respawn()
        {
            yield return new WaitForSeconds(5);
            GameManager.Instance.Forfeit();
            Instance.Revive();
        }
    }
}
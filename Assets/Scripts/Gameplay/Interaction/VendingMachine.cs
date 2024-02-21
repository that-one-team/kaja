using System.Collections;
using System.Collections.Generic;
using TOT.Common;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VendingMachine : Interactable
{
    [SerializeField]
    AudioClip _vendingMachineUseSFX;

    [SerializeField]
    List<Skill> _skillsToDrop = new();

    AudioSource _source;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.spatialBlend = 1;
    }

    public override void Interact()
    {
        if (!IsInteractable) return;
        _source.PlayOneShot(_vendingMachineUseSFX);
        IsInteractable = false;
        ShowOutlines(false);

        StartCoroutine(DoGacha());
    }

    IEnumerator DoGacha()
    {
        yield return new WaitForSeconds(6);
        var skillToDrop = _skillsToDrop.SelectRandom();
        // print("GACHA! " + skillToDrop.name);

        var dropped = Instantiate(skillToDrop.gameObject, transform.position + transform.forward, Quaternion.identity);
        dropped.GetComponent<Skill>().enabled = false;
        var rb = dropped.GetComponent<Rigidbody>();
        var col = dropped.GetComponent<Collider>();
        col.isTrigger = false;
        rb.useGravity = true;
        rb.AddForce((transform.forward + transform.up) * 5, ForceMode.Impulse);
        yield return new WaitForSeconds(0.6f);
        dropped.GetComponent<Skill>().enabled = true;
        rb.useGravity = false;
        rb.isKinematic = true;
        col.isTrigger = true;
    }
}

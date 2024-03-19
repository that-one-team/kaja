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
    AudioClip _invalidSFX;

    [SerializeField]
    List<WeightedDrop> _itemsToDrop = new();

    public int ScoreCost;

    AudioSource _source;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.spatialBlend = 1;
        ScoreCost = 100;
    }

    public override void Interact()
    {
        if (!IsInteractable || PlayerScore.Instance.Score < ScoreCost)
        {
            _source.PlayOneShot(_invalidSFX, 0.2f);
            if (PlayerScore.Instance.Score < ScoreCost)
                Notifications.Instance.Notify($"You dont have enough score. Cost: {ScoreCost}");
            return;
        }

        PlayerScore.Instance.AddScore(-ScoreCost);
        _source.PlayOneShot(_vendingMachineUseSFX);
        ShowOutlines(false);

        StartCoroutine(DoGacha());
    }

    IEnumerator DoGacha()
    {
        IsInteractable = false;
        yield return new WaitForSeconds(6);
        var item = _itemsToDrop.SelectRandom();

        var dropped = Instantiate(item.Item, transform.position + Vector3.up + transform.forward, Quaternion.identity);
        var rb = dropped.GetComponent<Rigidbody>();
        var col = dropped.GetComponent<Collider>();
        col.isTrigger = false;
        rb.useGravity = true;
        rb.AddForce((transform.forward + transform.up) * 5, ForceMode.Impulse);

        yield return new WaitForSeconds(0.6f);
        rb.useGravity = false;
        rb.isKinematic = true;
        col.isTrigger = true;
        IsInteractable = true;
    }
}

using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Billboard))]
public class AmmoDrop : Interactable
{
    public ItemData Data;
    public int AmmoToGive;
    [SerializeField] LayerMask _mask;

    bool _shouldFreeze = false, _isFrozen = false;

    private void OnValidate()
    {
        if (Data == null || Data.AmmoSprite == null) return;

        GetComponent<TrailRenderer>().startColor = Data.LightColor;
    }

    private void Start()
    {
        GetComponent<Outline>().enabled = false;
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        GetComponent<Collider>().isTrigger = false;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100, _mask))
            transform.position = hit.point;

        float angle = Random.Range(0f, 2f * Mathf.PI);
        float force = 3;
        Vector3 dir = new(Mathf.Cos(angle) * force, 12, Mathf.Sin(angle) * force);
        rb.AddForce(dir, ForceMode.Impulse);

        var rend = GetComponent<Renderer>();
        rend.material.mainTexture = Data.AmmoSprite;
        rend.material.SetTexture("_EmissionMap", Data.AmmoSprite);

        AmmoToGive = Random.Range(Data.InitialAmmo, Data.InitialAmmo + (int)(1 + PlayerScore.Instance.Score * 0.005f));
        Data.InitialAmmo = AmmoToGive;

        StartCoroutine(DelayDrop());

        IEnumerator DelayDrop()
        {
            yield return new WaitForSeconds(0.1f);
            _shouldFreeze = true;
        }
    }

    public override void Interact()
    {
        if (PlayerInventory.Instance.TryAddAmmo(Data, AmmoToGive)) Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!_shouldFreeze) return;
        GetComponent<Collider>().isTrigger = true;
        var rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        _isFrozen = true;
    }

    void Update()
    {
        if (!_isFrozen) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1000, LayerMask.GetMask("Default")))
        {
            transform.position = hit.point + Vector3.up * 0.5f;
        }
    }
}

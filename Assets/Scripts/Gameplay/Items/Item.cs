using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Billboard))]
public class Item : MonoBehaviour
{
    [Expandable]
    public ItemData Data;

    private void OnValidate()
    {
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 1;

        if (Data == null) return;
        UpdateVisuals();
    }

    [Button("Update visuals")]
    public virtual void UpdateVisuals()
    {
        if (Data == null) return;
        var tex = Data.HandSprite != null ? Data.HandSprite : Data.WorldSprite;
        GetComponent<Renderer>().material.mainTexture = tex;
        GetComponent<Renderer>().material.SetTexture("_EmissionMap", tex);

        name = Data.FriendlyName;
        transform.localScale = Vector3.one * Data.Scale;
    }
}

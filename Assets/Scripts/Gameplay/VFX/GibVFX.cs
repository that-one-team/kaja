using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GibVFX : MonoBehaviour
{
    AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public virtual void DoGib(AudioClip clip)
    {
        _source.spatialBlend = 1;
        _source.PlayOneShot(clip);
        Destroy(gameObject, 5);
    }
}
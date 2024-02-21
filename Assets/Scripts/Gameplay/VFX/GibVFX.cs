using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GibVFX : MonoBehaviour
{
    [SerializeField] bool _destroyAfterTime;

    AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public virtual void DoGib(AudioClip clip)
    {
        transform.eulerAngles = Vector3.up * Random.Range(0, 360);
        _source.spatialBlend = 1;
        _source.PlayOneShot(clip);
        Destroy(gameObject, _destroyAfterTime ? 5 : 100000);
    }
}
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GibVFX : MonoBehaviour
{
    [SerializeField] bool _destroyAfterTime;
    AudioSource _source;

    [SerializeField] Texture2D _tex;
    [SerializeField] Renderer _sprite;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        if (_sprite != null && _tex != null)
            _sprite.material.SetTexture("_MainTex", _tex);
    }

    public virtual void DoGib(AudioClip clip)
    {
        transform.eulerAngles = Vector3.up * Random.Range(0, 360);
        _source.spatialBlend = 1;
        _source.PlayOneShot(clip);
        Destroy(gameObject, _destroyAfterTime ? 5 : 100000);
    }
}
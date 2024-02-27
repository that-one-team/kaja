using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : Item
{
    [SerializeField] float _equipSpeed = 0.2f;
    protected bool IsReady = false;

    bool _isShooting;
    float _timer;

    AudioSource _source;
    [SerializeField] LayerMask _layerMask;

    // too lazy to rewrite this to Data.Offset LMAO
    Vector3 _offset;

    private void Start()
    {
        _timer = Data.FireRate;
        _source = GetComponent<AudioSource>();

        _offset = Data.Offset;

        transform.localScale = Vector3.one * Data.Scale;
    }

    public void Equip()
    {
        print("equipped " + Data.FriendlyName);
        transform.localPosition = Data.Offset + Vector3.down;
        gameObject.SetActive(true);
        transform.DOLocalMove(Data.Offset, _equipSpeed).OnComplete(() => IsReady = true);
    }

    public void Unequip()
    {
        transform.DOLocalMove(_offset + Vector3.down, _equipSpeed).OnComplete(() =>
        {
            gameObject.SetActive(false);
            IsReady = false;
        });
    }

    public virtual void Shoot()
    {
        if (!IsReady) return;
        if (_timer > 0 && _isShooting) return;

        _isShooting = true;
        _timer = Data.FireRate;
        _source.PlayOneShot(Data.ShootAudio);
        Vector3 endpoint = transform.forward * 10000;
        DoAnimation();

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 1000, _layerMask))
        {
            if (hit.collider.TryGetComponent(out LivingBeing being))
            {
                being.Damage(Data.Damage);
                if (being is Enemy)
                    (being as Enemy).Knockback(transform.forward * Data.KnockbackForce);
            }

            endpoint = hit.point;
        }

        if (Data.UseItemVFX == null) return;

        var vfx = Instantiate(Data.UseItemVFX, Camera.main.transform.position + Camera.main.transform.forward * 1f, Quaternion.identity).GetComponent<ItemVFX>();
        vfx.DoVFX(Data, vfx.transform.position, endpoint);
    }

    private void Update()
    {
        if (!_isShooting) return;

        _timer -= Time.deltaTime;
    }

    protected virtual void DoAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(_offset + Data.EndPose, 0.05f));
        seq.Append(transform.DOLocalMove(_offset, 0.2f));
        seq.Play();
    }
}
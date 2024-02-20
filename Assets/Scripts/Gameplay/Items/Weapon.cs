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

    private void Start()
    {
        _timer = Data.FireRate;
        _source = GetComponent<AudioSource>();
    }

    public void Equip()
    {
        transform.localPosition = Vector3.down;
        gameObject.SetActive(true);
        transform.DOLocalMoveY(0, _equipSpeed).OnComplete(() => IsReady = true);
    }

    public void Unequip()
    {
        transform.DOLocalMoveY(Vector3.down.y, _equipSpeed).OnComplete(() =>
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

}
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Weapon : Item
{
    [SerializeField] float _equipSpeed = 0.2f;
    protected bool IsReady = false;

    public int Ammo;

    bool _isShooting;
    float _timer;

    AudioSource _source;
    [SerializeField] LayerMask _layerMask;

    public GameObject Target;

    // too lazy to rewrite this to Data.Offset LMAO
    Vector3 _offset;

    public float FireRateMultiplier = 1;

    private void Start()
    {
        _timer = Data.FireRate;
        _source = GetComponent<AudioSource>();

        _offset = Data.Offset;

        transform.localScale = Vector3.one * Data.Scale;
    }

    public void Equip()
    {
        transform.localPosition = Data.Offset + Vector3.down;
        gameObject.SetActive(true);
        transform.DOLocalMove(Data.Offset, _equipSpeed).OnComplete(() => IsReady = true);
        Start();
        if (Data.EquipAudio != null)
            _source.PlayOneShot(Data.EquipAudio, 0.5f);
    }

    public void Unequip()
    {
        transform.DOLocalMove(_offset + Vector3.down, _equipSpeed).OnComplete(() =>
        {
            // gameObject.SetActive(false);
            IsReady = false;
        });
    }

    public virtual void Shoot()
    {
        if (!IsReady || GameManager.Instance.IsPaused || !Player.Instance.IsAlive) return;
        if (_timer > 0 && _isShooting) return;
        if (Data.InitialAmmo != 0 && Ammo <= 0) return;
        if (PlayerInventory.Instance.CurrentWeapon != this) return;

        _isShooting = true;
        _timer = Data.FireRate * FireRateMultiplier;
        _source.PlayOneShot(Data.ShootAudio);
        Vector3 endpoint = transform.forward * 10000;
        DoAnimation();

        if (Data.IsProjectile) ProjectileShoot();
        else HitscanShoot(ref endpoint);

        // i didnt use clause guard for this cuz i wanted it to look nice LMAO
        if (Data.UseItemVFX != null)
        {
            var vfx = Instantiate(Data.UseItemVFX, Camera.main.transform.position + Camera.main.transform.forward * 1f, Quaternion.identity).GetComponent<ItemVFX>();
            vfx.DoVFX(Data, vfx.transform.position, endpoint);
        }

        if (Data.InitialAmmo != 0)
            Ammo--;
    }

    private void Update()
    {
        if (!_isShooting) return;
        _timer -= Time.deltaTime;
    }

    protected virtual void ProjectileShoot()
    {
        var projectile = Instantiate(Data.ProjectilePrefab, Camera.main.transform.position + Camera.main.transform.forward, Camera.main.transform.rotation);
        if (projectile.TryGetComponent(out Projectile proj))
        {
            proj.Spawn(Data.Damage, Camera.main.transform.forward * Data.ProjectileForce);
        }
        else
            Destroy(projectile);

        var player = PlayerController.Instance.gameObject;
        var forceMult = player.GetComponent<Rigidbody>().velocity.magnitude > 0 ? 0.5f : 1;
        player.GetComponent<Rigidbody>().AddForce(Data.KnockbackForce * forceMult * -player.transform.forward, ForceMode.Impulse);
    }

    protected virtual void HitscanShoot(ref Vector3 endpoint)
    {
        if (Target)
        {
            if (Target.TryGetComponent(out LivingBeing being))
            {
                being.Damage(Data.Damage);
                if (being is Enemy)
                    (being as Enemy).Knockback(transform.forward * Data.KnockbackForce);
                endpoint = being.transform.position + Vector3.up;
            }
            return;
        }

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Data.Range, _layerMask))
        {
            if (hit.collider.TryGetComponent(out LivingBeing being))
            {
                being.Damage(Data.Damage);
                if (being is Enemy)
                    (being as Enemy).Knockback(transform.forward * Data.KnockbackForce);
            }

            endpoint = hit.point;
        }
    }

    protected virtual void DoAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(_offset + Data.EndPose, 0.05f));
        seq.Append(transform.DOLocalMove(_offset, 0.2f));
        seq.Play();
    }
}